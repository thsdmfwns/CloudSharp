using CloudSharp.Api.Error;
using CloudSharp.Api.Store;
using CloudSharp.Api.Util;
using CloudSharp.Data.Ticket;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using FluentResults;

namespace CloudSharp.Api.Service;

public class MemberFileService(IDirectoryPathStore _directoryPathStore, ILogger<MemberFileService> _logger) : IMemberFileService
{
    private string _memberDirectoryPath => _directoryPathStore.MemberDirectoryPath;

    #region Files

     public Result<List<FileInfoDto>> GetFiles(Guid directoryId, string? targetFolderPath)
    {
        try
        {

            var path = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId,
                targetFolderPath ?? string.Empty);
            if (!Directory.Exists(path))
            {
                return Result.Fail(new NotFoundError().CausedBy("folder not found"));
            }

            return new DirectoryInfo(path).GetFiles()
                .Select(x => x.ToDto(_memberDirectoryPath)).ToList();
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
            var path = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId,
                targetPath);
            if (!File.Exists(path))
            {
                return Result.Fail(new NotFoundError().CausedBy("file not found"));
            }

            return new FileInfo(path).ToDto(_directoryPathStore.MemberDirectoryPath);
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
            var fromPath = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId, targetPath);
            var toPath = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId, toFolderPath ?? string.Empty);
            if (!File.Exists(fromPath) || !Directory.Exists(toPath))
            {
                return Result.Fail(new NotFoundError().CausedBy("folder or file not found"));
            }
            var toPathWithFileName = Path.Combine(toPath, fileName);
            ;       File.Move(fromPath, toPathWithFileName, true);
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
            var path = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId,
                targetPath);
            if (!File.Exists(path))
            {
                return Result.Fail(new NotFoundError().CausedBy("file not found"));
            }

            var directory = Path.GetDirectoryName(path);
            var changeFilePath = Path.Combine(directory!, fileName);
            File.Move(path, changeFilePath);
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
            var path = _directoryPathStore.GetTargetPath(TargetFileDirectoryType.Member, directoryId,
                targetPath);
            if (!File.Exists(path))
            {
                return Result.Fail(new NotFoundError().CausedBy("file not found"));
            }

            File.Delete(path);
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