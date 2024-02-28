using CloudSharp.Data.EntityFramework.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Repository;

public class MemberRepository(DatabaseContext context) : IMemberRepository
{
    public async ValueTask<Result<Member>> FindByLoginId(string id)
    {
        var member = await context.Members.SingleOrDefaultAsync(x => x.LoginId == id);
        return member is null ? Result.Fail("member not found") : member;
    }

    public async ValueTask<Result> UpdateLastAccessed(Guid id)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.LastAccessed, DateTime.Now));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result<Member>> InsertMember(Member member)
    {
        await context.Members.AddAsync(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async ValueTask<Result<Member>> FindByMemberId(Guid id)
    {
        var member = await context.Members.FindAsync(id);
        return member is null ? Result.Fail("member not found") : member;
    }

    public async ValueTask<Result> UpdateRole(Guid id, MemberRole role)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.Role, role));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result> UpdateEmail(Guid id, string email)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.Email, email));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result> UpdatePassword(Guid id, string passwordHash)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.Password, passwordHash));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result> UpdateNickname(Guid id, string nickname)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.Nickname, nickname));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result> UpdateProfileId(Guid id, Guid profileId)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteUpdateAsync(x => x.
                SetProperty(member => member.ProfileImageId, profileId));
        return Result.OkIf(await changed > 0, "member not found");
    }

    public async ValueTask<Result> DeleteMember(Guid id)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteDeleteAsync();
        return Result.OkIf(await changed > 0, "member not found");
    }
}