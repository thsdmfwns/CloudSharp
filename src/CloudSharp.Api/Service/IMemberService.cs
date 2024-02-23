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

    public ValueTask<Result<MemberDto>> UpdateRole(Guid idx, ulong roleId);
    public ValueTask<Result<MemberDto>> UpdateEmail(Guid idx, string email);
    public ValueTask<Result<MemberDto>> UpdateNickname(Guid idx, string nickname);
    public ValueTask<Result<MemberDto>> UpdatePassword(Guid idx, string password);
    public ValueTask<Result<MemberDto>> UpdateProfileUrl(Guid idx, string profileUrl);

    public ValueTask<Result<MemberDto>> FindByMemberId(Guid idx);
    public ValueTask<Result<MemberDto>> FindByLoginId(string id);

    public ValueTask<Result> DeleteMember(ulong idx, string password);
}