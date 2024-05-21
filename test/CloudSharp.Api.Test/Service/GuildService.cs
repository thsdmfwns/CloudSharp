using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Api.Util;
using CloudSharp.Data;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using CloudSharp.Share.DTO.EqualityComparer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class GuildService : IDisposable
{
    private Faker _faker = null!;
    private Respawner _respawner = null!;
    private IGuildService _guildService = null!;
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
    public void OneTimeTearDown()
    {
        Dispose();
    }

    [SetUp]
    public async Task SetUp()
    {
        _guildService = new Api.Service.GuildService(new GuildRepository(_databaseContext),
            NullLogger<Api.Service.GuildService>.Instance);
        await _respawner.ResetAsync(_dbConnection);
        _databaseContext.ChangeTracker.Clear();
        
        _seededMembers = await _databaseContext.SeedMembers(count: 10);
        _seededGuilds = await _databaseContext.SeedGuilds(count: 1);
        _rootSeededGuild = _seededGuilds.First();
        _seededGuildRoles = await _databaseContext.SeedGuildRoles(_rootSeededGuild);
        _seededGuildChannels = await _databaseContext.SeedGuildChannels(_rootSeededGuild);
        _seededGuildChannelRoles =
            await _databaseContext.SeedGuildChannelRoles(_seededGuildChannels, _seededGuildRoles);
        _seededGuildMembers = await _databaseContext.SeedGuildMembers(_rootSeededGuild, _seededMembers);
        _seededGuildMemberRoles =
            await _databaseContext.SeedGuildMemberRoles(_seededGuildMembers, _seededGuildRoles);
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
        _databaseContext.Dispose();
    }

    [Test]
    [TestCase(null, null)] //success
    [TestCase(ulong.MaxValue, typeof(NotFoundError))] //invalid id
    public async Task GetGuild(ulong? guildId, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var result = await _guildService.GetGuild(guildId.Value);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = new GuildDto
            {
                GuildId = _rootSeededGuild.GuildId,
                GuildName = _rootSeededGuild.GuildName,
                GuildProfileId = _rootSeededGuild.GuildProfileImageId,
                CreatedOn = _rootSeededGuild.CreatedOn,
                UpdatedOn = _rootSeededGuild.UpdatedOn,
                Members = _rootSeededGuild.SeedToGuildMemberDtos(_seededGuildMembers, _seededGuildMemberRoles, _seededGuildRoles),
                Channels = _rootSeededGuild.SeedToGuildChannelDtos(_seededGuildChannels, _seededGuildChannelRoles, _seededGuildRoles),
                Roles = _rootSeededGuild.SeedToGuildRoleDtos(_seededGuildRoles)
            };
            Assert.That(result.Value, Is.EqualTo(expect).Using(new GuildDtoEqualityComparer()));
            return;
        }

        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null)] //success
    public async Task CreateGuild(string? guildName, Guid? guildProfileId, Type? errorType)
    {
        guildName ??= _faker.Name.JobTitle();

        var result = await _guildService.CreateGuild(guildName, guildProfileId);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var inserted = await _databaseContext.Guilds.FindAsync(result.Value);
            Assert.That(inserted is not null);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(ulong.MaxValue, null, typeof(NotFoundError))] //invalid idGuildChannelRoles
    public async Task UpdateGuildName(ulong? guildId, string? guildName, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        guildName ??= _faker.Name.JobTitle();

        var result = await _guildService.UpdateGuildName(guildId.Value, guildName);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var changed = await _databaseContext.Guilds.FindAsync(guildId.Value);
            Assert.That(changed is not null);
            Assert.That(changed!.GuildName, Is.EqualTo(guildName));    
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(ulong.MaxValue, null, typeof(NotFoundError))] //invalid id
    public async Task UpdateGuildProfileImage(ulong? guildId, string? profileIdString, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var profileId = profileIdString?.ToGuid() ?? Guid.NewGuid();

        var result = await _guildService.UpdateGuildProfileImage(guildId.Value, profileId);

        if (errorType is  null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.Guilds.FindAsync(guildId);
            Assert.That(actual is not null);
            Assert.That(actual!.GuildProfileImageId, Is.EqualTo(profileId));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null)] //success
    public async Task DeleteGuild(ulong? guildId, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;

        var result = await _guildService.DeleteGuild(guildId.Value);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            var actualMembers = await _databaseContext.GuildMembers
                .Where(x => x.GuildId == guildId).ToListAsync();
            var actualChannels = await _databaseContext.GuildChannels
                .Where(x => x.GuildId == guildId).ToListAsync();
            Assert.Multiple(() =>
            {
                Assert.That(actual is null);
                Assert.That(actualMembers, Is.Empty);
                Assert.That(actualChannels, Is.Empty);
            });
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

}