using Bogus;
using CloudSharp.Api.Util;
using CloudSharp.Data.Entities;
using CloudSharp.Data.EntityFramework;

namespace CloudSharp.Api.Test.Util;

public static class EntityExtensions
{
    public static Faker<Member> SetRules(this Faker<Member> faker, string password = "password")
    {
        faker
            .RuleFor(p => p.MemberId, f => Guid.NewGuid())
            .RuleFor(p => p.LoginId, f => f.Name.FirstName())
            .RuleFor(p => p.Password, f => PasswordHasher.HashPassword(password))
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Nickname, f => f.Company.CompanyName())
            .RuleFor(p => p.ProfileImageId, f => Guid.NewGuid());
        return faker;
    } 
    
    public static async ValueTask<List<Member>> SeedMembers(this DatabaseContext databaseContext, int count = 10)
    {
        var faker = new Faker<Member>().SetRules();
        var members = faker.Generate(count);
        await databaseContext.Members.AddRangeAsync(members);
        await databaseContext.SaveChangesAsync();
        return members;
    }

    public static Faker<Data.Entities.Share> SetRules(
        this Faker<Data.Entities.Share> faker, Member member, string? password = null,
        DateTime? expireTime = null, string folderPath = ".")
    {
        faker
            .RuleFor(p => p.ShareId, f => Guid.NewGuid())
            .RuleFor(p => p.MemberId, f => member.MemberId)
            .RuleFor(p => p.FilePath,
                f => folderPath == "." || string.IsNullOrEmpty(folderPath)
                    ? f.System.FileName()
                    : Path.Combine(folderPath, f.System.FileName()))
            .RuleFor(p => p.Password, p => password is not null ? PasswordHasher.HashPassword(password) : null)
            .RuleFor(p => p.ExpireTime, f => expireTime ?? DateTime.MaxValue)
            .RuleFor(p => p.CreatedOn, f => f.Date.Recent());
        return faker;
    }

    public static async ValueTask<List<Data.Entities.Share>> SeedShares(this DatabaseContext databaseContext, Member member, string? password = null, DateTime? expireTime = null, int count = 10, string folderPath = ".")
    {
        var faker = new Faker<Data.Entities.Share>()
            .SetRules(member, password, expireTime, folderPath);
        var shares = faker.Generate(count);
        await databaseContext.Shares.AddRangeAsync(shares);
        await databaseContext.SaveChangesAsync();
        return shares;
    }
}