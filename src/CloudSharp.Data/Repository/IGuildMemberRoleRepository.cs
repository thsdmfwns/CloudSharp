using CloudSharp.Data.Entities;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildMemberRoleRepository
{
    ValueTask<Result<ulong>> AddGuildMemberRole(GuildMemberRole guildMemberRole);
    ValueTask<Result> RemoveGuildMemberRole(ulong guildMemberRoleId);
}