using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildService : IGuildService
{
    public ValueTask<Result> CreateGuild(string guildName, Guid? guildProfileId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<GuildDto>> GetGuild(ulong guildId)
    {
        throw new NotImplementedException();
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