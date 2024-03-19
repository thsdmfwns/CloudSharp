using CloudSharp.Share.Enum;

namespace CloudSharp.Api.Store;

public interface IDirectoryPathStore
{

    string VolumePath { get; }
    string MemberDirectoryPath { get; }
    string GuildDirectoryPath { get; }
    string GetTargetPath(DirectoryType directoryType, Guid directoryId,
        string targetPath);
}