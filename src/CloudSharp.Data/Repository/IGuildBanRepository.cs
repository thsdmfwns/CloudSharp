using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildBanRepository
{
    ValueTask<Result<ulong>> InsertGuildBan(GuildBan guildBan);

    ValueTask<Result<bool>> Exist(ulong guildId, Guid memberId);
    ValueTask<Result<GuildBan>> FindLatestExisted(ulong guildId, Guid memberId);
    ValueTask<Result<List<GuildBan>>> FindBansByGuildId(ulong guildId);
    ValueTask<Result<List<GuildBan>>> FindBansByIssuedMemberId(ulong guildId, Guid issuedMemberId);
    ValueTask<Result<List<GuildBan>>> FindBansByBannedMemberId(ulong guildId, Guid bannedMemberId);
    ValueTask<Result> UpdateGuildBan(ulong guildBanId, Action<GuildBan> updateAction);

}