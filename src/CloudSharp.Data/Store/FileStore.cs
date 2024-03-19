using CloudSharp.Share.Enum;
using FluentResults;
using Result = MySqlX.XDevAPI.Common.Result;

namespace CloudSharp.Data.Store;

public class FileStore : IFileStore
{
    public FileStore(string volumePath = "/cloud_sharp" )
    {
        VolumePath = volumePath;
        Directory.CreateDirectory(MemberDirectoryPath);
        Directory.CreateDirectory(GuildDirectoryPath);
    }

    public string VolumePath { get; private set; }
    public string MemberDirectoryPath => Path.Combine(VolumePath, "member");
    public string GuildDirectoryPath => Path.Combine(VolumePath, "guild");
    public Result<FileInfo> GetFileInfo(DirectoryType directoryType, Guid directoryId, string targetPath)
    {
        var filePath = GetTargetPath(directoryType, directoryId, targetPath);
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            return FluentResults.Result.Fail("file not found");
        }

        return fileInfo;
    }

    public Result<DirectoryInfo> GetDirectoryInfo(DirectoryType directoryType, Guid directoryId, string targetPath)
    {
        var filePath = GetTargetPath(directoryType, directoryId, targetPath);
        var directoryInfo = new DirectoryInfo(filePath);
        if (!directoryInfo.Exists)
        {
            return FluentResults.Result.Fail("file not found");
        }
        return directoryInfo;
    }

    private string GetDirectoryPath(DirectoryType directoryType) => directoryType switch
    {
        DirectoryType.Member => MemberDirectoryPath,
        DirectoryType.Guild => MemberDirectoryPath,
        _ => VolumePath
    };
    public string GetTargetPath(DirectoryType directoryType, Guid directoryId, string targetPath)
       =>Path.Combine(GetDirectoryPath(directoryType), directoryId.ToString(), targetPath);
}