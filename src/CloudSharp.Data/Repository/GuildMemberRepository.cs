using CloudSharp.Data.Entities;
using FluentResults;

namespace CloudSharp.Data.Repository;

public class GuildMemberRepository(DatabaseContext databaseContext) : IGuildMemberRepository
{
    public ValueTask<Result<ulong>> AddGuildMember(GuildMember guildMember)
    {
        throw new NotImplementedException();
    }
}