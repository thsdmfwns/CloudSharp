using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildRepository
{
    ValueTask<Result<GuildDto>> FindGuildById(ulong guildId);
}