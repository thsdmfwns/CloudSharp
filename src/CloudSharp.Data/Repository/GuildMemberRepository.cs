using CloudSharp.Data.Entities;
using CloudSharp.Data.Exception;
using CloudSharp.Data.Query;
using CloudSharp.Data.Util;
using CloudSharp.Share.DTO;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Repository;

public class GuildMemberRepository(DatabaseContext databaseContext) : IGuildMemberRepository
{
    public async ValueTask<Result<ulong>> AddGuildMember(GuildMember guildMember)
    {
        await databaseContext.GuildMembers.AddAsync(guildMember);
        var result = await databaseContext.SaveChangesAsyncWithResult();
        if (result.IsFailed)
        {
            return new Error("fail to AddGuildMember").CausedBy(result.Errors);
        }

        return guildMember.GuildMemberId;
    }

    public async ValueTask<Result<GuildMemberDto>> FindGuildMemberByGuildMemberId(ulong guildMemberId)
    {
        var connectionString = databaseContext.Database.GetConnectionString();
        if (connectionString is null)
        {
            return new ExceptionalError(new DatabaseConnectionNotFoundException(
                "Can not find Connection string at FindGuildMemberByGuildMemberId", Environment.StackTrace));
        }

        var query = new FIndGuildMemberByGuildMemberIdQuery
        {
            DbConnectionString = connectionString,
            GuildMemberId = guildMemberId
        };

        var result = await Result.Try(() => query.Query(),
            ex => new ExceptionalError("fail to Query FIndGuildMemberByGuildMemberIdQuery by exception", ex));
        
        return result.IsFailed ? Result.Fail(result.Errors) : result.Value;
    }

    public async ValueTask<Result<bool>> IsOwnerMember(ulong guildMemberId)
    {
        var member = await databaseContext.GuildMembers.FindAsync(guildMemberId);
        if (member is null)
        {
            return Result.Fail("member not found");
        }

        return member.IsOwner;
    }

    public async ValueTask<Result> UpdateGuildMember(ulong guildMemberId, Action<GuildMember> updateAction)
    {
        var guildMember = await databaseContext.GuildMembers.FindAsync(guildMemberId);
        if (guildMember is null)
        {
            return Result.Fail("guildMember not found");
        }
        updateAction.Invoke(guildMember);
        var result = await databaseContext.SaveChangesAsyncWithResult();
        return Result.OkIf(result.IsSuccess, new Error("fail to UpdateGuildMember").CausedBy(result.Errors));
    }

    public async ValueTask<Result> ChangeOwnerMember(ulong ownerGuildMemberId, ulong destinyGuildMemberId)
    {
        var from = await databaseContext.GuildMembers.FindAsync(ownerGuildMemberId);
        var to = await databaseContext.GuildMembers.FindAsync(destinyGuildMemberId);
        if (from is null || to is null)
        {
            return Result.Fail("guild member not found");
        }

        from.IsOwner = false;
        to.IsOwner = true;
        var saveResult = await databaseContext.SaveChangesAsyncWithResult();
        
        return Result.OkIf(saveResult.IsSuccess, new Error("fail to ChangeOwnerMember").CausedBy(saveResult.Errors));
    }
}