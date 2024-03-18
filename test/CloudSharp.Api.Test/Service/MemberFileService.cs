using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Store;
using CloudSharp.Api.Test.Util;
using CloudSharp.Share.Enum;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Internal;

namespace CloudSharp.Api.Test.Service;

public class MemberFileService
{
    private IMemberFileService _memberFileService = null!;
    private Guid _memberDirectoryId;
    private string _memberDirectoryPath = null!;
    const string FolderName  = "Folder";
    private IDirectoryPathStore _directoryPathStore = null!;
    private Faker _faker = null!;
    private string _fileInDirectory = null!;
    private string _fileInFolder= null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _directoryPathStore = new DirectoryPathStore("/tmp/cloud_sharp"); 
        _memberDirectoryId = Guid.NewGuid();
        _memberDirectoryPath =
            _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, _memberDirectoryId, "");
        _faker = new Faker();
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, FolderName));
    }

    [SetUp]
    public void SetUp()
    {
        _memberFileService =
            new Api.Service.MemberFileService(_directoryPathStore, NullLogger<Api.Service.MemberFileService>.Instance);
        Directory.Delete(_memberDirectoryPath, true);
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, FolderName));
        _fileInDirectory = _faker.MakeFakeFile(_memberDirectoryPath, null);
        _fileInFolder = _faker.MakeFakeFile(_memberDirectoryPath, FolderName);
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(null, FolderName, null)] // in folder success
    [TestCase("", null,  typeof(NotFoundError))] // wrong id
    [TestCase(null, "not_folder",  typeof(NotFoundError))] // wrong folder
    public void GetFiles(string? directoryIdString, string? targetFolderPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        var findResult = _memberFileService.GetFiles(directoryId, targetFolderPath);
        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            Assert.That(findResult.Value, Is.Not.Empty);
            return;
        }
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(null, null, null, true)] //in folder success
    [TestCase("", null,  typeof(NotFoundError))] // wrong id
    [TestCase(null, "not_file",  typeof(NotFoundError))] // wrong file
    public void GetFile(string? directoryIdString, string? targetPath, Type? errorType, bool isInFolder = false)
    {
        
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= isInFolder ? _fileInFolder : _fileInDirectory;
        var findResult = _memberFileService.GetFile(directoryId, targetPath);
        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            Assert.That(findResult.Value.Name, Is.EqualTo(Path.GetFileName(targetPath)));
            return;
        }
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }
}