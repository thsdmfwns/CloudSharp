using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IShareService
{
    ValueTask<Result<ShareDto>> GetShare(Guid shareId);
    ValueTask<Result<bool>> HasPassword(Guid shareId);
    ValueTask<bool> ExistShare(Guid shareId);
    ValueTask<Result<List<ShareDto>>> GetSharesByMemberId(Guid memberId);
    ValueTask<Result<List<ShareDto>>> GetShareInFolder(Guid memberId, string folderPath);
    ValueTask<Result> LoginShare(Guid shareId, string password);
    
    ValueTask<Result<Guid>> AddShare(Guid memberId, string filePath, string? password, DateTime? expireTime);

    ValueTask<Result> UpdateShare(Guid shareId, Action<Data.EntityFramework.Entities.Share> updateAction);

    ValueTask<Result> DeleteShare(Guid shareId);
    ValueTask<Result> DeleteShareByFilePath(Guid memberId, string filePath);
    ValueTask<Result> DeleteShareInFolder(Guid memberId, string folderPath);
}