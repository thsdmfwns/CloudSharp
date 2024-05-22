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

    public async ValueTask<Result<GuildMemberDto>> GetOwnerGuildMember(ulong guildId)
    {
        var result = await _guildMemberRepository.FindOwnerGuildMemberByGuildId(guildId);
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

    public async ValueTask<Result> ChangeGuildOwner(ulong ownerGuildMemberId, ulong destinyGuildMemberId)
    {
        var isOwnerResult = await _guildMemberRepository.IsOwnerMember(ownerGuildMemberId);
        if (isOwnerResult.IsFailed)
        {
            return new NotFoundError().CausedBy(isOwnerResult.Errors);
        }
        if (!isOwnerResult.Value)
        {
            return new BadRequestError().CausedBy("member is not owner");
        }
        
        var changeResult = await _guildMemberRepository.ChangeOwnerMember(ownerGuildMemberId, destinyGuildMemberId);
        if (changeResult.IsFailed && changeResult.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(changeResult.Errors);
        }

        return Result.OkIf(changeResult.IsSuccess, new NotFoundError().CausedBy(changeResult.Errors));
    }

    public async ValueTask<Result> DeleteGuildMember(ulong guildMemberId)
    {
        var isOwnerResult = await _guildMemberRepository.IsOwnerMember(guildMemberId);
        if (isOwnerResult.IsFailed)
        {
            return new NotFoundError().CausedBy(isOwnerResult.Errors);
        }
        if (isOwnerResult.Value)
        {
            return new BadRequestError().CausedBy("member is owner");
        }
        
        var result = await _guildMemberRepository.DeleteGuildMember(guildMemberId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
            return new InternalServerError().CausedBy(result.Errors);
        
        return Result.OkIf(result.IsSuccess, new NotFoundError().CausedBy(result.Errors));
    }
}