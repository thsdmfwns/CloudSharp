using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Data.EntityFramework.Repository;
using CloudSharp.Data.Store;
using CloudSharp.Share.Enum;
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class ShareService
{
    private const string Password = "password";
    const string _volumePath  = "/tmp/cloud_sharp";
    private Respawner _respawner = null!;
    private Faker _faker = null!;
    private IShareService _shareService = null!;
    private DbConnection _dbConnection = null!;
    private DatabaseContext _databaseContext = null!;
    private IFileStore _fileStore = null!;
    private Member _seededMember = null!;
    private List<Data.EntityFramework.Entities.Share> _seededShares = null!;
    private string _memberDirectoryPath = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        if (Directory.Exists(_volumePath))
        {
            Directory.Delete(_volumePath, true);
        }
        _databaseContext = DatabaseUtil.GetDatabaseContext();
        _dbConnection = _databaseContext.Database.GetDbConnection();
        _respawner = await _dbConnection.GetRespawner();
        _faker = new Faker();
        _fileStore = new FileStore(_volumePath);
;       await _databaseContext.MigrateAsync();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        _shareService = new Api.Service.ShareService(new ShareRepository(_databaseContext), _fileStore);
        await _respawner.ResetAsync(_dbConnection);
        _seededMember = (await _databaseContext.SeedMembers(1)).Single();
        _memberDirectoryPath = _fileStore.GetTargetPath(DirectoryType.Member, _seededMember.MemberId, ".");
        _seededShares = await _databaseContext.SeedShares(_seededMember);
        if (Directory.Exists(_memberDirectoryPath))
        {
            Directory.Delete(_memberDirectoryPath, true);
        }
        Directory.CreateDirectory(_memberDirectoryPath);
        _seededShares.ForEach(x => _faker.MakeFakeFile(_memberDirectoryPath, ".", x.FilePath));
    }
    public void Dispose()
    {
        _dbConnection.Close();
        _databaseContext.Dispose();
    }

    [Test]
    [TestCase(null, null)] // success
    [TestCase("", typeof(NotFoundError))] // invalid id
    public async Task GetShare(string? shareIdString, Type? errorType)
    {
        var shareId = shareIdString?.ToGuid() ?? _seededShares.First().ShareId;

        var findResult = await _shareService.GetShare(shareId);
        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess);
            Assert.That(findResult.Value.ShareId, Is.EqualTo(shareId.ToString()));
            return;
        }
        
        //fail
        Assert.That(findResult.IsFailed);
        Assert.That(findResult.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null,  null)] // success
    [TestCase(null, "not_password",  typeof(UnauthorizedError))] // invalid password
    [TestCase("", null,  typeof(NotFoundError))] // invalid id
    public async Task VerifySharePassword(string? shareIdString, string? password, Type? errorType)
    {
        password ??= Password;
        var lockShare = (await _databaseContext.SeedShares(_seededMember, password:Password, count:1)).First();
        var shareId = shareIdString?.ToGuid() ?? lockShare.ShareId;

        var result = await _shareService.VerifySharePassword(shareId, password);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    /*
     *
       ValueTask<Result<List<ShareDto>>> GetSharesByMemberId(Guid memberId);
       ValueTask<Result<List<ShareDto>>> GetShareInFolder(Guid memberId, string folderPath);

       ValueTask<Result<Guid>> AddShare(Guid memberId, string filePath, string? password, DateTime? expireTime);

       ValueTask<Result> UpdateExpireTimeShare(Guid shareId, DateTime? expireTime);
       ValueTask<Result> UpdatePassword(Guid shareId, string? password);

       ValueTask<Result> DeleteShare(Guid shareId);
       ValueTask<Result> DeleteShareByFilePath(Guid memberId, string filePath);
       ValueTask<Result> DeleteShareInFolder(Guid memberId, string folderPath);
     */
}