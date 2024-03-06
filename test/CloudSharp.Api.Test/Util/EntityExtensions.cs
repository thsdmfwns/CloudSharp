using Bogus;
using CloudSharp.Api.Util;
using CloudSharp.Data.EntityFramework.Entities;

namespace CloudSharp.Api.Test.Util;

public static class EntityExtensions
{
    public static Faker<Member> SetRules(this Faker<Member> faker, ulong memberRoleId, string password = "password")
    {
        faker
            .RuleFor(p => p.MemberId, f => Guid.NewGuid())
            .RuleFor(p => p.LoginId, f => f.Name.FirstName())
            .RuleFor(p => p.Password, f => PasswordHasher.HashPassword(password))
            .RuleFor(p => p.RoleId, memberRoleId)
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Nickname, f => f.Company.CompanyName())
            .RuleFor(p => p.ProfileImageId, f => Guid.NewGuid());
        return faker;
    } 
}