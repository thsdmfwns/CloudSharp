using CloudSharp.Api.Model;
using FluentResults;

namespace CloudSharp.Api.Repository;

public interface IMemberRepository
{
    public ValueTask<Result<Member>> FindById(string id);

    //return inserted member's idx
    public ValueTask<Result<ulong>> InsertMember(string id, string hashedPassword, ulong role, string email, string nickname,
        string? profileUrl);

    public ValueTask<Result<Member>> FindByIdx(ulong idx);
}