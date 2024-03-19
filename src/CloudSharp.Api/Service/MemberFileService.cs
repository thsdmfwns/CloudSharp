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
        throw new NotImplementedException();
    }

    public Result<FolderInfoDto> GetFolder(Guid directoryId, string targetPath)
    {
        throw new NotImplementedException();
    }

    public Result MoveFolder(Guid directoryId, string targetPath, string? toFolderPath)
    {
        throw new NotImplementedException();
    }

    public Result RenameFolder(Guid directoryId, string targetPath, string folderName)
    {
        throw new NotImplementedException();
    }

    public Result RemoveFolder(Guid directoryId, string targetPath)
    {
        throw new NotImplementedException();
    }

    public Result MakeFolder(Guid directoryId, string? targetFolderPath, string folderName)
    {
        throw new NotImplementedException();
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