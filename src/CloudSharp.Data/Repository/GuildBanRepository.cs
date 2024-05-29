using CloudSharp.Data.Entities;
using CloudSharp.Data.Util;
using FluentResults;

namespace CloudSharp.Data.Repository;

public class GuildBanRepository(DatabaseContext databaseContext) : IGuildBanRepository
{
    public async ValueTask<Result<ulong>> InsertGuildBan(GuildBan guildBan)
    {
        await databaseContext.GuildBans.AddAsync(guildBan);
        var result = await databaseContext.SaveChangesAsyncWithResult();
        if (result.IsFailed)
        {
            return new Error("fail to InsertGuildBan").CausedBy(result.Errors);
        }

        return guildBan.GuildBanId;
    }
}