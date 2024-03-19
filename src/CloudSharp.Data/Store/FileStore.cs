using CloudSharp.Share.Enum;

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