using System.Data.Common;
using Bogus;
using CloudSharp.Api.Error;
using CloudSharp.Api.Service;
using CloudSharp.Api.Test.Util;
using CloudSharp.Api.Util;
using CloudSharp.Data;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO.EqualityComparer;
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
    private GuildMember _SeededOwner = null!;
    private List<GuildMember> _SeededMembersWithoutOwner = null!;

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
        _seededGuildChannelRoles =
            await _databaseContext.SeedGuildChannelRoles(_seededGuildChannels, _seededGuildRoles);
        _seededGuildMembers = await _databaseContext.SeedGuildMembers(_rootSeededGuild, _seededMembers);
        _seededGuildMemberRoles =
            await _databaseContext.SeedGuildMemberRoles(_seededGuildMembers, _seededGuildRoles);

        _SeededOwner = await _faker.PickRandom(_seededGuildMembers)
            .UpdateGuildMember(_databaseContext, x => x.IsOwner = true);
        _SeededMembersWithoutOwner = _seededGuildMembers
            .Where(x => x.GuildMemberId != _SeededOwner.GuildMemberId)
            .ToList();
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

    [Test]
    [TestCase(null, null)] //success
    [TestCase(ulong.MaxValue, typeof(NotFoundError))] // invalid id
    public async Task GetGuildMember(ulong? guildMemberId, Type? errorType)
    {
        guildMemberId ??= _seededGuildMembers.First().GuildMemberId;

        var result = await _guildMemberService.GetGuildMember(guildMemberId.Value);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var expect = _rootSeededGuild
                .SeedToGuildMemberDtos(_seededGuildMembers, _seededGuildMemberRoles, _seededGuildRoles)
                .First(x => x.GuildMemberId == guildMemberId);
            
            Assert.That(result.Value, Is.EqualTo(expect).Using(new GuildMemberDtoEqualityCompare()));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null)] //success
    [TestCase(ulong.MaxValue, typeof(NotFoundError))] // invalid id
    public async Task BanGuildMember(ulong? guildMemberId, Type? errorType)
    {
        guildMemberId ??= _seededGuildMembers.First().GuildMemberId;

        var result = await _guildMemberService.BanGuildMember(guildMemberId.Value);
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.GuildMembers.FindAsync(guildMemberId.Value);
            Assert.That(actual!.IsBanned, Is.True);
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null)] // success
    [TestCase(ulong.MaxValue, null, typeof(NotFoundError))] //invalid id
    [TestCase(null, "", typeof(BadRequestError))] // invalid name
    [TestCase(null, "<script></script>", typeof(BadRequestError))] // invalid name
    public async Task UpdateGuildMemberName(ulong? guildMemberId, string? guildMemberName, Type? errorType)
    {
        guildMemberId ??= _seededGuildMembers.First().GuildMemberId;
        guildMemberName ??= _faker.Internet.UserName().ToLower();

        var result = await _guildMemberService.UpdateGuildMemberName(guildMemberId.Value, guildMemberName);

        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actual = await _databaseContext.GuildMembers.FindAsync(guildMemberId.Value);
            Assert.That(actual!.MemberName, Is.EqualTo(guildMemberName));
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }

    [Test]
    [TestCase(null, null, null, false)] //success
    [TestCase(ulong.MaxValue, null, typeof(NotFoundError), false)] //invalid id
    [TestCase(null, ulong.MaxValue, typeof(NotFoundError), false)] //invalid id
    [TestCase(null, null, typeof(BadRequestError), true)] //owner is not owner
    public async Task ChangeGuildOwner(ulong? ownerGuildMemberId, ulong? destinyGuildMemberId, Type? errorType, bool ownerIsNotOwner = false)
    {
        var seededMemberIdsWithoutOwner = _SeededMembersWithoutOwner.Select(x => x.GuildMemberId).ToList();
        
        ownerGuildMemberId ??= ownerIsNotOwner ? _faker.PickRandom(seededMemberIdsWithoutOwner) : _SeededOwner.GuildMemberId;
        destinyGuildMemberId ??= _faker.PickRandom(seededMemberIdsWithoutOwner);

        var result = await _guildMemberService.ChangeGuildOwner(ownerGuildMemberId.Value, destinyGuildMemberId.Value);
        
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actualOwner = await _databaseContext.GuildMembers.FindAsync(ownerGuildMemberId.Value);
            var actualDestiny = await _databaseContext.GuildMembers.FindAsync(destinyGuildMemberId.Value);
            Assert.Multiple(() =>
            {
                Assert.That(actualOwner!.IsOwner, Is.False);
                Assert.That(actualDestiny!.IsOwner, Is.True);
            });
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }
    
    [Test]
    [TestCase(null, null, false)] //success
    [TestCase(ulong.MaxValue, typeof(NotFoundError), false)] //invalid id
    [TestCase(null, typeof(BadRequestError), true)] //member is owner
    public async Task DeleteGuildMember(ulong? guildMemberId, Type? errorType, bool memberIsOwner = false)
    {
        var seededMemberIdsWithoutOwner = _SeededMembersWithoutOwner.Select(x => x.GuildMemberId).ToList();
        guildMemberId ??= memberIsOwner ? _SeededOwner.GuildMemberId : seededMemberIdsWithoutOwner.First();

        var result = await _guildMemberService.DeleteGuildMember(guildMemberId.Value);
        
        if (errorType is null)
        {
            Assert.That(result.IsSuccess);
            var actualMember = await _databaseContext.GuildMembers.SingleOrDefaultAsync(x => x.GuildMemberId == guildMemberId);
            var actualRoles = await _databaseContext.GuildMemberRoles
                .Where(x => x.GuildMemberId == guildMemberId).ToListAsync();
            Assert.Multiple(() =>
            {
                Assert.That(actualMember is null);
                Assert.That(actualRoles, Is.Empty);
            });
            return;
        }
        
        //fail
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.HasError(x => x.GetType() == errorType), Is.True);
    }
    
}