using CloudSharp.Data.Entities;
using CloudSharp.Data.Util;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Repository;

public class GuildMemberRoleRepository(DatabaseContext databaseContext) : IGuildMemberRoleRepository
{
    public async ValueTask<Result<ulong>> AddGuildMemberRole(GuildMemberRole guildMemberRole)
    {
        await databaseContext.GuildMemberRoles.AddAsync(guildMemberRole);
        var result = await databaseContext.SaveChangesAsyncWithResult();
        if (result.IsFailed)
        {
            return new Error("fail to AddGuildMemberRole").CausedBy(result.Errors);
        }

        return guildMemberRole.GuildMemberRoleId;
    }

    public async ValueTask<Result> RemoveGuildMemberRole(ulong guildMemberRoleId)
    {
        var result = await Result.Try(() => databaseContext.GuildMemberRoles
            .Where(x => x.GuildMemberRoleId == guildMemberRoleId)
            .ExecuteDeleteAsync(),
            ex => new ExceptionalError("fail to RemoveGuildMemberRole by exception", ex));
        return result.IsFailed ? 
            Result.Fail(result.Errors) :
            Result.OkIf(result.Value > 0, new Error("deleted count is less than 0"));
    }
}