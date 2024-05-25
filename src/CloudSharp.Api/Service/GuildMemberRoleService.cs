using CloudSharp.Api.Error;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildMemberRoleService(IGuildMemberRoleRepository _repository) : IGuildMemberRoleService
{
    public async ValueTask<Result<ulong>> AddRole(ulong guildMemberId, ulong guildRoleId)
    {
        var role = new GuildMemberRole
        {
            GuildMemberId = guildMemberId,
            GuildRoleId = guildRoleId,
        };
        var result = await _repository.AddGuildMemberRole(role);
        if (result.IsFailed)
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        return result.Value;
    }

    public async ValueTask<Result> RemoveRole(ulong guildMemberRoleId)
    {
        var result = await _repository.RemoveGuildMemberRole(guildMemberRoleId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
            return new InternalServerError().CausedBy(result.Errors);
        
        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }
}