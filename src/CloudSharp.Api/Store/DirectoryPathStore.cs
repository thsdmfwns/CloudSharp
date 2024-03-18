using CloudSharp.Api.Service;
using CloudSharp.Share.Enum;

namespace CloudSharp.Api.Store;

public class DirectoryPathStore : IDirectoryPathStore
{
    public string VolumePath => "/cloud_sharp";
    public string MemberDirectoryPath => Path.Combine(VolumePath, "member");
    public string GuildDirectoryPath => Path.Combine(VolumePath, "guild");
    
    public string GetTargetPath(TargetFileDirectoryType targetFileDirectoryType, Guid directoryId, string targetPath)
    {
        var directoryPath = targetFileDirectoryType switch
        {
            TargetFileDirectoryType.Member => MemberDirectoryPath,
            TargetFileDirectoryType.Guild => MemberDirectoryPath,
            _ => VolumePath
        };
        return Path.Combine(directoryPath, directoryId.ToString(), targetPath);
    }
}