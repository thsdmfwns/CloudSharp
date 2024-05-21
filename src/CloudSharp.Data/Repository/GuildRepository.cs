using CloudSharp.Data.Entities;
using CloudSharp.Data.Exception;
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
        await databaseContext.Guilds.AddAsync(guild);
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
            return new ExceptionalError(new DatabaseConnectionNotFoundException(
                "Can not find Connection string at FindGuildById", Environment.StackTrace));
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
        guild.UpdatedOn = DateTime.Now;
        databaseContext.Guilds.Update(guild);
        var saveResult = await databaseContext.SaveChangesAsyncWithResult();
        
        return Result.OkIf(saveResult.IsSuccess, 
            new Error("fail to update guild property").CausedBy(saveResult.Errors));
    }

    public async ValueTask<Result> DeleteGuild(ulong guildId)
    {
        await using var transaction = await databaseContext.Database.BeginTransactionAsync();
        var deleteResults = new List<Result<int>>
        {
            //guild channels
            await Result.Try(
                () => databaseContext.GuildChannels
                    .Where(x => x.GuildId == guildId).ExecuteDeleteAsync(),
                ex => new ExceptionalError("fail to delete GuildChannels by exception", ex)),
            
            //guild members
            await Result.Try(
                () => databaseContext.GuildMembers.Where(x => x.GuildId == guildId).ExecuteDeleteAsync(),
                ex => new ExceptionalError("fail to delete GuildMembers by exception", ex)),
            
            //guild
            await Result.Try(
                () => databaseContext.Guilds.Where(x => x.GuildId == guildId).ExecuteDeleteAsync(),
                ex => new ExceptionalError("fail to delete Guilds by exception", ex)),
        };
        
        var errors = deleteResults.SelectMany(x => x.Errors).ToList();
        if (errors.Count <= 0)
        {
            await transaction.CommitAsync();
            return Result.Ok();
        }
        
        //has error
        await transaction.RollbackAsync();
        return Result.Fail(errors);
    }
}