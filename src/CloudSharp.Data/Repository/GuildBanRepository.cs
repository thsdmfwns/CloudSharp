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
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var findResult = await Result.Try(() => 
                databaseContext.GuildBans
                .AnyAsync(x => x.GuildId == guildId
                               && x.BannedMemberId == memberId
                               && x.BanEndUnixSeconds > now
                               && x.IsUnbanned == false),
            ex => new ExceptionalError("fail to find existed ban by exception", ex));
        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        return findResult.Value;
    }

    public async ValueTask<Result<GuildBan>> FindLatestExisted(ulong guildId, Guid memberId)
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var findResult = await Result.Try(() =>
            databaseContext.GuildBans
                .Where(x => x.GuildId == guildId
                            && x.BannedMemberId == memberId
                            && x.BanEndUnixSeconds > now
                            && x.IsUnbanned == false)
                .Include(x => x.BannedMember)
                .OrderBy(x => x.BanEndUnixSeconds)
                .FirstOrDefaultAsync(),
            ex => new ExceptionalError("fail to FindLatestExisted by exception", ex));
        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        if (findResult.ValueOrDefault is null)
        {
            return Result.Fail("LatestBan Not found");
        }

        return findResult.Value!;
    }

    public async ValueTask<Result<List<GuildBan>>> FindBansByGuildId(ulong guildId)
    {
        var findResult = await Result.Try(() =>
            databaseContext.GuildBans
                .Where(x => x.GuildId == guildId)
                .Include(x => x.BannedMember)
                .OrderBy(x => x.BannedMemberId)
                .ToListAsync(),
            ex => new ExceptionalError("fail to FindBansByGuildId by exception", ex));

        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        return findResult.Value;
    }

    public async ValueTask<Result<List<GuildBan>>> FindBansByIssuedMemberId(ulong guildId, Guid issuedMemberId)
    {
        var findResult = await Result.Try(() =>
            databaseContext.GuildBans
                .Where(x => x.GuildId == guildId && x.BanIssuerMemberId == issuedMemberId)
                .Include(x => x.BannedMember)
                .OrderBy(x => x.BannedMemberId)
                .ToListAsync(),
            ex => new ExceptionalError("fail to FIndIssuedBans by exception", ex));

        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        return findResult.Value;
    }

    public async ValueTask<Result<List<GuildBan>>> FindBansByBannedMemberId(ulong guildId, Guid bannedMemberId)
    {
        var findResult = await Result.Try(() =>
                databaseContext.GuildBans
                    .Where(x => x.GuildId == guildId && x.BannedMemberId == bannedMemberId)
                    .Include(x => x.BannedMember)
                    .OrderBy(x => x.BannedMemberId)
                    .ToListAsync(),
            ex => new ExceptionalError("fail to FIndIssuedBans by exception", ex));

        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        return findResult.Value;
    }
}