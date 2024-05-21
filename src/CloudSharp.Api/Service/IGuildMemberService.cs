using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IGuildMemberService
{
    ValueTask<Result<ulong>> AddGuildMember(Guid memberId, ulong guildId, string memberName);
    ValueTask<GuildMemberDto> GetGuildMember(ulong guildMemberId);
    ValueTask<Result> BanGuildMember(ulong guildMemberId);
    ValueTask<Result> UpdateGuildMemberName(ulong guildMemberId, string guildMemberName);
    ValueTask<Result> ChangeGuildOwner(ulong fromGuildMemberId, ulong toGuildMemberId);
    ValueTask<Result> DeleteGuildMember(ulong guildMemberId);
}