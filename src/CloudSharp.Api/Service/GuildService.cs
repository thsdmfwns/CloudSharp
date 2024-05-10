using CloudSharp.Api.Error;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildService(IGuildRepository guildRepository, ILogger<IGuildService> logger) : IGuildService
{
    public async ValueTask<Result<ulong>> CreateGuild(Guid ownerMemberId, string guildName, Guid? guildProfileId)
    {
        var guild = new Guild
        {
            GuildName = guildName,
            GuildProfileImageId = guildProfileId,
            OwnMemberId = ownerMemberId,
        };
        var result = await guildRepository.InsertGuild(guild);
        if (result.IsFailed)
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        return result.Value;
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