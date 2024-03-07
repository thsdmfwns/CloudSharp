using System.Data.Common;
using System.Text;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Data.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Respawn;
using Respawn.Graph;

namespace CloudSharp.Api.Test.Service;

public class MemberService : IDisposable
{
    private const string _password = "password";
    private Respawner _respawner;
    private IMemberService _memberService;
    private DbConnection _dbConnection;
    private DatabaseContext _databaseContext;
    private List<Member> _seededMembers;
    private Faker<Member> _memberFaker;


    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _databaseContext = BuildDatabaseContext();
        _dbConnection = _databaseContext.Database.GetDbConnection();
        var respawnerOptions =  new RespawnerOptions
        {
            SchemasToInclude =
            [
                "cloud_sharp"
            ],
            TablesToIgnore =
            [
                "MemberRoles",
                "__EFMigrationsHistory"
            ],
            DbAdapter = DbAdapter.MySql
        };
        await _dbConnection.OpenAsync();
        
        if((await _databaseContext.Database.GetPendingMigrationsAsync()).Any()){
            await _databaseContext.Database.MigrateAsync();
        }
        _respawner = await Respawner.CreateAsync(_dbConnection, respawnerOptions);
    }

    [SetUp]
    public async Task SetUp()
    {
        _memberService = new Api.Service.MemberService(new MemberRepository(_databaseContext),
            NullLogger<Api.Service.MemberService>.Instance);
        await _respawner.ResetAsync(_dbConnection);
        _seededMembers = await SeedMembers();
    }

    private async ValueTask<List<Member>> SeedMembers()
    {
        var faker = new Faker<Member>().SetRules(_password);
        var members = faker.Generate(10);
        await _databaseContext.Members.AddRangeAsync(members);
        await _databaseContext.SaveChangesAsync();
        return members;
    }
    

    private DatabaseContext BuildDatabaseContext()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        dbContextOptionsBuilder.UseMySQL(
            "server=localhost;port=11001;database=cloud_sharp;user=root;password=q1w2e3r4",
            b => b.MigrationsAssembly("CloudSharp.Migration"));
        return new DatabaseContext(dbContextOptionsBuilder.Options);
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _databaseContext.Dispose();
    }

    [Test]
    public async Task Login()
    {
        var member = _seededMembers.First();
        var loginResult = await _memberService.Login(member.LoginId, _password);
        Assert.That(loginResult.IsSuccess, Is.True);
    }

    [Test]
    [TestCase(null, "not_password", typeof(UnauthorizedError))] // wrong password
    [TestCase("not_id", null, typeof(UnauthorizedError)) ] // wrong loginId
    public async Task Login_fail(string? loginId, string? password, Type errorType)
    {
        var member = _seededMembers.First();
        var loginResult = await _memberService.Login(loginId ?? member.LoginId, password ?? _password);
        Assert.That(loginResult.IsFailed, Is.True);
        Assert.That(loginResult.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    public async Task Register()
    {
        var faker = new Faker<Member>().SetRules();
        var member = faker.Generate(1).First();
        var registerResult = await _memberService.Register(
            member.LoginId,
            member.Password,
            member.Email,
            member.Nickname,
            member.ProfileImageId);
        Assert.That(registerResult.IsSuccess, Is.True);
        
        var insertedMemberDto = registerResult.Value;
        var insertedMember = await _databaseContext.Members.FindAsync(Guid.Parse(insertedMemberDto.MemberId));
        Assert.That(insertedMember, Is.Not.Null);
    }
    
    [Test]
    [TestCase(1, null, typeof(ConflictError))] // exist loginId
    [TestCase(null, "not_email", typeof(BadRequestError))] // bad email
    public async Task Register_fail(int? seedIndex, string? email, Type errorType)
    {
        var loginId = seedIndex is null ? null : _seededMembers[seedIndex.Value].LoginId;
        var roleId = 1ul;
        var faker = new Faker<Member>().SetRules();
        var member = faker.Generate(1).First();
        var registerResult = await _memberService.Register(
            loginId ?? member.LoginId,
            member.Password,
            email ?? member.Email,
            member.Nickname,
            member.ProfileImageId);
        
        Assert.That(registerResult.IsFailed, Is.True);
        Assert.That(registerResult.HasError(x => x.GetType() == errorType), Is.True);
    }
    

}