using CloudSharp.Data.Entities;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildBanRepository
{
    ValueTask<Result<ulong>> InsertGuildBan(GuildBan guildBan);
}