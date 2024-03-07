using CloudSharp.Data.EntityFramework.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Repository;

public class MemberRepository(DatabaseContext context) : IMemberRepository
{
    public async ValueTask<Result<Member>> FindByLoginId(string id)
    {
        var member = await context.Members
            .SingleOrDefaultAsync(x => x.LoginId == id);
        return member is null ? Result.Fail("member not found") : member;
    }

    public async ValueTask<Result<Member>> InsertMember(Member member)
    {
        await context.Members.AddAsync(member);
        await context.SaveChangesAsync();
        return member;
    }

    public async ValueTask<Result<Member>> FindByMemberId(Guid id)
    {
        var member = await context.Members
            .SingleOrDefaultAsync(x => x.MemberId == id);
        return member is null ? Result.Fail("member not found") : member;
    }

    public async ValueTask<Result> UpdateMember(Guid id, Action<Member> memberUpdateAction)
    {
        var member = await context.Members.FindAsync(id);
        if (member is null)
        {
            return Result.Fail("member not found");
        }
        memberUpdateAction.Invoke(member);
        context.Members.Update(member);
        await context.SaveChangesAsync();
        return Result.Ok();
    }

    public async ValueTask<Result> DeleteMember(Guid id)
    {
        var changed = context.Members
            .Where(x => x.MemberId == id)
            .ExecuteDeleteAsync();
        return Result.OkIf(await changed > 0, "member not found");
    }
}