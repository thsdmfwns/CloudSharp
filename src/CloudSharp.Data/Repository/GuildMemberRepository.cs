using CloudSharp.Data.Entities;
using CloudSharp.Data.Util;
using FluentResults;

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
}