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

    public ValueTask<Result<MemberDto>> UpdateRole(ulong idx, ulong role);
    public ValueTask<Result<MemberDto>> UpdateEmail(ulong idx, string email);
    public ValueTask<Result<MemberDto>> UpdateNickname(ulong idx, string nickname);
    public ValueTask<Result<MemberDto>> UpdatePassword(ulong idx, string password);
    public ValueTask<Result<MemberDto>> UpdateProfileUrl(ulong idx, string profileUrl);

    public ValueTask<Result<MemberDto>> FindByIdx(ulong idx);
    public ValueTask<Result<MemberDto>> FindById(string id);
    public ValueTask<Result<MemberDto>> FindByDirectory(Guid directory);

    public ValueTask<Result> DeleteMember(ulong idx, string password);
}