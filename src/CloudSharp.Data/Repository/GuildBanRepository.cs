using CloudSharp.Data.Entities;
using CloudSharp.Data.Exception;
using CloudSharp.Data.Query;
using CloudSharp.Data.Util;
using CloudSharp.Share.DTO;
using FluentResults;
using Microsoft.EntityFrameworkCore;

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

    public async ValueTask<Result<bool>> Exist(ulong guildId, Guid memberId)
    {
        var findResult = await Result.Try(() => 
                databaseContext.GuildBans
                .AnyAsync(x => x.GuildId == guildId
                               && x.BannedMemberId == memberId
                               && x.BanEnd > DateTimeOffset.Now),
            ex => new ExceptionalError("fail to find existed ban by exception", ex));
        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        return findResult.Value;
    }

    public async ValueTask<Result<GuildBanDto>> FindLatest(ulong guildId, Guid memberId)
    {
        var connectionString = databaseContext.Database.GetConnectionString();
        if (connectionString is null)
        {
            return new ExceptionalError(new DatabaseConnectionNotFoundException(
                "Can not find Connection string at FindGuildMemberByGuildMemberId", Environment.StackTrace));
        }

        var query = new FindLatestGuildBanByIds
        {
            GuildId = guildId,
            BannedMemberId = memberId,
            DbConnectionString =connectionString 
        };
        var result = await Result.Try(() => query.Query(),
            ex => new ExceptionalError("fail to find latest guild ban by Exception", ex));
            
        return result.IsFailed ? Result.Fail(result.Errors) : result.Value;
    }
}