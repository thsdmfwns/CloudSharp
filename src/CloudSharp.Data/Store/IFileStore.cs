using CloudSharp.Share.Enum;
using FluentResults;

namespace CloudSharp.Data.Store;

public interface IFileStore
{

    string VolumePath { get; }
    string MemberDirectoryPath { get; }
    string GuildDirectoryPath { get; }
    Result<FileInfo> GetFileInfo(DirectoryType directoryType, Guid directoryId,
        string targetPath);
    Result<DirectoryInfo> GetDirectoryInfo(DirectoryType directoryType, Guid directoryId,
        string targetPath);
    string GetTargetPath(DirectoryType directoryType, Guid directoryId, string targetPath);
}