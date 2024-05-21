using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class GuildExtensions
{
    public static GuildRoleDto ToDto(this GuildRole entity)
    {
        return new GuildRoleDto
        {
            GuildId = entity.GuildId,
            GuildRoleId = entity.GuildRoleId,
            RoleName = entity.RoleName,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            RoleColorRed = entity.RoleColorRed,
            RoleColorBlue = entity.RoleColorBlue,
            RoleColorGreen = entity.RoleColorGreen,
            RoleColorAlpha = entity.RoleColorAlpha
        };
    }
    
    public static GuildMemberRoleDto ToDto(this GuildMemberRole entity)
    {
        return new GuildMemberRoleDto
        {
            GuildMemberRoleId = entity.GuildMemberRoleId,
            GuildMemberId = entity.GuildMemberId,
            GuildRole = entity.GuildRole.ToDto(),
            CreatedOn = entity.CreatedOn
        };
    }

    public static GuildMemberDto ToDto(this GuildMember entity)
    {
        return new GuildMemberDto
        {
            GuildId = entity.GuildId,
            GuildMemberId = entity.GuildMemberId,
            MemberId = entity.MemberId,
            IsBanned = entity.IsBanned,
            IsOwner = entity.IsOwner,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            HadRoles = entity.GuildMemberRoles.Select(x => x.ToDto()).ToList()
        };
    }

    public static GuildChannelRoleDto ToDto(this GuildChannelRole entity)
    {
        return new GuildChannelRoleDto
        {
            GuildChannelRoleId = entity.GuildChannelRoleId,
            GuildChannelId = entity.GuildChannelId,
            GuildRole = entity.GuildRole.ToDto(),
            CreatedOn = entity.CreatedOn
        };
    }

    public static GuildChannelDto ToDto(this GuildChannel entity)
    {
        return new GuildChannelDto
        {
            GuildId = entity.GuildId,
            ChannelId = entity.GuildChannelId,
            ChannelName = entity.GuildChannelName,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            ChannelRoles = entity.GuildChannelRoles.Select(x => x.ToDto()).ToList()
        };
    }

    public static GuildDto ToDto(this Guild entity)
    {
        return new GuildDto
        {
            GuildId = entity.GuildId,
            GuildName = entity.GuildName,
            GuildProfileId = entity.GuildProfileImageId,
            CreatedOn = entity.CreatedOn,
            UpdatedOn = entity.UpdatedOn,
            Members = entity.GuildMembers.Select(x => x.ToDto()).ToList(),
            Channels = entity.GuildChannels.Select(x => x.ToDto()).ToList(),
            Roles = entity.GuildRoles.Select(x => x.ToDto()).ToList()
        };
    }
}