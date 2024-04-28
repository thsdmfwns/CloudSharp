using CloudSharp.Data.EntityFramework;
using CloudSharp.Data.Query;
using CloudSharp.Share.DTO;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Repository;

public class GuildRepository(DatabaseContext databaseContext) : IGuildRepository
{
    public async ValueTask<Result<GuildDto>> FindGuildById(ulong guildId)
    {
        var connectionString = databaseContext.Database.GetConnectionString();
        if (connectionString is null)
        {
            return new Error("connectionString not found");
        }
        var query = new FIndGuildByIdQuery
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
}