using CloudSharp.Api.Error;
using CloudSharp.Api.Util;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildMemberService(IGuildMemberRepository _guildMemberRepository) : IGuildMemberService
{
    public async ValueTask<Result<ulong>> AddGuildMember(Guid memberId, ulong guildId, string memberName)
    {
        var guildMember = new GuildMember
        {
            MemberName = memberName,
            GuildId = guildId,
            MemberId = memberId,
            CreatedOn = DateTime.Now,
        };
        
        var result = await _guildMemberRepository.AddGuildMember(guildMember);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        if (result.IsFailed)
        {
            return new NotFoundError().CausedBy(result.Errors);
        }

        return result.Value;
    }

    public async ValueTask<Result<GuildMemberDto>> GetGuildMember(ulong guildMemberId)
    {
        var result = await _guildMemberRepository.FindGuildMemberByGuildMemberId(guildMemberId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }
        if (result.IsFailed)
        {
            return new NotFoundError().CausedBy(result.Errors);
        }

        return result.Value;
    }

    public async ValueTask<Result> BanGuildMember(ulong guildMemberId)
    {
        var result = await _guildMemberRepository.UpdateGuildMember(guildMemberId, x => x.IsBanned = true);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }

        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }

    public async ValueTask<Result> UpdateGuildMemberName(ulong guildMemberId, string guildMemberName)
    {
        if (!guildMemberName.IsMemberName())
        {
            return new BadRequestError().CausedBy("Bad name");
        }
        var result = await _guildMemberRepository.UpdateGuildMember(guildMemberId, x => x.MemberName = guildMemberName);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }

        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
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