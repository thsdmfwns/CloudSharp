using CloudSharp.Share.Enum;

namespace CloudSharp.Share.DTO;

public class FileInfoDto
{
    public required string Name { get; set; }
    public string? Extension { get; set; }
    public long LastWriteTime { get; set; }
    public long? Size { get; set; }
    public required string Path { get; set; }
}