using CloudSharp.Data.Entities;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IMemberRepository
{
    public ValueTask<Result<Member>> FindByLoginId(string id);
    public ValueTask<Result<Member>> InsertMember(Member member);
    public ValueTask<Result<Member>> FindByMemberId(Guid idx);
    public ValueTask<Result> UpdateMember(Guid id, Action<Member> memberUpdateAction);
    public ValueTask<Result> DeleteMember(Guid id);
}