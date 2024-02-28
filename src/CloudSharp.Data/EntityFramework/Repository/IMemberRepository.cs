using CloudSharp.Data.EntityFramework.Entities;
using FluentResults;

namespace CloudSharp.Data.EntityFramework.Repository;

public interface IMemberRepository
{
    public ValueTask<Result<Member>> FindByLoginId(string id);
    public ValueTask<Result> UpdateLastAccessed(Guid id);
    public ValueTask<Result<Member>> InsertMember(Member member);
    public ValueTask<Result<Member>> FindByMemberId(Guid idx);
    public ValueTask<Result> UpdateRole(Guid id, MemberRole role);
    public ValueTask<Result> UpdateEmail(Guid id, string email);
    public ValueTask<Result> UpdatePassword(Guid id, string passwordHash);
    public ValueTask<Result> UpdateNickname(Guid id, string nickname);
    public ValueTask<Result> UpdateProfileId(Guid id, Guid profileId);
    public ValueTask<Result> DeleteMember(Guid id);
}