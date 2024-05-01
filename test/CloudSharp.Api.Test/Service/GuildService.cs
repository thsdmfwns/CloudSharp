using System.Data.Common;
using Bogus;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Data.Entities;
using CloudSharp.Data.EntityFramework;
using CloudSharp.Data.Repository;
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
        _seededMembers = await _databaseContext.SeedMembers(count:1);
        _seededGuilds = await _databaseContext.SeedGuilds(_seededMembers.First(), count:1);
        _seededGuildRoles = await _databaseContext.SeedGuildRoles(_seededGuilds.First());
        _seededGuildChannels = await _databaseContext.SeedGuildChannels(_seededGuilds.First());
        _seededGuildChannelRoles = await _databaseContext.SeedGuildChannelRoles(_seededGuildChannels.First(), _seededGuildRoles.First());
        _seededGuildMembers = await _databaseContext.SeedGuildMembers(_seededGuilds.First(), _seededMembers.First());
        _seededGuildMemberRoles = await _databaseContext.SeedGuildMemberRoles(_seededGuildMembers.First(), _seededGuildRoles.First());
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
        _databaseContext.Dispose();
    }
}