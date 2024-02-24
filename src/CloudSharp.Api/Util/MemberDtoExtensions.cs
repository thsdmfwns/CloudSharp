using CloudSharp.Api.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class MemberDtoExtensions
{
    public static MemberDto ToMemberDto(this Member member)
    {
        return new MemberDto
        {
            MemberId = member.MemberId.ToString(),
            LoginId = member.LoginId,
            Email = member.Email,
            Nickname = member.Nickname,
            ProfileImageId = member.ProfileImageId.ToString(),
            RoleId = member.Role.Id
        };
    }
}