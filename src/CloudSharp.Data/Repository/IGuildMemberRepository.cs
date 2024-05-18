using CloudSharp.Data.Entities;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildMemberRepository
{
    ValueTask<Result<ulong>> AddGuildMember(GuildMember guildMember);
}