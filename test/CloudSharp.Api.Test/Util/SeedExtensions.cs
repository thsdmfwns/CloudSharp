using CloudSharp.Api.Util;
using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Test.Util;

public static class SeedExtensions
{
    public static List<GuildMemberDto> SeedToGuildMemberDtos(
        this Guild rootSeededGuild,
        IEnumerable<GuildMember> seededGuildMembers,
        IEnumerable<GuildMemberRole> seededGuildMemberRoles,
        IEnumerable<GuildRole> seededGuildRoles)
    {
        return seededGuildMembers
            .Where(x => x.GuildId == rootSeededGuild.GuildId)
            .Select(x => new GuildMemberDto
            {
                GuildId = x.GuildId,
                GuildMemberId = x.GuildMemberId,
                MemberId = x.MemberId,
                IsBanned = x.IsBanned,
                IsOwner = x.IsOwner,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                HadRoles = seededGuildMemberRoles
                    .Where(y => x.GuildMemberId == y.GuildMemberId)
                    .Select(y => new GuildMemberRoleDto
                    {
                        GuildMemberRoleId = y.GuildMemberRoleId,
                        GuildMemberId = y.GuildMemberId,
                        GuildRole = seededGuildRoles.Single(role => role.GuildRoleId == y.GuildRoleId).ToDto(),
                        CreatedOn = y.CreatedOn
                    })
                    .ToList()
            }).ToList();
    }

    public static List<GuildChannelDto> SeedToGuildChannelDtos(
        this Guild rootSeededGuild,
        IEnumerable<GuildChannel> seededGuildChannels,
        IEnumerable<GuildChannelRole> seededGuildChannelRoles,
        IEnumerable<GuildRole> seededGuildRoles)
    {
        return seededGuildChannels
            .Where(x => x.GuildId == rootSeededGuild.GuildId)
            .Select(x => new GuildChannelDto
            {
                GuildId = x.GuildId,
                ChannelId = x.GuildChannelId,
                ChannelName = x.GuildChannelName,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                ChannelRoles = seededGuildChannelRoles
                    .Where(y => x.GuildChannelId == y.GuildChannelId)
                    .Select(y => new GuildChannelRoleDto
                    {
                        GuildChannelRoleId = y.GuildChannelRoleId,
                        GuildChannelId = y.GuildChannelId,
                        GuildRole = seededGuildRoles.Single(role => role.GuildRoleId == y.GuildRoleId).ToDto(),
                        CreatedOn = y.CreatedOn
                    })
                    .ToList(),
            }).ToList();
    }

    public static List<GuildRoleDto> SeedToGuildRoleDtos(
        this Guild rootSeededGuild, 
        IEnumerable<GuildRole> seededGuildRoles)
    {
        return seededGuildRoles
            .Where(x => x.GuildId == rootSeededGuild.GuildId)
            .Select(x => x.ToDto())
            .ToList();
    }
}
