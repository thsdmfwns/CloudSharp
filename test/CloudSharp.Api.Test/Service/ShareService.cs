using System.Collections.Immutable;
using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Api.Util;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Data.EntityFramework.Repository;
using CloudSharp.Data.Store;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class ShareService
{
    private const string Password = "password";
    const string _volumePath  = "/tmp/cloud_sharp";
    private const string _folderName = "folder";
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
        if (Directory.Exists(_memberDirectoryPath))
        {
            Directory.Delete(_memberDirectoryPath, true);
        }
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, _folderName));
        _seededShares = await SeedShare(_seededMember);
    }
    public void Dispose()
    {
        _dbConnection.Close();
        _databaseContext.Dispose();
    }

    private async Task<List<Data.EntityFramework.Entities.Share>> SeedShare(
        Member? member, string folderPath = ".", string? password = null, DateTime? expireTime = null, int count = 10)
    {
        member ??= _seededMember;
        var memberDirectory = _fileStore.GetTargetPath(DirectoryType.Member, member.MemberId, ".");
        var seeded = await _databaseContext.SeedShares(member, password, expireTime, count, folderPath);
        seeded.ForEach(x => _faker.MakeFakeFile(memberDirectory, ".", x.FilePath));
        return seeded;
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
        var lockShare = (await SeedShare(_seededMember, _folderName, count: 1, password : Password)).First();
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

    [Test]
    [TestCase(null, null)] //success
    [TestCase("", null)] //empty id
    public async Task GetSharesByMemberId(string? memberIdString, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _seededMember.MemberId;

        var result = await _shareService.GetSharesByMemberId(memberId);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = _seededShares
                .Where(x => x.MemberId == memberId)
                .Select(x => x.ToShareDto())
                .ToList();
            result.Value.ForEach(ac => Assert.That(expect.Any(ex => ac == ex), Is.True));            
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null)] // success
    [TestCase(null, "not_folder", typeof(NotFoundError))] // invalid folder
    [TestCase(null, "../../folder", typeof(BadRequestError))] // invalid folder
    [TestCase("", null, typeof(NotFoundError))] // invalid id
    public async Task GetShareInFolder(string? memberIdString, string? folderPath, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _seededMember.MemberId;
        folderPath ??= _folderName;
        _seededShares.AddRange(await SeedShare(_seededMember, _folderName, count: 5));
        var result = await _shareService.GetShareInFolder(memberId, folderPath);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = _seededShares
                .Where(x => x.FilePath.StartsWith(folderPath))
                .Select(x => x.ToShareDto())
                .ToList();
            result.Value.ForEach(ac => Assert.That(expect.Any(ex => ex == ac)));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null, null, null)] //success
    [TestCase("", null, null, null, typeof(NotFoundError))] //invalid memberId
    [TestCase(null, "not_file", null, null, typeof(NotFoundError))] //invalid path
    [TestCase(null, "../../file", null, null, typeof(BadRequestError))] //invalid path
    public async Task AddShare(string? memberIdString, string? filePath, string? password, DateTime? expireTime, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _seededMember.MemberId;
        filePath ??= _faker.MakeFakeFile(_memberDirectoryPath, ".");

        var result = await _shareService.AddShare(memberId, filePath, password, expireTime);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var inserted = await _databaseContext.Shares.FindAsync(result.Value);
            Assert.That(inserted is not null);
            var expect = new ShareDto
            {
                ShareId = result.Value.ToString(),
                MemberId = memberId.ToString(),
                ExpireTime = new DateTimeOffset(expireTime ?? DateTime.MaxValue).ToUnixTimeSeconds(),
                HasPassword = password is not null,
                FileName = Path.GetFileName(filePath)
            };
            var actual = inserted!.ToShareDto();
            Assert.That(actual, Is.EqualTo(expect));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }
    /*
     *
       ValueTask<Result> UpdateExpireTimeShare(Guid shareId, DateTime? expireTime);
       ValueTask<Result> UpdatePassword(Guid shareId, string? password);

       ValueTask<Result> DeleteShare(Guid shareId);
       ValueTask<Result> DeleteShareByFilePath(Guid memberId, string filePath);
       ValueTask<Result> DeleteShareInFolder(Guid memberId, string folderPath);
     */
}