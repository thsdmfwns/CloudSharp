using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class GuildMemberRoleService : IAsyncDisposable
{
    private Faker _faker = null!;
    private Respawner _respawner = null!;
    private IGuildMemberRoleService _guildMemberRoleService = null!;
    private DbConnection _dbConnection = null!;
    private DatabaseContext _databaseContext = null!;
    private List<Member> _seededMembers = null!;
    private List<Guild> _seededGuilds = null!;
    private List<GuildMember> _seededGuildMembers = null!;
    private List<GuildMemberRole> _seededGuildMemberRoles = null!;
    private List<GuildRole> _seededGuildRoles = null!;

    private Guild _rootSeededGuild = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _faker = new Faker();
        _databaseContext = DatabaseUtil.GetDatabaseContext();
        _dbConnection = _databaseContext.Database.GetDbConnection();
        _respawner = await _dbConnection.GetRespawner();
        await _databaseContext.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DisposeAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        _guildMemberRoleService = new Api.Service.GuildMemberRoleService(new GuildMemberRoleRepository(_databaseContext));
        await _respawner.ResetAsync(_dbConnection);
        _databaseContext.ChangeTracker.Clear();
        
        _seededMembers = await _databaseContext.SeedMembers(count: 10);
        _seededGuilds = await _databaseContext.SeedGuilds(count: 1);
        _rootSeededGuild = _seededGuilds.First();
        _seededGuildRoles = await _databaseContext.SeedGuildRoles(_rootSeededGuild);
        _seededGuildMembers = await _databaseContext.SeedGuildMembers(_rootSeededGuild, _seededMembers);
        _seededGuildMemberRoles =
            await _databaseContext.SeedGuildMemberRoles(_seededGuildMembers, _seededGuildRoles);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _databaseContext.DisposeAsync();
    }


    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(ulong.MaxValue, null, typeof(InternalServerError))] // invalid id
    [TestCase(null, ulong.MaxValue, typeof(InternalServerError))] // invalid id
    public async Task AddRole(ulong? guildMemberId, ulong? guildRoleId, Type? errorType)
    {
        guildMemberId ??= _seededGuildMembers.First().GuildMemberId;
        guildRoleId ??= _seededGuildRoles.First().GuildRoleId;
        var result = await _guildMemberRoleService.AddRole(guildMemberId.Value, guildRoleId.Value);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.GuildMemberRoles.FindAsync(result.Value);
            Assert.That(actual is not null);
            Assert.That(actual!.GuildMemberId, Is.EqualTo(guildMemberId));
            Assert.That(actual.GuildRoleId, Is.EqualTo(guildRoleId));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null)] //success
    [TestCase(ulong.MaxValue, typeof(NotFoundError))] //invalid id
    public async Task RemoveRole(ulong? guildMemberRoleId, Type? errorType)
    {
        guildMemberRoleId ??= _seededGuildMemberRoles.First().GuildMemberRoleId;

        var result = await _guildMemberRoleService.RemoveRole(guildMemberRoleId.Value);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual =
                await _databaseContext.GuildMemberRoles.SingleOrDefaultAsync(x => x.GuildMemberRoleId == guildMemberRoleId);
            Assert.That(actual is null);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }
}