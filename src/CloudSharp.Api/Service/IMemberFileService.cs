using CloudSharp.Data.Ticket;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IMemberFileService
{
    Result<List<FileInfoDto>> GetFiles(Guid directoryId, string? targetFolderPath);
    Result<FileInfoDto> GetFile(Guid directoryId, string targetPath);
    Result MoveFile(Guid directoryId, string targetPath, string destPath);
    Result RenameFile(Guid directoryId, string targetPath, string fileName);
    Result RemoveFile(Guid directoryId, string targetPath);
        
    Result<List<FolderInfoDto>> GetFolders(Guid directoryId, string? targetFolderPath);
    Result<FolderInfoDto> GetFolder(Guid directoryId, string targetPath);
    Result MoveFolder(Guid directoryId, string targetPath, string destPath);
    Result RenameFolder(Guid directoryId, string targetPath, string folderName);
    Result RemoveFolder(Guid directoryId, string targetPath);
    Result MakeFolder(Guid directoryId, string? targetFolderPath, string folderName);
}