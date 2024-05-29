using CloudSharp.Share.DTO;
using Dapper;
using FluentResults;
using MySql.Data.MySqlClient;

namespace CloudSharp.Data.Query;

public class FindLatestGuildBanByIds : IQuery<GuildBanDto>
{
    public required ulong GuildId { get; init; }
    public required Guid BannedMemberId { get; init; }
    public required string DbConnectionString { get; init; }

    #region record

    private record _GuildBan(
        System.UInt64 GuildBanId,
        System.UInt64 GuildId,
        System.String Note,
        System.DateTime BanEnd,
        System.Boolean IsUnbanned,
        System.DateTime CreatedOn,
        System.Guid? IssuerMemberId,
        //BannedMember
        System.Guid MemberId,
        System.String LoginId,
        System.String Email,
        System.String Nickname,
        System.Guid? ProfileImageId
    );

    #endregion

    #region Sql

    private const string sql =
        """
        SELECT Ban.GuildBanId,
               Ban.GuildId,
               Ban.Note,
               Ban.BanEnd,
               Ban.IsUnbanned,
               Ban.CreatedOn,
               Ban.BanIssuerMemberId As IssuerMemberId,
               Member.MemberId,
               Member.LoginId,
               Member.Email,
               Member.Nickname,
               Member.ProfileImageId
        FROM cloud_sharp.GuildBans As Ban
        JOIN cloud_sharp.Members Member on Member.MemberId = Ban.BannedMemberId
        WHERE Ban.GuildId = @GuildId AND Ban.BannedMemberId = @BannedMemberId
        ORDER BY Ban.BanEnd DESC
        LIMIT 1;
        """;

    #endregion
    
    public async ValueTask<Result<GuildBanDto>> Query()
    {
        await using var conn = new MySqlConnection(DbConnectionString);
        var queryResult = await conn.QuerySingleOrDefaultAsync<_GuildBan>(sql, new { GuildId, BannedMemberId });
        if (queryResult is null)
        {
            return Result.Fail("guild ban not found");
        }
        return new GuildBanDto
        {
            GuildBanId = queryResult.GuildBanId,
            GuildId = queryResult.GuildId,
            IssuerMemberId = queryResult.IssuerMemberId,
            BannedMember = new MemberDto
            {
                MemberId = queryResult.MemberId,
                LoginId = queryResult.LoginId,
                Email = queryResult.Email,
                Nickname = queryResult.Nickname,
                ProfileImageId = queryResult.ProfileImageId?.ToString()
            },
            IsUnbanned = queryResult.IsUnbanned,
            Note = queryResult.Note,
            BanEnd = queryResult.BanEnd,
            CreatedOn = queryResult.CreatedOn
        };
    }
}