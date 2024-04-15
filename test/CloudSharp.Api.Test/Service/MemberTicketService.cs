using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data.Store;
using CloudSharp.Data.Ticket;
using CloudSharp.Share.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudSharp.Api.Test.Service;

public class MemberTicketService
{
    private IMemberTicketService _memberTicketService = null!;
    private ITicketStore _ticketStore = null!;
    const string _volumePath  = "/tmp/cloud_sharp";
    private IFileStore _fileStore = null!;
    private string _filePath = null!;
    private Guid _memberDirectoryId;
    private string _memberDirectoryPath = null!;
    private Faker _faker;
    
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
            _fileStore.GetTargetPath(DirectoryType.Member, _memberDirectoryId, ".");
        _faker = new Faker();
        _ticketStore = new HashMapTicketStore();
    }
    
    
    [SetUp]
    public void SetUp()
    {
        _memberTicketService =
            new Api.Service.MemberTicketService(_ticketStore, _fileStore, NullLogger<IMemberTicketService>.Instance);
        if (Directory.Exists(_memberDirectoryPath))
        {
            Directory.Delete(_memberDirectoryPath, true);
        }
        Directory.CreateDirectory(_memberDirectoryPath);
        _filePath = _faker.MakeFakeFile(_memberDirectoryPath, null);
    }
    
    [Test]
    [TestCase(null, null, null)] // success
    [TestCase("", null, typeof(NotFoundError))] // invalid id
    [TestCase(null, "not_file", typeof(NotFoundError))] // invalid target
    [TestCase(null, "../file", typeof(BadRequestError))] // invalid target
    public async Task AddFileStreamTicket(string? memberIdString, string? targetPath, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _memberDirectoryId;
        targetPath ??= _filePath;

        var result = await _memberTicketService.AddFileStreamTicket(memberId, targetPath);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(await _ticketStore.ExistTicket<FileStreamTicket>(result.Value), Is.True);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null)] //success
    [TestCase("", null, null, typeof(NotFoundError))] //invalid memberId
    [TestCase(null, "not_folder", null, typeof(NotFoundError))] //invalid target
    [TestCase(null, "..", null, typeof(BadRequestError))] //invalid target
    [TestCase(null, null, "exist", typeof(ConflictError))] //exist file name
    [TestCase(null, null, "../file.exe", typeof(BadRequestError))] //invalid file name
    [TestCase(null, null, "", typeof(BadRequestError))] //invalid file name
    public async Task AddFileUploadTicket(string? memberIdString, string? targetFolderPath, string? filename, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _memberDirectoryId;
        targetFolderPath ??= ".";
        filename ??= _faker.System.FileName();
        if (filename == "exist")
        {
            filename = Path.GetFileName(_filePath);
        }
        
        var result = await _memberTicketService.AddFileUploadTicket(memberId, targetFolderPath, filename);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(await _ticketStore.ExistTicket<FileUploadTicket>(result.Value), Is.True);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }
    
}