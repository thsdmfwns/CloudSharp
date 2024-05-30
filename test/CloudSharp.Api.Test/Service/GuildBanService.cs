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
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace CloudSharp.Api.Test.Service;

public class GuildBanService : IAsyncDisposable
{
    private Faker _faker = null!;
    private Respawner _respawner = null!;
    private IGuildBanService _service = null!;
    private DbConnection _dbConnection = null!;
    private DatabaseContext _databaseContext = null!;
    
    private List<Member> _seededMembers = null!;
    private List<Guild> _seededGuilds = null!;
    private List<GuildBan> _seededGuildBans = null!;
    
    private Guild _rootSeededGuild = null!;
    private List<Member> _bannedMembers = null!;
    private List<Member> _notBannedMembers = null!;


        
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _faker = new Faker();
        _databaseContext = DatabaseUtil.GetDatabaseContext();
        _dbConnection = _databaseContext.Database.GetDbConnection();
        _respawner = await _dbConnection.GetRespawner();
        await _databaseContext.MigrateAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        _service = new Api.Service.GuildBanService(new GuildBanRepository(_databaseContext));
        await _respawner.ResetAsync(_dbConnection);
        _databaseContext.ChangeTracker.Clear();
        
        _seededMembers = await _databaseContext.SeedMembers(count: 10);
        _seededGuilds = await _databaseContext.SeedGuilds(count: 1);
        _rootSeededGuild = _seededGuilds.First();
        _bannedMembers = _faker.PickRandom(_seededMembers, 3).ToList();
        _notBannedMembers = _seededMembers.Where(x => !_bannedMembers.Contains(x)).ToList();
        _seededGuildBans =
            await _databaseContext.SeedGuildBans(_rootSeededGuild.GuildId, _bannedMembers, _notBannedMembers);
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DisposeAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _databaseContext.DisposeAsync();
    }

    [Test]
    [TestCase(null, null, null, null, null, null)] // success
    [TestCase(ulong.MaxValue, null, null, null, null, typeof(InternalServerError))] //invalid guild id
    [TestCase(null, "", null, null, null, typeof(InternalServerError))] //invalid member id
    [TestCase(null, null, "", null, null, typeof(InternalServerError))] //invalid member id
    [TestCase(null, null, null, null, DateTimeCase.Past, typeof(BadRequestError))] //invalid banEndDate
    public async Task AddGuildBan(ulong? guildId, string? issuerMemberIdString, string? bannedMemberIdString,
        string? note, DateTimeCase? banEndCase, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var issuerMemberId = issuerMemberIdString?.ToGuid() ?? _notBannedMembers.First().MemberId;
        var bannedMemberId = bannedMemberIdString?.ToGuid() ?? _notBannedMembers.ElementAt(1).MemberId;
        note ??= _faker.Lorem.Sentences();
        var banEnd = banEndCase?.DateTimeCaseToDateTime() ?? _faker.Date.FutureOffset();

        var result = await _service.AddGuildBan(guildId.Value, issuerMemberId, bannedMemberId, note, banEnd);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            Assert.That(_databaseContext.GuildBans.Where(x => x.GuildBanId == result.Value).Any);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    [Test]
    [TestCase(null, null, null, true)] //success
    [TestCase(ulong.MaxValue, null, null, false)] //invalid GuildID
    [TestCase(null, "", null, false)] //invalid MemberID
    public async Task Exist(ulong? guildId, string? bannedMemberIdString, Type? errorType, bool expectResult = true)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var bannedMemberId = bannedMemberIdString?.ToGuid() ?? _bannedMembers.First().MemberId;
        var result = await _service.Exist(guildId.Value, bannedMemberId);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            Assert.That(result.Value, Is.EqualTo(expectResult));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    
    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(ulong.MaxValue, null, typeof(NotFoundError))] //invalid GuildID
    [TestCase(null, "", typeof(NotFoundError))] //invalid MemberID
    public async Task GetLatest(ulong? guildId, string? bannedMemberIdString, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var bannedMemberId = bannedMemberIdString?.ToGuid() ?? _bannedMembers.First().MemberId;
        var result = await _service.GetLatestExisted(guildId.Value, bannedMemberId);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expectBan = _seededGuildBans
                .OrderBy(x => x.BanEndUnixSeconds)
                .First(x => x.GuildId == guildId && x.BannedMemberId == bannedMemberId);
            var expect = new GuildBanDto
            {
                GuildBanId = expectBan.GuildBanId,
                GuildId = expectBan.GuildId,
                IssuerMemberId = expectBan.BanIssuerMemberId,
                BannedMember = _seededMembers.Single(x => x.MemberId == expectBan.BannedMemberId).ToMemberDto(),
                IsUnbanned = expectBan.IsUnbanned,
                Note = expectBan.Note,
                BanEnd = DateTimeOffset.FromUnixTimeSeconds(expectBan.BanEndUnixSeconds),
                CreatedOn = expectBan.CreatedOn.ToLocalTime()
            };
            
            Assert.That(result.Value, Is.EqualTo(expect));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    
    [Test]
    [TestCase(null, null)] //success
    [TestCase(ulong.MaxValue, null)] //invalid id
    public async Task GetBansByGuildId(ulong? guildId, Type? errorType)
    {
        guildId ??= _rootSeededGuild.GuildId;
        var result = await _service.GetBansByGuildId(guildId.Value);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = _seededGuildBans
                .Where(x => x.GuildId == guildId.Value)
                .OrderBy(x => x.BannedMemberId)
                .Select(x => x.SeedToGuildBanDto(
                    _seededMembers.Single(y => y.MemberId == x.BannedMemberId)))
                .ToList();
            Assert.That(result.Value, Is.EqualTo(expect));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }

    
    [Test]
    [TestCase(null, null, null)] //success
    [TestCase(ulong.MaxValue, null, null)] //invalid id
    [TestCase(null, "", null)] //invalid id
    public async Task FIndBansByIssuedMemberId(ulong? guildId, string? issuerMemberIdString, Type? errorType)
    {
        var issuedMemberId = issuerMemberIdString?.ToGuid() ?? _notBannedMembers.First().MemberId;
        guildId ??= _rootSeededGuild.GuildId;
        var result = await _service.GetBansByIssuedMemberId(guildId.Value, issuedMemberId);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = _seededGuildBans
                .Where(x => x.GuildId == guildId.Value && x.BanIssuerMemberId == issuedMemberId)
                .OrderBy(x => x.BannedMemberId)
                .Select(x => x.SeedToGuildBanDto(
                    _seededMembers.Single(y => y.MemberId == x.BannedMemberId)))
                .ToList();
            Assert.That(result.Value, Is.EqualTo(expect));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed);
        Assert.That(result.HasError(x => x.GetType() == errorType));
    }
    
    
}