using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IShareService
{
    ValueTask<Result<ShareDto>> GetShare(Guid shareId);
    ValueTask<Result> ValidateSharePassword(Guid shareId, string password);
    ValueTask<Result<List<ShareDto>>> GetSharesByMemberId(Guid memberId);
    ValueTask<Result<List<ShareDto>>> GetShareInFolder(Guid memberId, string folderPath);
    
    ValueTask<Result<Guid>> AddShare(Guid memberId, string filePath, string? password, DateTime? expireTime);

    ValueTask<Result> UpdateExpireTimeShare(Guid shareId, DateTime? expireTime);
    ValueTask<Result> UpdatePassword(Guid shareId, string? password);

    ValueTask<Result> DeleteShare(Guid shareId);
    ValueTask<Result> DeleteShareByFilePath(Guid memberId, string filePath);
    ValueTask<Result> DeleteShareInFolder(Guid memberId, string folderPath);
}