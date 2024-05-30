using CloudSharp.Api.Error;
using CloudSharp.Data.Entities;
using CloudSharp.Data.Repository;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class GuildBanService(IGuildBanRepository _repository) : IGuildBanService
{
    public async ValueTask<Result<ulong>> AddGuildBan(ulong guildId, Guid issuerMemberId, Guid bannedMemberId, string note, DateTimeOffset banEnd)
    {
        if (banEnd < DateTimeOffset.Now)
        {
            return new BadRequestError().CausedBy("banEnd is less than now");
        }

        var ban = new GuildBan
        {
            GuildId = guildId,
            BanIssuerMemberId = issuerMemberId,
            BannedMemberId = bannedMemberId,
            Note = note,
            BanEndUnixSeconds = banEnd.ToUnixTimeSeconds(),
        };

        var insertResult = await _repository.InsertGuildBan(ban);
        if (insertResult.IsFailed)
        {
            return new InternalServerError().CausedBy(insertResult.Errors);
        }
        return insertResult.Value;
    }

    public async ValueTask<Result<bool>> Exist(ulong guildId, Guid bannedMemberId)
    {
        var result = await _repository.Exist(guildId, bannedMemberId);
        if (result.IsFailed && result.HasError<ExceptionalError>())
        {
            return new InternalServerError().CausedBy(result.Errors);
        }

        return result.Value;
    }

    public async ValueTask<Result<GuildBanDto>> GetLatestExisted(ulong guildId, Guid bannedMemberId)
    {
        var result = await _repository.FindLatestExisted(guildId, bannedMemberId);
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

    public ValueTask<Result<List<GuildBanDto>>> GetBansByGuildId(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<List<GuildBanDto>>> GetDoBans(ulong guildId, Guid issuerMemberId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<List<GuildBanDto>>> GetBanned(ulong guildId, Guid bannedMemberId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> UnBan(ulong guildBanId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> UpdateBanEnd(ulong guildBanId, DateTimeOffset banEnd)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> DeleteBan(ulong guildBanId)
    {
        throw new NotImplementedException();
    }
}