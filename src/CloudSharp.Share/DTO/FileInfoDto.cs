using CloudSharp.Share.Enum;

namespace CloudSharp.Share.DTO;

public class FileInfoDto
{
    public required string Name { get; set; }
    public required string? Extension { get; set; }
    public required long LastWriteTime { get; set; }
    public required long Size { get; set; }
    public required string Path { get; set; }
}