using CloudSharp.Api.Service;
using CloudSharp.Share.Enum;

namespace CloudSharp.Api.Store;

public class DirectoryPathStore : IDirectoryPathStore
{
    public DirectoryPathStore(string volumePath = "/cloud_sharp" )
    {
        VolumePath = volumePath;
        Directory.CreateDirectory(MemberDirectoryPath);
        Directory.CreateDirectory(GuildDirectoryPath);
    }

    public string VolumePath { get; private set; }
    public string MemberDirectoryPath => Path.Combine(VolumePath, "member");
    public string GuildDirectoryPath => Path.Combine(VolumePath, "guild");
    
    public string GetTargetPath(DirectoryType directoryType, Guid directoryId, string targetPath)
    {
        var directoryPath = directoryType switch
        {
            DirectoryType.Member => MemberDirectoryPath,
            DirectoryType.Guild => MemberDirectoryPath,
            _ => VolumePath
        };
        return Path.Combine(directoryPath, directoryId.ToString(), targetPath);
    }
}