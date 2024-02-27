using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IMemberService
{
    public ValueTask<Result<MemberDto>> Login(string id, string password);

    public ValueTask<Result<MemberDto>> Register(
        string id,
        string password,
        ulong role,
        string email,
        string nickname,
        string? profileUrl
    );

    public ValueTask<Result> UpdateRole(Guid id, ulong roleId);
    public ValueTask<Result> UpdateEmail(Guid id, string email);
    public ValueTask<Result> UpdateNickname(Guid id, string nickname);
    public ValueTask<Result> UpdatePassword(Guid id, string password, string updatePassword);
    public ValueTask<Result> UpdateProfileUrl(Guid id, Guid profileImageId);

    public ValueTask<Result<MemberDto>> FindByMemberId(Guid id);
    public ValueTask<Result<MemberDto>> FindByLoginId(string id);

    public ValueTask<Result> DeleteMember(Guid id, string password);
}