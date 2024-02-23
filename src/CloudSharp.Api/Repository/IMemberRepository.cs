using CloudSharp.Api.Entities;
using FluentResults;

namespace CloudSharp.Api.Repository;

public interface IMemberRepository
{
    public ValueTask<Result<Member>> FindByLoginId(string id);

    //return inserted member's idx
    public ValueTask<Result<Member>> InsertMember(Guid memberId, string loginId, string hashedPassword, ulong role, string email, string nickname,
        string? profileUrl);

    public ValueTask<Result<Member>> FindByMemberId(Guid idx);

    public ValueTask<Result<Member>> UpdateRole(Guid id, ulong roleId);
}