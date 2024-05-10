using CloudSharp.Data.Entities;
using CloudSharp.Data.Query;
using CloudSharp.Data.Util;
using CloudSharp.Share.DTO;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Repository;

public class GuildRepository(DatabaseContext databaseContext) : IGuildRepository
{
    public async ValueTask<Result<ulong>> InsertGuild(Guild guild)
    {
        var result = await databaseContext.Guilds.AddAsync(guild);
        var saveResult = await databaseContext.SaveChangesAsyncWithResult();
        if (saveResult.IsFailed)
        {
            return new Error("fail to insertGuild").CausedBy(saveResult.Errors);
        }
        return guild.GuildId;
    }

    public async ValueTask<Result<GuildDto>> FindGuildById(ulong guildId)
    {
        var connectionString = databaseContext.Database.GetConnectionString();
        if (connectionString is null)
        {
            return new Error("connectionString not found");
        }
        var query = new FindGuildByIdQuery
        {
            DbConnectionString = connectionString,
            GuildId = guildId
        };
        var queryResult = await query.Query();
        if (queryResult.IsFailed)
        {
            return Result.Fail(queryResult.Errors);
        }

        return queryResult.Value;
    }

    public async ValueTask<Result> UpdateGuildProperty(ulong guildId, Action<Guild> updateAction)
    {
        var guild = await databaseContext.Guilds.FindAsync(guildId);
        if (guild is null)
        {
            return new Error("guild not found");
        }
        updateAction.Invoke(guild);
        databaseContext.Guilds.Update(guild);
        var saveResult = await databaseContext.SaveChangesAsyncWithResult();
        
        return Result.OkIf(saveResult.IsSuccess, 
            new Error("fail to update guild property").CausedBy(saveResult.Errors));
    }
}