using CloudSharp.Api.Error;
using CloudSharp.Api.Util;
using CloudSharp.Data.Store;
using CloudSharp.Data.Ticket;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using FluentResults;

namespace CloudSharp.Api.Service;

public class MemberFileService(IFileStore fileStore, ILogger<MemberFileService> _logger) : IMemberFileService
{
    private string MemberDirectoryPath => fileStore.MemberDirectoryPath;

    #region Files

     public Result<List<FileInfoDto>> GetFiles(Guid directoryId, string? targetFolderPath)
    {
        try
        {
            var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath ?? ".");
            if (findResult.IsFailed)
            {
                return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
            }

            if (!findResult.Value.Exists)
            {
                return Result.Fail(new NotFoundError().CausedBy("folder not found"));
            }

            return findResult.Value.GetFiles()
                .Select(x => x.ToDto(MemberDirectoryPath)).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public Result<FileInfoDto> GetFile(Guid directoryId, string targetPath)
    {
        try
        {
            var findResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId, targetPath);
            if (findResult.IsFailed)
            {
                return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
            }
            
            if (!findResult.Value.Exists)
            {
                return Result.Fail(new NotFoundError().CausedBy("file not found"));
            }

            return findResult.Value.ToDto(fileStore.MemberDirectoryPath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public Result MoveFile(Guid directoryId, string targetPath, string destPath)
    {
        try
        {
            if (!Path.GetFileName(destPath).IsFileName())
            {
                return Result.Fail(new BadRequestError().CausedBy($"wrong file name"));
            }
            
            var fromFIndResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId, targetPath);
            var destFindResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId, destPath);
            if (fromFIndResult.IsFailed || destFindResult.IsFailed)
            {
                return Result.Fail(new BadRequestError().CausedBy("invalid Path"));
            }

            if (!fromFIndResult.Value.Exists || !destFindResult.Value.DirectoryExist())
            {
                return Result.Fail(new NotFoundError().CausedBy("file or folder not found"));
            }
            if (destFindResult.Value.Exists)
            {
                return Result.Fail(new ConflictError().CausedBy("file exist"));
            }
            
            fromFIndResult.Value.MoveTo(destFindResult.Value.FullName);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }

    }

    public Result RenameFile(Guid directoryId, string targetPath, string fileName)
    {
        try
        {
            if (!fileName.IsFileName())
            {
                return new BadRequestError().CausedBy("Invalid file name");
            }
            return MoveFile(directoryId, targetPath, Path.Combine(Path.GetDirectoryName(targetPath) ?? ".", fileName));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public Result RemoveFile(Guid directoryId, string targetPath)
    {
        try
        {
            var findResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId, targetPath);
            if (findResult.IsFailed)
            {
                return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
            }

            if (!findResult.Value.Exists)
            {
                return Result.Fail(new NotFoundError().CausedBy("file nor found"));
            }
            findResult.Value.Delete();
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    #endregion
    
    #region Folder
    
    public Result<List<FolderInfoDto>> GetFolders(Guid directoryId, string? targetFolderPath)
    {
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath ?? ".");
        if (findResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
        }
        if (!findResult.Value.Exists)
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }

        return findResult.Value.GetDirectories().Select(x => x.ToFolderInfoDto(MemberDirectoryPath)).ToList();
    }

    public Result<FolderInfoDto> GetFolder(Guid directoryId, string targetPath)
    {
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        if (findResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
        }
        if (!findResult.Value.Exists)
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }
        
        return findResult.Value.ToFolderInfoDto(MemberDirectoryPath);
    }

    public Result MoveFolder(Guid directoryId, string targetPath, string destPath)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
        {
            return Result.Fail(new BadRequestError().CausedBy("empty target path"));
        }
        
        var fromFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        var destFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, destPath);
        if (fromFindResult.IsFailed || destFindResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy("invalid path"));
        }

        if (!fromFindResult.Value.Exists || !destFindResult.Value.ParentDirectoryExist())
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }

        if (destFindResult.Value.Exists)
        {
            return Result.Fail(new ConflictError().CausedBy("folder exist"));
        }
        
        fromFindResult.Value.MoveTo(destFindResult.Value.FullName);
        return Result.Ok();
    }

    public Result RenameFolder(Guid directoryId, string targetPath, string folderName)
    {
        if (!folderName.IsFolderName())
        {
            return Result.Fail(new BadRequestError().CausedBy($"wrong folderName : {folderName}"));
        }

        var destPath = Path.Combine(Path.GetDirectoryName(targetPath) ?? ".", folderName);
        var destFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, destPath);
        if (destFindResult.IsFailed)
        {
            return new BadRequestError().CausedBy(destFindResult.Errors);
        }
        if (!destFindResult.Value.ParentDirectoryExist())
        {
            Directory.CreateDirectory(destFindResult.Value.Parent!.FullName);
        }
        
        return MoveFolder(directoryId, targetPath, destPath);
    }

    public Result RemoveFolder(Guid directoryId, string targetPath)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
        {
            return Result.Fail(new BadRequestError().CausedBy("empty target path"));
        }
        
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        if (findResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(findResult.Errors));
        }
        if (!findResult.Value.Exists)
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }
        
        findResult.Value.Delete(true);
        return Result.Ok();
    }

    public Result MakeFolder(Guid directoryId, string? targetFolderPath, string folderName)
    {
        if (!folderName.IsFolderName())
        {
            return Result.Fail(new BadRequestError().CausedBy($"wrong folder name : {folderName}"));
        }
        
        var makeFolderFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId,
            Path.Combine(targetFolderPath ?? ".", folderName));
        if (makeFolderFindResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(makeFolderFindResult.Errors));
        }
        if (!makeFolderFindResult.Value.ParentDirectoryExist())
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }
        if (makeFolderFindResult.Value.Exists)
        {
            return Result.Fail(new ConflictError().CausedBy("exist folder"));
        }
        
        Directory.CreateDirectory(makeFolderFindResult.Value.FullName);
        return Result.Ok();
    }

    #endregion
   
}