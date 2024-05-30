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

    public async ValueTask<Result<GuildBanDto>> FindLatestExisted(ulong guildId, Guid memberId)
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
                .FirstOrDefaultAsync());
        if (findResult.IsFailed)
        {
            return Result.Fail(findResult.Errors);
        }

        if (findResult.ValueOrDefault is null)
        {
            return Result.Fail("LatestBan Not found");
        }

        return new GuildBanDto
        {
            GuildBanId = findResult.Value!.GuildBanId,
            GuildId = findResult.Value.GuildId,
            IssuerMemberId = findResult.Value.BanIssuerMemberId,
            BannedMember = new MemberDto
            {
                MemberId = findResult.Value.BannedMember.MemberId,
                LoginId = findResult.Value.BannedMember.LoginId,
                Email = findResult.Value.BannedMember.Email,
                Nickname = findResult.Value.BannedMember.Nickname,
                ProfileImageId = findResult.Value.BannedMember.ProfileImageId.ToString()
            },
            IsUnbanned = findResult.Value.IsUnbanned,
            Note = findResult.Value.Note,
            BanEnd = DateTimeOffset.FromUnixTimeSeconds(findResult.Value.BanEndUnixSeconds),
            CreatedOn = findResult.Value.CreatedOn
        };
    }
}