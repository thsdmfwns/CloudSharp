using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Api.Util;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Data.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class MemberService : IDisposable
{
    private const string Password = "password";
    private Respawner _respawner = null!;
    private IMemberService _memberService = null!;
    private DbConnection _dbConnection = null!;
    private DatabaseContext _databaseContext = null!;
    private List<Member> _seededMembers = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _databaseContext = DatabaseUtil.GetDatabaseContext();
        _dbConnection = _databaseContext.Database.GetDbConnection();
        _respawner = await _dbConnection.GetRespawner();
        await _databaseContext.MigrateAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        _memberService = new Api.Service.MemberService(new MemberRepository(_databaseContext),
            NullLogger<Api.Service.MemberService>.Instance);
        await _respawner.ResetAsync(_dbConnection);
        _seededMembers = await _databaseContext.SeedMembers();
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _databaseContext.Dispose();
    }

    [Test]
    [TestCase(null, null, null)] // success
    [TestCase(null, "not_password", typeof(UnauthorizedError))] // wrong password
    [TestCase("not_id", null, typeof(UnauthorizedError)) ] // wrong loginId
    public async Task Login(string? loginId, string? password, Type? errorType)
    {
        var member = _seededMembers.First();
        loginId ??= member.LoginId;
        password ??= Password;
        var loginResult = await _memberService.Login(loginId, password);
        
        if (errorType is null)
        {
            Assert.That(loginResult.IsSuccess, Is.True);
            return;
        }
        
        //fail
        Assert.That(loginResult.IsFailed, Is.True);
        Assert.That(loginResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, null, null, null, false)] // success
    [TestCase(null, null, null, null, null, typeof(ConflictError), true)] // existed id
    [TestCase(null, null, "not_email", null, null, typeof(BadRequestError), false)] // wrong email
    public async Task Register(string? id, string? password, string? email, string? nickname, Guid? profileUrl, Type? errorType, bool isExistedId)
    {
        var faker = new Faker<Member>().SetRules();
        var member = faker.Generate(1).First();
        id ??= member.LoginId ;
        if (isExistedId)
        {
            id = _seededMembers.First().LoginId;
        }
        password ??= member.Password;
        email ??= member.Email;
        nickname ??= member.Nickname;
        profileUrl ??= member.ProfileImageId;
        var registerResult = await _memberService.Register(
            id, password, email, nickname, profileUrl);
        
        if (errorType is null)
        {
            Assert.That(registerResult.IsSuccess, Is.True);
            var insertedMemberDto = registerResult.Value;
            var insertedMember = await _databaseContext.Members.FindAsync(Guid.Parse(insertedMemberDto.MemberId));
            Assert.That(insertedMember, Is.Not.Null);
            return;
        }

        //fail
        Assert.That(registerResult.IsFailed, Is.True);
        Assert.That(registerResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(NotFoundError))] //wrong id
    public async Task UpdateNickname(string? idString, string? nickname, Type? errorType)
    {
        const string updateNickname = "updateNickname";
        var memberId = idString?.ToGuid() ?? _seededMembers.First().MemberId;
        nickname ??= updateNickname;
        var updateResult = await _memberService.UpdateNickname(memberId, nickname);

        if (errorType is null)
        {
            Assert.That(updateResult.IsSuccess, Is.True);
            var updatedMember = await _databaseContext.Members.FindAsync(memberId);
            Assert.That(updatedMember!.Nickname, Is.EqualTo(updateNickname));
            return;
        }
        
        //fail
        Assert.That(updateResult.IsFailed, Is.True);
        Assert.That(updateResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(NotFoundError))] //wrong Id
    [TestCase(null, "not_email", typeof(BadRequestError))] //wrong email
    public async Task UpdateEmail(string? idString, string? email, Type? errorType)
    {
        var memberId = idString?.ToGuid() ?? _seededMembers.First().MemberId;
        email ??= new Faker().Internet.Email();
        
        var updateResult = await _memberService.UpdateEmail(memberId, email);

        if (errorType is null)
        {
            Assert.That(updateResult.IsSuccess, Is.True);
            var updatedMember = await _databaseContext.Members.FindAsync(memberId);
            Assert.That(updatedMember!.Email, Is.EqualTo(email));
            return;
        }
        
        //fail
        Assert.That(updateResult.IsFailed, Is.True);
        Assert.That(updateResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(NotFoundError))] //wrong id
    public async Task UpdatePassword(string? idString, string? password, Type? errorType)
    {
        const string updatePassword = "updatePassword";
        var memberId = idString?.ToGuid() ?? _seededMembers.First().MemberId;
        var updateResult = await _memberService.UpdatePassword(memberId, updatePassword);

        if (errorType is null)
        {
            Assert.That(updateResult.IsSuccess, Is.True);
            var updatedMember = await _databaseContext.Members.FindAsync(memberId);
            Assert.That(PasswordHasher.VerifyHashedPassword(updatedMember!.Password, updatePassword).IsSuccess, Is.True);
            return;
        }
        
        //fail
        Assert.That(updateResult.IsFailed, Is.True);
        Assert.That(updateResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(NotFoundError))] //wrong id
    public async Task UpdateProfileUrl(string? idString, string? profileImageIdString, Type? errorType)
    {
        var memberId = idString?.ToGuid() ?? _seededMembers.First().MemberId;
        var profileImageId = profileImageIdString?.ToGuid() ?? Guid.NewGuid();

        var updateResult = await _memberService.UpdateProfileUrl(memberId, profileImageId);
        if (errorType is null)
        {
            Assert.That(updateResult.IsSuccess, Is.True);
            var updatedMember = await _databaseContext.Members.FindAsync(memberId);
            Assert.That(updatedMember!.ProfileImageId, Is.EqualTo(profileImageId));
            return;
        }
        
        //fail
        Assert.That(updateResult.IsFailed, Is.True);
        Assert.That(updateResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null)] // success
    [TestCase("", typeof(NotFoundError))] // wrong id
    public async Task FindByMemberId(string? idString, Type? errorType)
    {
        var member = _seededMembers.First();
        var memberId = idString?.ToGuid() ?? member.MemberId;
        var findResult = await _memberService.FindByMemberId(memberId);

        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            Assert.That(findResult.Value, Is.EqualTo(member.ToMemberDto()));
            return;
        }
        
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null)] // success
    [TestCase("not_id", typeof(NotFoundError))] // wrong id
    public async Task FindByLoginId(string? id, Type? errorType)
    {
        var member = _seededMembers.First();
        id ??= member.LoginId;
        var findResult = await _memberService.FindByLoginId(id);

        if (errorType is null)
        {
            Assert.That(findResult.IsSuccess, Is.True);
            Assert.That(findResult.Value, Is.EqualTo(member.ToMemberDto()));
            return;
        }
        
        //fail
        Assert.That(findResult.IsFailed, Is.True);
        Assert.That(findResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    

}