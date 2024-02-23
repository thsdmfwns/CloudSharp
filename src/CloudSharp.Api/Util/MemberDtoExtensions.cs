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
            Email = member.Email,
            Nickname = member.Nickname,
            ProfileImageURL = member.ProfileImageURL,
            RoleId = member.Role.Id
        };
    }
}