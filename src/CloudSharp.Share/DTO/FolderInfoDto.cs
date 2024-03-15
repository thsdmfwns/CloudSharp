namespace CloudSharp.Share.DTO;

public class FolderInfoDto
{
    public required string Name { get; set; }
    public long LastWriteTime { get; set; }
    public required string Path { get; set; }
}