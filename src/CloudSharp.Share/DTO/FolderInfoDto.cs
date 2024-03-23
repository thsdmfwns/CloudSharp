namespace CloudSharp.Share.DTO;

public record FolderInfoDto
{
    public required string Name { get; set; }
    public required long LastWriteTime { get; set; }
    public required string Path { get; set; }
}