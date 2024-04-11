using CloudSharp.Api.Error;
using CloudSharp.Api.Util;
using CloudSharp.Data.EntityFramework.Repository;
using CloudSharp.Data.Store;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using FluentResults;

namespace CloudSharp.Api.Service;

public class ShareService(IShareRepository repository, IFileStore fileStore) : IShareService
{
    public async ValueTask<Result<ShareDto>> GetShare(Guid shareId)
    {
        var share = await repository.GetShareById(shareId);
        if (share is null)
        {
            return new NotFoundError().CausedBy("share not found");
        }

        return share.ToShareDto();
    }

    public async ValueTask<Result<bool>> VerifySharePassword(Guid shareId, string password)
    {
        var share = await repository.GetShareById(shareId);
        if (share is null)
        {
            return new NotFoundError().CausedBy("share not found");
        }

        var passwordHash = share.Password;
        if (passwordHash is null)
        {
            return true;
        }
        return PasswordHasher.VerifyHashedPassword(passwordHash, password);
    }

    public async ValueTask<Result<List<ShareDto>>> GetSharesByMemberId(Guid memberId)
    {
        var shares = await repository.GetSharesByMemberId(memberId);
        return shares.Select(x => x.ToShareDto()).ToList();
    }

    public async ValueTask<Result<List<ShareDto>>> GetShareInFolder(Guid memberId, string folderPath)
    {
        var findFolderResult = fileStore.GetDirectoryInfo(DirectoryType.Member, memberId, folderPath);
        if (findFolderResult.IsFailed)
        {
            return new BadRequestError().CausedBy(findFolderResult.Errors);
        }
        if (!findFolderResult.Value.Exists)
        {
            return new NotFoundError().CausedBy("folder not found");
        }

        var shares = await repository.GetSharesByFolderPath(memberId, folderPath);
        return shares.Select(x => x.ToShareDto()).ToList();
    }

    public async ValueTask<Result<Guid>> AddShare(Guid memberId, string filePath, string? password, DateTime? expireTime)
    {
        var findFileResult = fileStore.GetFileInfo(DirectoryType.Member, memberId, filePath);
        if (findFileResult.IsFailed)
        {
            return new BadRequestError().CausedBy(findFileResult.Errors);
        }
        if (!findFileResult.Value.Exists)
        {
            return new NotFoundError().CausedBy("file not found");
        }
        var share = new Data.EntityFramework.Entities.Share
        {
            ShareId = Guid.NewGuid(),
            MemberId = memberId,
            FilePath = filePath,
            Password = password,
            ExpireTime = expireTime ?? DateTime.MaxValue,
        };
        var result = await repository.InsertShare(share);
        if (result.IsFailed)
        {
            return new ConflictError().CausedBy(result.Errors);
        }

        return result.Value;
    }

    public async ValueTask<Result> UpdateExpireTimeShare(Guid shareId, DateTime? expireTime)
    {
        var updateResult = await repository.UpdateShare(shareId, x => x.ExpireTime = expireTime ?? DateTime.MaxValue);
        return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
    }

    public async ValueTask<Result> UpdatePassword(Guid shareId, string? password)
    {
        string? passwordHash = null;
        if (password is not null)
        {
            passwordHash = PasswordHasher.HashPassword(password);
        }
        var updateResult = await repository.UpdateShare(shareId, x => x.Password = passwordHash);
        return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
    }

    public async ValueTask<Result> DeleteShare(Guid shareId)
    {
        var deleteResult = await repository.DeleteShare(shareId);
        return Result.OkIf(deleteResult.IsSuccess, new NotFoundError().CausedBy(deleteResult.Errors));
    }

    public async ValueTask<Result> DeleteShareByFilePath(Guid memberId, string filePath)
    {
        var findFileResult = fileStore.GetFileInfo(DirectoryType.Member, memberId, filePath);
        if (findFileResult.IsFailed)
        {
            return new BadRequestError().CausedBy(findFileResult.Errors);
        }

        var deleteResult = await repository.DeleteShareByPath(memberId, filePath);
        return Result.OkIf(deleteResult.IsSuccess, new NotFoundError().CausedBy(deleteResult.Errors));
    }

    public async ValueTask<Result> DeleteShareInFolder(Guid memberId, string folderPath)
    {
        var findFolderResult = fileStore.GetDirectoryInfo(DirectoryType.Member, memberId, folderPath);
        if (findFolderResult.IsFailed)
        {
            return new BadRequestError().CausedBy(findFolderResult.Errors);
        }
        
        var deleteResult = await repository.DeleteShareByStartWithPath(memberId, folderPath);
        return Result.OkIf(deleteResult.IsSuccess, new NotFoundError().CausedBy(deleteResult.Errors));
    }
}