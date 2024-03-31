using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Util;

public static class FileInfoExtensions
{
    public static FileInfoDto ToDto(this FileInfo fileInfo, string directoryPath)
        => new() {
            Name = fileInfo.Name,
            Extension = fileInfo.Extension,
            LastWriteTime = fileInfo.LastWriteTime.ToUniversalTime().Ticks,
            Size = fileInfo.Length,
            Path = fileInfo.FullName[directoryPath.Length..].TrimStart('/'),
        };

    public static bool DirectoryExist(this FileInfo fileInfo)
        => fileInfo.Directory?.Exists ?? false;
}