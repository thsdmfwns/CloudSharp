using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IGuildService
{
    ValueTask<Result<ulong>> CreateGuild(string guildName, Guid? guildProfileId);
    
    ValueTask<Result<GuildDto>> GetGuild(ulong guildId);
    
    ValueTask<Result> UpdateGuildName(ulong guildId, string guildName);
    
    ValueTask<Result> UpdateGuildProfileImage(ulong guildId, Guid? profileId);
    
    ValueTask<Result> DeleteGuild(ulong guildId);
}