namespace CloudSharp.Share.DTO;

public record ShareDto
{
    public required string ShareId { get; init; }
    public required string MemberId { get; init; }
    public required long ExpireTime { get; init; }
    public required bool HasPassword { get; init; }
    public required string FileName { get; init; }
}