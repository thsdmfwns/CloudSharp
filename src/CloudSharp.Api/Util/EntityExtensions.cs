using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class EntityExtensions
{
    public static MemberDto ToMemberDto(this Member member)
    {
        return new MemberDto
        {
            MemberId = member.MemberId.ToString(),
            LoginId = member.LoginId,
            Email = member.Email,
            Nickname = member.Nickname,
            ProfileImageId = member.ProfileImageId?.ToString(),
        };
    }
}