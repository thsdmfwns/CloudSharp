using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data.Store;
using CloudSharp.Share.Enum;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudSharp.Api.Test.Service;

public class MemberFileService
{
    private IMemberFileService _memberFileService = null!;
    private Guid _memberDirectoryId;
    private string _memberDirectoryPath = null!;
    const string _folderName  = "Folder";
    const string _emptyFolderName  = "EmptyFolder";
    const string _volumePath  = "/tmp/cloud_sharp";
    private IFileStore _fileStore = null!;
    private Faker _faker = null!;
    private string _fileInDirectory = null!;
    private string _fileInFolder= null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (Directory.Exists(_volumePath))
        {
            Directory.Delete(_volumePath, true);
        }
        _fileStore = new FileStore(_volumePath); 
        _memberDirectoryId = Guid.NewGuid();
        _memberDirectoryPath =
            _fileStore.GetTargetPath(DirectoryType.Member, _memberDirectoryId, "");
        _faker = new Faker();
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, _folderName));
    }

    [SetUp]
    public void SetUp()
    {
        _memberFileService =
            new Api.Service.MemberFileService(_fileStore, NullLogger<Api.Service.MemberFileService>.Instance);
        Directory.Delete(_memberDirectoryPath, true);
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, _folderName));
        Directory.CreateDirectory(Path.Combine(_memberDirectoryPath, _emptyFolderName));
        _fileInDirectory = _faker.MakeFakeFile(_memberDirectoryPath, null);
        _fileInFolder = _faker.MakeFakeFile(_memberDirectoryPath, _folderName);
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(null, _folderName, null)] // in folder success
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
    public void GetFile(string? directoryIdString, string? targetPath,  Type? errorType, bool isInFolder = false)
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

    [Test]
    [TestCase(null, null, null, null, null)] //success
    [TestCase(null, null, null, "file.txt", null)] //change file name
    [TestCase("", null, null, null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "not_file", null, null, typeof(NotFoundError))] //wrong target
    [TestCase(null, null, "not_folder", null, typeof(NotFoundError))] //wrong folder
    [TestCase(null, null, null, " ", typeof(BadRequestError))] //wrong filename
    public void MoveFile(string? directoryIdString, string? targetPath, string? toFolderPath, string? fileName, Type? errorType)
    {
        
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _fileInDirectory;
        toFolderPath ??= _folderName;
        fileName ??= Path.GetFileName(_fileInDirectory);
        
        var moveResult = _memberFileService.MoveFile(directoryId, targetPath, Path.Combine(toFolderPath, fileName));
        if (errorType is null)
        {
            Assert.That(moveResult.IsSuccess, Is.True);
            var movedFilePath = _fileStore.GetTargetPath(DirectoryType.Member, directoryId,
                Path.Combine(toFolderPath, fileName));
            Assert.That(File.Exists(movedFilePath), Is.True);
            return;
        }
        //fail
        Assert.That(moveResult.IsFailed, Is.True);
        Assert.That(moveResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null)] //success
    [TestCase(null, null, "  ", typeof(BadRequestError))] //wrong file name
    [TestCase(null, null, "not/file", typeof(BadRequestError))] //wrong file name
    [TestCase("", null, null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "not_file", null, typeof(NotFoundError))] //wrong target
    public void RenameFile(string? directoryIdString, string? targetPath, string? fileName, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _fileInDirectory;
        fileName ??= _faker.System.FileName();

        var renameResult = _memberFileService.RenameFile(directoryId, targetPath, fileName);
        if (errorType is null)
        {
            Assert.That(renameResult.IsSuccess, Is.True);
            var renamedFilePath = _fileStore.GetTargetPath(DirectoryType.Member, directoryId, fileName);
            Assert.That(File.Exists(renamedFilePath), Is.True);
            return;
        }
        //fail
        Assert.That(renameResult.IsFailed, Is.True);
        Assert.That(renameResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)]
    [TestCase(null, "not_file", typeof(NotFoundError))] // wrong target
    [TestCase("", null, typeof(NotFoundError))] // wrong id
    public void RemoveFile(string? directoryIdString, string? targetPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _fileInDirectory;
        
        
        var removeResult = _memberFileService.RemoveFile(directoryId, targetPath);
        if (errorType is null)
        {
            Assert.That(removeResult.IsSuccess, Is.True);
            var removedFilePath = _fileStore.GetTargetPath(DirectoryType.Member, directoryId, targetPath);
            Assert.That(File.Exists(removedFilePath), Is.False);
            return;
        }
        //fail
        Assert.That(removeResult.IsFailed, Is.True);
        Assert.That(removeResult.HasError(x => x.GetType() == errorType), Is.True);
    }


    [Test]
    [TestCase(null, null, null)] // success
    [TestCase("", null, typeof(NotFoundError))] // wrong id
    [TestCase(null, "not_folder", typeof(NotFoundError))] // wrong folder
    public void GetFolders(string? directoryIdString, string? targetFolderPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;

        var findResult = _memberFileService.GetFolders(directoryId, targetFolderPath);
        
        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            var findFolder = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath ?? "");
            Assert.That(findFolder.Value.GetDirectories().Length, Is.EqualTo(findResult.Value.Count));
            return;
        }
        
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)] // success
    [TestCase("", null, typeof(NotFoundError))] // wrong id
    [TestCase(null, "not_folder", typeof(NotFoundError))] // wrong folder

    public void GetFolder(string? directoryIdString, string? targetPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _folderName;
        var findResult = _memberFileService.GetFolder(directoryId, targetPath);
        
        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            var findFolder = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath ?? "");
            Assert.That(findFolder.Value.Exists, Is.True);
            return;
        }
        
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null)] //success
    [TestCase("", null, null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "not_folder", null, typeof(NotFoundError))] //wrong target
    [TestCase(null, null, "not_folder", typeof(NotFoundError))] //wrong dest
    [TestCase(null, null, "", typeof(ConflictError))] //exist folder
    [TestCase(null, "", null, typeof(BadRequestError))] //empty target
    public void MoveFolder(string? directoryIdString, string? targetPath, string? toFolderPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _folderName;
        toFolderPath ??= _emptyFolderName;
        var destPath = Path.Combine(toFolderPath, Path.GetFileName(targetPath));
        var moveResult = _memberFileService.MoveFolder(directoryId, targetPath, destPath);

        if (errorType is null)
        {
            Assert.That(moveResult.IsSuccess, Is.True);
            var toFolder = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, toFolderPath);
            Assert.That(toFolder.Value.GetDirectories().Any(x => x.Name == _folderName), Is.True);
            return;
        }
        
        //fail
        Assert.That(moveResult.IsFailed, Is.True);
        Assert.That(moveResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null)] //success
    [TestCase(null, null, "new/folder", null)] //path folder name
    [TestCase("", null, null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "", null, typeof(BadRequestError))] //empty target
    [TestCase(null, "not_folder", null, typeof(NotFoundError))] //wrong target
    [TestCase(null, null, "", typeof(BadRequestError))] //wrong name
    public void RenameFolder(string? directoryIdString, string? targetPath, string? folderName, Type? errorType)
    {
        const string changeFolderName = "NewFolder";
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _folderName;
        folderName ??= changeFolderName;

        var renameResult = _memberFileService.RenameFolder(directoryId, targetPath, folderName);
        
        if (errorType is null)
        {
            Assert.That(renameResult.IsSuccess, Is.True);
            var directory = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, Path.Combine(folderName, ".."));
            Assert.That(directory.Value.GetDirectories().Any(x => x.Name == Path.GetFileName(folderName)), Is.True);
            return;
        }
        
        //fail
        Assert.That(renameResult.IsFailed, Is.True);
        Assert.That(renameResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "", typeof(BadRequestError))] //empty target
    [TestCase(null, "not_folder", typeof(NotFoundError))] //wrong target
    public void RemoveFolder(string? directoryIdString, string? targetPath, Type? errorType)
    {
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _folderName;

        var removeResult = _memberFileService.RemoveFolder(directoryId, targetPath);
            
        if (errorType is null)
        {
            Assert.That(removeResult.IsSuccess, Is.True);
            var directory = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, Path.Combine(targetPath, ".."));
            Assert.That(directory.Value.GetDirectories().Any(x => x.Name == targetPath), Is.False);
            return;
        }
        
        //fail
        Assert.That(removeResult.IsFailed, Is.True);
        Assert.That(removeResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null)] //success
    [TestCase("", null, null, typeof(NotFoundError))] //wrong id
    [TestCase(null, "not_folder", null, typeof(NotFoundError))] //wrong target
    [TestCase(null, "..", null, typeof(BadRequestError))] //wrong target
    [TestCase(null, null, "", typeof(BadRequestError))] //wrong name
    public void MakeFolder(string? directoryIdString, string? targetFolderPath, string? folderName, Type? errorType)
    {
        const string makeFolderName = "NewFolder";
        var directoryId = directoryIdString?.ToGuid() ?? _memberDirectoryId;
        targetFolderPath ??= _folderName;
        folderName ??= makeFolderName;

        var makeResult = _memberFileService.MakeFolder(directoryId, targetFolderPath, folderName);
        
        if (errorType is null)
        {
            Assert.That(makeResult.IsSuccess, Is.True);
            var directory = _fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath);
            Assert.That(directory.Value.GetDirectories().Any(x => x.Name == folderName), Is.True);
            return;
        }
        
        //fail
        Assert.That(makeResult.IsFailed, Is.True);
        Assert.That(makeResult.HasError(x => x.GetType() == errorType), Is.True);
    }
}