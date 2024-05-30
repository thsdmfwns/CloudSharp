using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildBanRepository
{
    ValueTask<Result<ulong>> InsertGuildBan(GuildBan guildBan);

    ValueTask<Result<bool>> Exist(ulong guildId, Guid memberId);
    ValueTask<Result<GuildBanDto>> FindLatestExisted(ulong guildId, Guid memberId);
}