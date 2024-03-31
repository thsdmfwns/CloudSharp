using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class DirectoryInfoExtensions
{
    public static FolderInfoDto ToFolderInfoDto(this DirectoryInfo directoryInfo, string memberDirectoryPath)
    {
        return new FolderInfoDto
        {
            Name = directoryInfo.Name,
            Path = directoryInfo.FullName[memberDirectoryPath.Length..].TrimStart('/'),
            LastWriteTime = directoryInfo.LastWriteTime.ToUniversalTime().Ticks
        };
    }

    public static bool ParentDirectoryExist(this DirectoryInfo directoryInfo)
        => directoryInfo.Parent?.Exists ?? false;
}