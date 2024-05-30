using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class EntityExtensions
{
    public static MemberDto ToMemberDto(this Member member)
    {
        return new MemberDto
        {
            MemberId = member.MemberId,
            LoginId = member.LoginId,
            Email = member.Email,
            Nickname = member.Nickname,
            ProfileImageId = member.ProfileImageId?.ToString(),
        };
    }

    public static ShareDto ToShareDto(this Data.Entities.Share share)
    {
        return new ShareDto
        {
            ShareId = share.ShareId,
            MemberId = share.MemberId,
            ExpireTime = new DateTimeOffset(share.ExpireTime).ToUnixTimeSeconds(),
            HasPassword = share.Password is not null,
            FileName = Path.GetFileName(share.FilePath)
        };
    }

    public static GuildBanDto ToGuildBanDto(this GuildBan guildBan)
    {
        return new GuildBanDto
        {
            GuildBanId = guildBan.GuildBanId,
            GuildId = guildBan.GuildId,
            IssuerMemberId = guildBan.BanIssuerMemberId,
            BannedMember = new MemberDto
            {
                MemberId = guildBan.BannedMember.MemberId,
                LoginId = guildBan.BannedMember.LoginId,
                Email = guildBan.BannedMember.Email,
                Nickname = guildBan.BannedMember.Nickname,
                ProfileImageId = guildBan.BannedMember.ProfileImageId.ToString()
            },
            IsUnbanned = guildBan.IsUnbanned,
            Note = guildBan.Note,
            BanEnd = DateTimeOffset.FromUnixTimeSeconds(guildBan.BanEndUnixSeconds),
            CreatedOn = guildBan.CreatedOn
        };
    }
}