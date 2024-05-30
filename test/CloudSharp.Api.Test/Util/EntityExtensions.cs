using Bogus;
using CloudSharp.Api.Util;
using CloudSharp.Data;
using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Test.Util;

public static class EntityExtensions
{
    #region Member

    public static Faker<Member> SetMemberRules(this Faker<Member> faker, string password = "password")
    {
        faker
            .RuleFor(p => p.MemberId, _ => Guid.NewGuid())
            .RuleFor(p => p.LoginId, f => f.Name.FirstName())
            .RuleFor(p => p.Password, PasswordHasher.HashPassword(password))
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Nickname, f => f.Company.CompanyName())
            .RuleFor(p => p.ProfileImageId, Guid.NewGuid());
        return faker;
    } 
    
    public static async ValueTask<List<Member>> SeedMembers(this DatabaseContext databaseContext, int count = 10)
    {
        var faker = new Faker<Member>().SetMemberRules();
        var members = faker.Generate(count);
        await databaseContext.Members.AddRangeAsync(members);
        await databaseContext.SaveChangesAsync();
        return members;
    }

    #endregion

    #region Share

    public static Faker<Data.Entities.Share> SetShareRules(
        this Faker<Data.Entities.Share> faker, Member member, string? password = null,
        DateTime? expireTime = null, string folderPath = ".")
    {
        faker
            .RuleFor(p => p.ShareId, _ => Guid.NewGuid())
            .RuleFor(p => p.MemberId, member.MemberId)
            .RuleFor(p => p.FilePath,
                f => folderPath == "." || string.IsNullOrEmpty(folderPath)
                    ? f.System.FileName()
                    : Path.Combine(folderPath, f.System.FileName()))
            .RuleFor(p => p.Password, password is not null ? PasswordHasher.HashPassword(password) : null)
            .RuleFor(p => p.ExpireTime, expireTime ?? DateTime.MaxValue)
            .RuleFor(p => p.CreatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    }

    public static async ValueTask<List<Data.Entities.Share>> SeedShares(this DatabaseContext databaseContext, Member member, string? password = null, DateTime? expireTime = null, int count = 10, string folderPath = ".")
    {
        var faker = new Faker<Data.Entities.Share>()
            .SetShareRules(member, password, expireTime, folderPath);
        var shares = faker.Generate(count);
        await databaseContext.Shares.AddRangeAsync(shares);
        await databaseContext.SaveChangesAsync();
        return shares;
    }

    #endregion

    #region Guild

    public static GuildDto ToDto(this Guild entity)
    {
        return new GuildDto
        {
            GuildId = entity.GuildId,
            GuildName = entity.GuildName,
            GuildProfileId = entity.GuildProfileImageId,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            Members = entity.GuildMembers.Select(x => x.ToDto()).ToList(),
            Channels = entity.GuildChannels.Select(x => x.ToDto()).ToList(),
            Roles = entity.GuildRoles.Select(x => x.ToDto()).ToList()
        };
    }
    
    public static Faker<Guild> SetGuildRules(this Faker<Guild> faker, Guid? guildProfileImageId = null)
    {
        faker
            .RuleFor(p => p.GuildName, f => f.Internet.UserName())
            .RuleFor(p => p.GuildProfileImageId, guildProfileImageId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<Guild>> SeedGuilds(this DatabaseContext databaseContext, int count = 10)
    {
        var faker = new Faker<Guild>().SetGuildRules();
        var guilds = faker.Generate(count);
        await databaseContext.Guilds.AddRangeAsync(guilds);
        await databaseContext.SaveChangesAsync();
        return guilds;
    }

    #endregion

    #region GuildChannel

    public static GuildChannelDto ToDto(this GuildChannel entity)
    {
        return new GuildChannelDto
        {
            GuildId = entity.GuildId,
            ChannelId = entity.GuildChannelId,
            ChannelName = entity.GuildChannelName,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            ChannelRoles = entity.GuildChannelRoles.Select(x => x.ToDto()).ToList()
        };
    }

    
    public static Faker<GuildChannel> SetGuildChannelRules(this Faker<GuildChannel> faker, ulong guildId)
    {
        faker
            .RuleFor(p => p.GuildChannelId, _ => Guid.NewGuid())
            .RuleFor(p => p.GuildChannelName, f => f.Internet.UserName())
            .RuleFor(p => p.GuildId, guildId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildChannel>> SeedGuildChannels(this DatabaseContext databaseContext, Guild guild, int count = 10)
    {
        var faker = new Faker<GuildChannel>().SetGuildChannelRules(guild.GuildId);
        var guildChs = faker.Generate(count);
        await databaseContext.GuildChannels.AddRangeAsync(guildChs);
        await databaseContext.SaveChangesAsync();
        return guildChs;
    }

    #endregion

    #region GuildMember

    public static GuildMemberDto ToDto(this GuildMember entity)
    {
        return new GuildMemberDto
        {
            GuildId = entity.GuildId,
            GuildMemberId = entity.GuildMemberId,
            MemberId = entity.MemberId,
            IsBanned = entity.IsBanned,
            IsOwner = entity.IsOwner,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            HadRoles = entity.GuildMemberRoles.Select(x => x.ToDto()).ToList()
        };
    }
    
    public static Faker<GuildMember> SetGuildMemberRules(this Faker<GuildMember> faker, ulong guildId, Guid memberId)
    {
        faker
            .RuleFor(p => p.MemberName, f => f.Internet.UserName())
            .RuleFor(p => p.MemberId, memberId)
            .RuleFor(p => p.IsBanned, false)
            .RuleFor(p => p.IsOwner, false)
            .RuleFor(p => p.GuildId, guildId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildMember>> SeedGuildMembers(this DatabaseContext databaseContext, Guild guild, IEnumerable<Member> members)
    {
        var generates = 
            members.Select(member => 
                new Faker<GuildMember>().SetGuildMemberRules(guild.GuildId, member.MemberId))
                .Select(faker => faker.Generate(1).Single())
                .ToList();
        await databaseContext.GuildMembers.AddRangeAsync(generates);
        await databaseContext.SaveChangesAsync();
        return generates;
    }

    public static async ValueTask<GuildMember> UpdateGuildMember(this GuildMember guildMember,
        DatabaseContext databaseContext, Action<GuildMember> updateAction)
    {
        updateAction.Invoke(guildMember);
        databaseContext.GuildMembers.Update(guildMember);
        await databaseContext.SaveChangesAsync();
        return guildMember;
    }

    #endregion

    #region GuildRole

    public static GuildRoleDto ToDto(this GuildRole entity)
    {
        return new GuildRoleDto
        {
            GuildId = entity.GuildId,
            GuildRoleId = entity.GuildRoleId,
            RoleName = entity.RoleName,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            RoleColorRed = entity.RoleColorRed,
            RoleColorBlue = entity.RoleColorBlue,
            RoleColorGreen = entity.RoleColorGreen,
            RoleColorAlpha = entity.RoleColorAlpha
        };
    }
    
    public static Faker<GuildRole> SetGuildRoleRules(this Faker<GuildRole> faker, ulong guildId)
    {
        faker
            .RuleFor(p => p.RoleName, f => f.Internet.UserName())
            .RuleFor(p => p.RoleColorRed, f => f.Random.UInt())
            .RuleFor(p => p.RoleColorBlue, f => f.Random.UInt())
            .RuleFor(p => p.RoleColorGreen, f => f.Random.UInt())
            .RuleFor(p => p.GuildId, guildId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildRole>> SeedGuildRoles(this DatabaseContext databaseContext, Guild guild, int count = 10)
    {
        var faker = new Faker<GuildRole>().SetGuildRoleRules(guild.GuildId);
        var guildRoles = faker.Generate(count);
        await databaseContext.GuildRoles.AddRangeAsync(guildRoles);
        await databaseContext.SaveChangesAsync();
        return guildRoles;
    }

    #endregion

    #region GuildChannelRole

    public static GuildChannelRoleDto ToDto(this GuildChannelRole entity)
    {
        return new GuildChannelRoleDto
        {
            GuildChannelRoleId = entity.GuildChannelRoleId,
            GuildChannelId = entity.GuildChannelId,
            GuildRole = entity.GuildRole.ToDto(),
            CreatedOn = entity.CreatedOn
        };
    }
    
    public static Faker<GuildChannelRole> SetGuildChannelRoleRules(this Faker<GuildChannelRole> faker, Guid guildChannelId, ulong guildRoleId)
    {
        faker
            .RuleFor(p => p.GuildChannelId, guildChannelId)
            .RuleFor(p => p.GuildRoleId, guildRoleId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildChannelRole>> SeedGuildChannelRoles(this DatabaseContext databaseContext,
        IEnumerable<GuildChannel> guildChannels, IEnumerable<GuildRole> guildRoles, int count = 10)
    {
        var generated = 
            (guildChannels.SelectMany(guildChannel => guildRoles,
                (guildChannel, guildRole) =>
                    new Faker<GuildChannelRole>()
                        .SetGuildChannelRoleRules(guildChannel.GuildChannelId, guildRole.GuildRoleId))
            .Select(faker => faker.Generate(1).Single())).ToList();

        await databaseContext.GuildChannelRoles.AddRangeAsync(generated);
        await databaseContext.SaveChangesAsync();
        return generated;
    }

    #endregion

    #region GuildMemberRole

    public static GuildMemberRoleDto ToDto(this GuildMemberRole entity)
    {
        return new GuildMemberRoleDto
        {
            GuildMemberRoleId = entity.GuildMemberRoleId,
            GuildMemberId = entity.GuildMemberId,
            GuildRole = entity.GuildRole.ToDto(),
            CreatedOn = entity.CreatedOn
        };
    }
    
    public static Faker<GuildMemberRole> SetGuildMemberRoleRules(this Faker<GuildMemberRole> faker, ulong guildMemberId, IEnumerable<ulong> guildRoleIds)
    {
        faker
            .RuleFor(p => p.GuildMemberId, guildMemberId)
            .RuleFor(p => p.GuildRoleId, f => f.PickRandom(guildRoleIds))
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec());
        return faker;
    }

    public static async ValueTask<List<GuildMemberRole>> SeedGuildMemberRoles(this DatabaseContext databaseContext,
        IEnumerable<GuildMember> guildMembers, IEnumerable<GuildRole> guildRoles, int count = 10)
    {
        var roleIds = guildRoles.Select(x => x.GuildRoleId).ToList();
        var generated = guildMembers
            .Select(member => new Faker<GuildMemberRole>().SetGuildMemberRoleRules(member.GuildMemberId, roleIds))
            .Select(f => f.Generate(1).Single())
            .ToList();
        await databaseContext.GuildMemberRoles.AddRangeAsync(generated);
        await databaseContext.SaveChangesAsync();
        return generated;
    }
    
    #endregion

    #region GuildBan

    public static Faker<GuildBan> SetGuildBanRules(this Faker<GuildBan> faker, ulong guildId, Guid bannedMemberId, IEnumerable<Guid> issuerMemberIds)
    {
        faker
            .RuleFor(p => p.GuildId, guildId)
            .RuleFor(p => p.BanIssuerMemberId, f => f.PickRandom(issuerMemberIds))
            .RuleFor(p => p.BannedMemberId, bannedMemberId)
            .RuleFor(p => p.Note, f => f.Lorem.Sentence())
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.BanEndUnixSeconds, f => f.Date.FutureOffset().ToUnixTimeSeconds());
        return faker;
    }

    public static async ValueTask<List<GuildBan>> SeedGuildBans(
        this DatabaseContext databaseContext,
        ulong guildId,
        IEnumerable<Member> bannedMembers,
        IEnumerable<Member> issuerMembers)
    {
        var issuerMemberIds = issuerMembers.Select(x => x.MemberId).ToList();
        var bannedMemberIds = bannedMembers.Select(x => x.MemberId).ToList();
        var generated = bannedMemberIds
            .Select(id => new Faker<GuildBan>().SetGuildBanRules(guildId, id, issuerMemberIds))
            .Select(f => f.Generate(1).Single())
            .ToList();
        await databaseContext.GuildBans.AddRangeAsync(generated);
        await databaseContext.SaveChangesAsync();
        return generated;
    }

    #endregion
}