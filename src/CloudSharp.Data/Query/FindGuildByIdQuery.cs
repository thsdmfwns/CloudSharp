using CloudSharp.Share.DTO;
using Dapper;
using FluentResults;
using MySql.Data.MySqlClient;

namespace CloudSharp.Data.Query;

public class FindGuildByIdQuery : IQuery<GuildDto>
{
    public required string DbConnectionString { get; init; }
    public required ulong GuildId { get; init; }
    
    #region records
    // ReSharper disable ClassNeverInstantiated.Local
    // ReSharper disable InconsistentNaming
    private record _Guild(
        ulong GuildId,
        string GuildName,
        Guid? GuildProfileImageId,
        DateTime CreatedOn,
        DateTime? UpdatedOn
    );

    private record _GuildMember(
        ulong GuildId,
        ulong GuildMemberId,
        Guid MemberId,
        bool IsBanned,
        bool IsOwner,
        DateTime CreatedOn,
        DateTime? UpdatedOn
    );

    private record _GuildMemberRole(
        ulong GuildMemberRoleId,
        ulong GuildMemberId,
        ulong GuildRoleId,
        DateTime CreatedOn
    );

    private record _GuildChannel(
        ulong GuildId,
        Guid GuildChannelId,
        string GuildChannelName,
        DateTime CreatedOn,
        DateTime? UpdatedOn
    );

    private record _GuildChannelRole(
        ulong GuildChannelRoleId,
        Guid GuildChannelId,
        ulong GuildRoleId,
        DateTime CreatedOn
    );

    private record _GuildRole(
        ulong GuildId,
        ulong GuildRoleId,
        string RoleName,
        DateTime? UpdatedOn,
        DateTime CreatedOn,
        uint RoleColorRed,
        uint RoleColorGreen,
        uint RoleColorBlue,
        uint RoleColorAlpha
    );

    #endregion

    #region SQL

    private const string Sql =
        """
        SELECT Guild.GuildId, 
               Guild.GuildName,
               Guild.GuildProfileImageId,
               Guild.CreatedOn,
               Guild.UpdatedOn
        From cloud_sharp.Guilds AS Guild
        WHERE Guild.GuildId = @GuildId;

        SELECT GuildMember.GuildId,
               GuildMember.GuildMemberId,
               GuildMember.MemberId,
               GuildMember.IsBanned,
               GuildMember.IsOwner,
               GuildMember.CreatedOn,
               GuildMember.UpdatedOn
        From cloud_sharp.GuildMembers AS GuildMember
        WHERE GuildMember.GuildId = @GuildId;

        SELECT MemberRole.GuildMemberRoleId,
               MemberRole.GuildMemberId,
               MemberRole.GuildRoleId,
               MemberRole.CreatedOn
        From cloud_sharp.GuildMemberRoles AS MemberRole
        JOIN cloud_sharp.GuildMembers GuildMembers
            ON GuildMembers.GuildMemberId = MemberRole.GuildMemberId
        WHERE GuildMembers.GuildId = @GuildId;

        SELECT Channel.GuildId,
               Channel.GuildChannelId,
               Channel.GuildChannelName,
               Channel.CreatedOn,
               Channel.UpdatedOn
        FROM cloud_sharp.GuildChannels As Channel
        WHERE Channel.GuildId = @GuildId;

        SELECT ChannelRole.GuildChannelRoleId,
               ChannelRole.GuildChannelId,
               ChannelRole.GuildRoleId,
               ChannelRole.CreatedOn
        From cloud_sharp.GuildChannelRoles AS ChannelRole
        JOIN cloud_sharp.GuildChannels Channel
            on Channel.GuildChannelId = ChannelRole.GuildChannelId
        WHERE Channel.GuildId = @GuildId;

        SELECT Role.GuildId,
               Role.GuildRoleId,
               Role.RoleName,
               Role.UpdatedOn,
               Role.CreatedOn,
               Role.RoleColorRed,
               Role.RoleColorGreen,
               Role.RoleColorBlue,
               Role.RoleColorAlpha
        FROM cloud_sharp.GuildRoles AS Role
        WHERE Role.GuildId = @GuildId;
        """;
    
    #endregion
        
    
    public async ValueTask<Result<GuildDto>> Query()
    {
        await using var conn = new MySqlConnection(DbConnectionString);
        await using var queryResult = await conn.QueryMultipleAsync(Sql, new { GuildId });
        
        //read
        var guild = await queryResult.ReadSingleOrDefaultAsync<_Guild>();
        if (guild is null)
        {
            return new Error("guild not found");
        }
        var guildMembers = await queryResult.ReadAsync<_GuildMember>();
        var guildMemberRoles = await queryResult.ReadAsync<_GuildMemberRole>();
        var guildChannels = await queryResult.ReadAsync<_GuildChannel>();
        var guildChannelRoles = await queryResult.ReadAsync<_GuildChannelRole>();
        var guildRoles = await queryResult.ReadAsync<_GuildRole>();

        //parse
        var guildRolesDto = guildRoles
            .ToList()
            .Select(x => new GuildRoleDto
        {
            GuildId = x.GuildId,
            GuildRoleId = x.GuildRoleId,
            RoleName = x.RoleName,
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn,
            RoleColorRed = x.RoleColorRed,
            RoleColorBlue = x.RoleColorBlue,
            RoleColorGreen = x.RoleColorGreen,
            RoleColorAlpha = x.RoleColorAlpha
        }).ToList();
        var guildMemberRolesDto = guildMemberRoles
            .ToList()
            .Select(x => new GuildMemberRoleDto
            {
                GuildMemberRoleId = x.GuildMemberRoleId,
                GuildMemberId = x.GuildMemberId,
                GuildRole = guildRolesDto.Single(role => x.GuildRoleId == role.GuildRoleId),
                CreatedOn = x.CreatedOn
            }).ToList();
        var guildMembersDto = guildMembers
            .ToList()
            .Select(x => new GuildMemberDto
            {
                GuildId = x.GuildId,
                GuildMemberId = x.GuildMemberId,
                MemberId = x.MemberId,
                IsBanned = x.IsBanned,
                IsOwner = x.IsOwner,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                HadRoles = guildMemberRolesDto.Where(role => x.GuildMemberId == role.GuildMemberId).ToList()
            }).ToList();
        var guildChannelRolesDto = guildChannelRoles
            .ToList()
            .Select(x => new GuildChannelRoleDto
            {
                GuildChannelRoleId = x.GuildChannelRoleId,
                GuildChannelId = x.GuildChannelId,
                GuildRole = guildRolesDto.Single(role => x.GuildRoleId == role.GuildRoleId),
                CreatedOn = x.CreatedOn
            });
        var guildChannelsDto = guildChannels
            .ToList()
            .Select(x => new GuildChannelDto
            {
                GuildId = x.GuildId,
                ChannelId = x.GuildChannelId,
                ChannelName = x.GuildChannelName,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                ChannelRoles = guildChannelRolesDto.Where(role => x.GuildChannelId == role.GuildChannelId).ToList()
            }).ToList();
        
        return new GuildDto
        {
            GuildId = guild.GuildId,
            GuildName = guild.GuildName,
            GuildProfileId = guild.GuildProfileImageId,
            CreatedOn = guild.CreatedOn,
            UpdatedOn = guild.UpdatedOn,
            Members = guildMembersDto,
            Channels = guildChannelsDto,
            Roles = guildRolesDto
        };
    }
}