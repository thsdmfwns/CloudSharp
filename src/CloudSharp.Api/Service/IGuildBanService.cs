using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IGuildBanService
{
    ValueTask<Result<ulong>> AddGuildBan(ulong guildId, Guid issuerMemberId, Guid bannedMemberId, string note,
        DateTimeOffset banend);

    ValueTask<Result<bool>> Exist(ulong guildId, Guid bannedMemberId);
    ValueTask<Result<GuildBanDto>> GetLatestExisted(ulong guildId, Guid bannedMemberId);

    ValueTask<Result<List<GuildBanDto>>> GetBansByGuildId(ulong guildId);
    ValueTask<Result<List<GuildBanDto>>> GetBansByIssuedMemberId(ulong guildId, Guid issuerMemberId);
    ValueTask<Result<List<GuildBanDto>>> GetBansByBannedMemberId(ulong guildId, Guid bannedMemberId);

    ValueTask<Result> UnBan(ulong guildBanId);
    ValueTask<Result> UpdateBanEnd(ulong guildBanId, DateTimeOffset banEnd);

    ValueTask<Result> DeleteBan(ulong guildBanId);
}