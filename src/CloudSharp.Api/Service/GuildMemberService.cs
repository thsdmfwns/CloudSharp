using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildMemberService(IGuildMemberRepository _guildMemberRepository) : IGuildMemberService
{
    public ValueTask<Result<ulong>> AddGuildMember(Guid memberId, ulong guildId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<GuildMemberDto> GetGuildMember(ulong guildMemberId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> BanGuildMember(ulong guildMemberId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> UpdateGuildMemberName(ulong guildMemberId, string guildMemberName)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> ChangeGuildOwner(ulong fromGuildMemberId, ulong toGuildMemberId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> DeleteGuildMember(ulong guildMemberId)
    {
        throw new NotImplementedException();
    }
}