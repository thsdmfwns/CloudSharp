using Bogus;
using CloudSharp.Api.Util;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Data.EntityFramework.Repository;

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
}