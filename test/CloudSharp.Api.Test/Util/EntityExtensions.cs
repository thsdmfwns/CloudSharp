using Bogus;
using CloudSharp.Api.Util;
using CloudSharp.Data;
using CloudSharp.Data.Entities;

namespace CloudSharp.Api.Test.Util;

public static class EntityExtensions
{
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
    
    public static Faker<Guild> SetGuildRules(this Faker<Guild> faker, Guid memberId, Guid? guildProfileImageId = null)
    {
        faker
            .RuleFor(p => p.GuildName, f => f.Internet.UserName())
            .RuleFor(p => p.GuildProfileImageId, guildProfileImageId)
            .RuleFor(p => p.OwnMemberId, memberId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<Guild>> SeedGuilds(this DatabaseContext databaseContext, Member member, int count = 10)
    {
        var faker = new Faker<Guild>().SetGuildRules(member.MemberId);
        var guilds = faker.Generate(count);
        await databaseContext.Guilds.AddRangeAsync(guilds);
        await databaseContext.SaveChangesAsync();
        return guilds;
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
    
    public static Faker<GuildMember> SetGuildMemberRules(this Faker<GuildMember> faker, ulong guildId, Guid memberId, bool isBanned = false)
    {
        faker
            .RuleFor(p => p.MemberName, f => f.Internet.UserName())
            .RuleFor(p => p.MemberId, memberId)
            .RuleFor(p => p.IsBanned, isBanned)
            .RuleFor(p => p.GuildId, guildId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec())
            .RuleFor(p => p.UpdatedOn, f => f.Date.Recent().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildMember>> SeedGuildMembers(this DatabaseContext databaseContext, Guild guild, Member member, bool isBanned = false, int count = 10)
    {
        var faker = new Faker<GuildMember>().SetGuildMemberRules(guild.GuildId, member.MemberId, isBanned);
        var guildMems = faker.Generate(count);
        await databaseContext.GuildMembers.AddRangeAsync(guildMems);
        await databaseContext.SaveChangesAsync();
        return guildMems;
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
    
    public static Faker<GuildChannelRole> SetGuildChannelRoleRules(this Faker<GuildChannelRole> faker, Guid guildChannelId, ulong guildRoleId)
    {
        faker
            .RuleFor(p => p.GuildChannelId, guildChannelId)
            .RuleFor(p => p.GuildRoleId, guildRoleId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildChannelRole>> SeedGuildChannelRoles(this DatabaseContext databaseContext, GuildChannel guildChannel, GuildRole guildRole, int count = 10)
    {
        var faker = new Faker<GuildChannelRole>().SetGuildChannelRoleRules(guildChannel.GuildChannelId, guildRole.GuildRoleId);
        var guildChannelRoles = faker.Generate(count);
        await databaseContext.GuildChannelRoles.AddRangeAsync(guildChannelRoles);
        await databaseContext.SaveChangesAsync();
        return guildChannelRoles;
    }
    
    public static Faker<GuildMemberRole> SetGuildMemberRoleRules(this Faker<GuildMemberRole> faker, ulong guildMemberId, ulong guildRoleId)
    {
        faker
            .RuleFor(p => p.GuildMemberId, guildMemberId)
            .RuleFor(p => p.GuildRoleId, guildRoleId)
            .RuleFor(p => p.CreatedOn, f => f.Date.Past().RemoveNanoSec());
        return faker;
    } 
    
    public static async ValueTask<List<GuildMemberRole>> SeedGuildMemberRoles(this DatabaseContext databaseContext, GuildMember guildMember, GuildRole guildRole, int count = 10)
    {
        var faker = new Faker<GuildMemberRole>().SetGuildMemberRoleRules(guildMember.GuildMemberId, guildRole.GuildRoleId);
        var guildMemberRoles = faker.Generate(count);
        await databaseContext.GuildMemberRoles.AddRangeAsync(guildMemberRoles);
        await databaseContext.SaveChangesAsync();
        return guildMemberRoles;
    }
}