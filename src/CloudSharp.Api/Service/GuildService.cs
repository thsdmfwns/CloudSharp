using CloudSharp.Api.Error;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildService(IGuildRepository guildRepository, ILogger<IGuildService> logger) : IGuildService
{
    public ValueTask<Result> CreateGuild(string guildName, Guid? guildProfileId)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Result<GuildDto>> GetGuild(ulong guildId)
    {
        var result = await guildRepository.FindGuildById(guildId);
        if (result.IsFailed)
        {
            return new NotFoundError().CausedBy(result.Errors);
        }

        return result.Value;
    }

    public ValueTask<Result> UpdateGuildName(ulong guildId, string guildName)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> UpdateGuildProfileImage(ulong guildId, Guid? profileId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> DeleteGuild(ulong guildId)
    {
        throw new NotImplementedException();
    }
}