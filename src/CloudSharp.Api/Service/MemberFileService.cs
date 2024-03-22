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
            var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId,
                targetFolderPath ?? string.Empty);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
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
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
            }

            return findResult.Value.ToDto(fileStore.MemberDirectoryPath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public Result MoveFile(Guid directoryId, string targetPath, string? toFolderPath, string fileName)
    {
        try
        {
            if (!fileName.IsFileName())
            {
                return Result.Fail(new BadRequestError().CausedBy($"wrong file name : {fileName}"));
            }
            
            var fromFIndResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId, targetPath);
            var toFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, toFolderPath ?? string.Empty);
            if (fromFIndResult.IsFailed || toFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError().CausedBy("folder or file not found"));
            }
            
            var toPathWithFileName = Path.Combine(toFindResult.Value.FullName, fileName);
            fromFIndResult.Value.MoveTo(toPathWithFileName);
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
                return Result.Fail(new BadRequestError().CausedBy($"wrong file name : {fileName}"));
            }
            
            var findResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId,
                targetPath);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
            }
            var changeFilePath = Path.Combine(findResult.Value.DirectoryName!, fileName);
            findResult.Value.MoveTo(changeFilePath);
            return Result.Ok();
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
            var findResult = fileStore.GetFileInfo(DirectoryType.Member, directoryId,
                targetPath);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
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
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath ?? "");
        if (findResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
        }

        return findResult.Value.GetDirectories().Select(x => x.ToFolderInfoDto(MemberDirectoryPath)).ToList();
    }

    public Result<FolderInfoDto> GetFolder(Guid directoryId, string targetPath)
    {
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        if (findResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
        }

        return findResult.Value.ToFolderInfoDto(MemberDirectoryPath);
    }

    public Result MoveFolder(Guid directoryId, string targetPath, string? toFolderPath)
    {
        var fromFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        var toFindResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, toFolderPath ?? "");
        if (fromFindResult.IsFailed || toFindResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }
        fromFindResult.Value.MoveTo(toFindResult.Value.FullName);
        return Result.Ok();
    }

    public Result RenameFolder(Guid directoryId, string targetPath, string folderName)
    {
        if (!folderName.IsFolderName())
        {
            return Result.Fail(new BadRequestError().CausedBy($"wrong folderName : {folderName}"));
        }
        
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        if (findResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
        }

        
        var changeFolderPath = Path.Combine(findResult.Value.Parent!.FullName, folderName);
        Directory.CreateDirectory(Path.Combine(changeFolderPath, ".."));
        
        return Result.OkIf(Directory.Exists(changeFolderPath), new ConflictError().CausedBy("folder exist"));
    }

    public Result RemoveFolder(Guid directoryId, string targetPath)
    {
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetPath);
        if (findResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
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
        
        var findResult = fileStore.GetDirectoryInfo(DirectoryType.Member, directoryId, targetFolderPath ?? "");
        if (findResult.IsFailed)
        {
            return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
        }

        var makeFolderPath = Path.Combine(findResult.Value.FullName, folderName);
        
        if (Directory.Exists(makeFolderPath))
        {
            return Result.Fail(new ConflictError().CausedBy("exist folder"));
        }
        
        Directory.CreateDirectory(Path.Combine(findResult.Value.FullName, folderName));
        return Result.Ok();
    }

    #endregion

    #region Ticket

    public Result<FileStreamTicket> GetFileStreamTicket(Guid directoryId, string targetPath)
    {
        throw new NotImplementedException();
    }

    public Result<FileUploadTicket> GetFileUploadTicket(Guid directoryId, string? targetFolderPath, string filename)
    {
        throw new NotImplementedException();
    }

    #endregion
  
   
}