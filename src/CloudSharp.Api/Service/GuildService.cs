using CloudSharp.Api.Error;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;
using Mysqlx;

namespace CloudSharp.Api.Service;

public class GuildService(IGuildRepository guildRepository, ILogger<IGuildService> logger) : IGuildService
{
    public async ValueTask<Result<ulong>> CreateGuild(string guildName, Guid? guildProfileId)
    {
        var guild = new Guild
        {
            GuildName = guildName,
            GuildProfileImageId = guildProfileId,
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

    public async ValueTask<Result> UpdateGuildName(ulong guildId, string guildName)
    {
        var result = await guildRepository.UpdateGuildProperty(guildId, x => x.GuildName = guildName);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        
        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }

    public async ValueTask<Result> UpdateGuildProfileImage(ulong guildId, Guid? profileId)
    {
        
        var result = await guildRepository.UpdateGuildProperty(guildId, x => x.GuildProfileImageId = profileId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        
        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }

    public async ValueTask<Result> DeleteGuild(ulong guildId)
    {
        var result = await guildRepository.DeleteGuild(guildId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
            return new InternalServerError().CausedBy(result.Errors);
        
        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }
}