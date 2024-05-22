using CloudSharp.Data.Entities;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IGuildMemberRepository
{
    ValueTask<Result<ulong>> AddGuildMember(GuildMember guildMember);
    ValueTask<Result<GuildMemberDto>> FindGuildMemberByGuildMemberId(ulong guildMemberId);
    ValueTask<Result<GuildMemberDto>> FindOwnerByGuildId(ulong guildId);
    ValueTask<Result<bool>> IsOwnerMember(ulong guildMemberId);
    ValueTask<Result> UpdateGuildMember(ulong guildMemberId, Action<GuildMember> updateAction);
    ValueTask<Result> ChangeOwnerMember(ulong ownerGuildMemberId, ulong destinyGuildMemberId);
    ValueTask<Result> DeleteGuildMember(ulong guildMemberId);
}