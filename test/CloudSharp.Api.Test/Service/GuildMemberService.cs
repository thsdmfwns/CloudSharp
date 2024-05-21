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

public class GuildMemberService : IAsyncDisposable
{
    private Faker _faker = null!;
    private Respawner _respawner = null!;
    private IGuildMemberService _guildMemberService = null!;
    private DbConnection _dbConnection = null!;
    private DatabaseContext _databaseContext = null!;
    private List<Member> _seededMembers = null!;
    private List<Guild> _seededGuilds = null!;
    private List<GuildChannel> _seededGuildChannels = null!;
    private List<GuildChannelRole> _seededGuildChannelRoles = null!;
    private List<GuildMember> _seededGuildMembers = null!;
    private List<GuildMemberRole> _seededGuildMemberRoles = null!;
    private List<GuildRole> _seededGuildRoles = null!;

    private Guild _rootSeededGuild = null!;
    private GuildChannel _rootSeededGuildChannel = null!;
    private GuildMember _rootSeededGuildMember = null!;
    private GuildRole _rootSeededGuildRole = null!;

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
        _guildMemberService = new Api.Service.GuildMemberService(new GuildMemberRepository(_databaseContext));
        await _respawner.ResetAsync(_dbConnection);
        _databaseContext.ChangeTracker.Clear();
        
        _seededMembers = await _databaseContext.SeedMembers(count: 10);
        _seededGuilds = await _databaseContext.SeedGuilds(count: 1);
        _rootSeededGuild = _seededGuilds.First();
        _seededGuildRoles = await _databaseContext.SeedGuildRoles(_rootSeededGuild);
        _seededGuildChannels = await _databaseContext.SeedGuildChannels(_rootSeededGuild);
        _rootSeededGuildChannel = _seededGuildChannels.First();
        _rootSeededGuildRole = _seededGuildRoles.First();
        _seededGuildChannelRoles =
            await _databaseContext.SeedGuildChannelRoles(_rootSeededGuildChannel, _rootSeededGuildRole);
        _seededGuildMembers = await _databaseContext.SeedGuildMembers(_rootSeededGuild, _seededMembers);
        _rootSeededGuildMember = _seededGuildMembers.First();
        _seededGuildMemberRoles =
            await _databaseContext.SeedGuildMemberRoles(_rootSeededGuildMember, _rootSeededGuildRole);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _databaseContext.DisposeAsync();
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase("", null, typeof(InternalServerError))] //invalid Member Id
    [TestCase(null, ulong.MaxValue, typeof(InternalServerError))] //invalid Guild Id
    public async Task AddGuildMember(string? memberIdString, ulong? guildId, Type? errorType)
    {
        var memberId = memberIdString?.ToGuid() ?? _seededMembers.First().MemberId;
        var member = _seededMembers.FirstOrDefault(x => x.MemberId == memberId);
        guildId ??= _seededGuilds.First().GuildId;

        var result = await _guildMemberService.AddGuildMember(memberId, guildId.Value, member?.Nickname ?? "");
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.GuildMembers.FindAsync(result.Value);
            Assert.That(actual is not null);
            Assert.Multiple(() =>
            {
                Assert.That(actual!.MemberId, Is.EqualTo(memberId));
                Assert.That(actual.GuildId, Is.EqualTo(guildId));
            });
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }
}