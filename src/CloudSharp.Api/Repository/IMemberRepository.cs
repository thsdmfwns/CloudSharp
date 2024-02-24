using CloudSharp.Api.Entities;
using FluentResults;

namespace CloudSharp.Api.Repository;

public interface IMemberRepository
{
    public ValueTask<Result<Member>> FindByLoginId(string id);
    public ValueTask<Result<Member>> InsertMember(Guid memberId, string loginId, string hashedPassword, ulong role, string email, string nickname,
        string? profileUrl);
    public ValueTask<Result<Member>> FindByMemberId(Guid idx);
    public ValueTask<Result<Member>> UpdateRole(Guid id, ulong roleId);
    public ValueTask<Result<Member>> UpdateEmail(Guid id, string email);
    public ValueTask<Result<Member>> UpdatePassword(Guid id, string passwordHash);
    public ValueTask<Result<Member>> UpdateNickname(Guid id, string nickname);
    public ValueTask<Result<Member>> UpdateProfileId(Guid id, Guid profileId);
    public ValueTask<Result> DeleteMember(Guid id);
}