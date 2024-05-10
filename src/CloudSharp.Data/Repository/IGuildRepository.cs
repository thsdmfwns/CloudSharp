using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildRepository
{
    ValueTask<Result<ulong>> InsertGuild(Guild guild);
    ValueTask<Result<GuildDto>> FindGuildById(ulong guildId);
}