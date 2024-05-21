using CloudSharp.Share.DTO;
using Dapper;
using FluentResults;
using MySql.Data.MySqlClient;

namespace CloudSharp.Data.Query;

public class FIndGuildMemberByGuildMemberIdQuery : IQuery<GuildMemberDto>
{
    
    public required string DbConnectionString { get; init; }
    public required ulong GuildMemberId { get; init; }
    
    #region record

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
        DateTime MemberRoleCreatedOn,
        //guildRole
        ulong GuildId,
        string RoleName,
        DateTime? GuildRoleUpdatedOn,
        DateTime GuildRoleCreatedOn,
        uint RoleColorRed,
        uint RoleColorGreen,
        uint RoleColorBlue,
        uint RoleColorAlpha
    );
    
    #endregion

    #region Sql

    private const string Sql =
        """
            
        SELECT GuildMember.GuildId,
               GuildMember.GuildMemberId,
               GuildMember.MemberId,
               GuildMember.IsBanned,
               GuildMember.IsOwner,
               GuildMember.CreatedOn,
               GuildMember.UpdatedOn
        From cloud_sharp.GuildMembers AS GuildMember
        WHERE GuildMember.GuildMemberId = @GuildMemberId;

        SELECT MemberRole.GuildMemberRoleId,
               MemberRole.GuildMemberId,
               MemberRole.GuildRoleId,
               MemberRole.CreatedOn AS MemberRoleCreatedOn,
               GuildRole.GuildId,
               GuildRole.RoleName,
               GuildRole.UpdatedOn AS GuildRoleUpdatedOn,
               GuildRole.CreatedOn AS GuildRoleCreatedOn,
               GuildRole.RoleColorRed,
               GuildRole.RoleColorGreen,
               GuildRole.RoleColorBlue,
               GuildRole.RoleColorAlpha
        From cloud_sharp.GuildMemberRoles AS MemberRole
        JOIN cloud_sharp.GuildRoles GuildRole on GuildRole.GuildRoleId = MemberRole.GuildRoleId
        WHERE MemberRole.GuildMemberId = @GuildMemberId
        """;

    #endregion
    public async ValueTask<Result<GuildMemberDto>> Query()
    {
        await using var conn = new MySqlConnection(DbConnectionString);
        await using var queryResult = await conn.QueryMultipleAsync(Sql, new { GuildMemberId });
        var guildMember = await queryResult.ReadSingleOrDefaultAsync<_GuildMember>();
        if (guildMember is null)
        {
            return new Error("member not found");
        }
        var guildMemberRoles = (await queryResult.ReadAsync<_GuildMemberRole>()).ToList();

        return new GuildMemberDto
        {
            GuildId = guildMember.GuildId,
            GuildMemberId = guildMember.GuildMemberId,
            MemberId = guildMember.MemberId,
            IsBanned = guildMember.IsBanned,
            IsOwner = guildMember.IsOwner,
            CreatedOn = guildMember.CreatedOn,
            UpdatedOn = guildMember.UpdatedOn,
            HadRoles = guildMemberRoles.Select(x => new GuildMemberRoleDto
            {
                GuildMemberRoleId = x.GuildMemberRoleId,
                GuildMemberId = x.GuildMemberId,
                GuildRole = new GuildRoleDto
                {
                    GuildId = x.GuildId,
                    GuildRoleId = x.GuildRoleId,
                    RoleName = x.RoleName,
                    CreatedOn = x.GuildRoleCreatedOn,
                    UpdatedOn = x.GuildRoleUpdatedOn,
                    RoleColorRed = x.RoleColorRed,
                    RoleColorBlue = x.RoleColorBlue,
                    RoleColorGreen = x.RoleColorGreen,
                    RoleColorAlpha = x.RoleColorAlpha
                },
                CreatedOn = x.MemberRoleCreatedOn
            }).ToList()
        };
    }
}