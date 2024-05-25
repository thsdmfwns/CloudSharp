using FluentResults;

namespace CloudSharp.Api.Service;

public interface IGuildMemberRoleService
{
    public ValueTask<Result<ulong>> AddRole(ulong guildMemberId, ulong guildRoleId);
    public ValueTask<Result> RemoveRole(ulong guildMemberRoleId);
}