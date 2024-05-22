using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IGuildMemberService
{
    ValueTask<Result<ulong>> AddGuildMember(Guid memberId, ulong guildId, string memberName);
    ValueTask<Result<GuildMemberDto>> GetGuildMember(ulong guildMemberId);
    ValueTask<Result<GuildMemberDto>> GetOwnerGuildMember(ulong guildId);
    //todo 밴의 방식 변경? 
    // 밴에 대한 테이블을 하나 놔서 bool 형식이 아닌 dateTIme 형식으로
    ValueTask<Result> BanGuildMember(ulong guildMemberId);
    ValueTask<Result> UpdateGuildMemberName(ulong guildMemberId, string guildMemberName);
    ValueTask<Result> ChangeGuildOwner(ulong ownerGuildMemberId, ulong destinyGuildMemberId);
    ValueTask<Result> DeleteGuildMember(ulong guildMemberId);
}