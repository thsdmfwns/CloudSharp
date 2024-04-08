namespace CloudSharp.Share.DTO;

public record ShareDto
{
    public string ShareId { get; init; }
    public string MemberId { get; init; }
    public long ExpireTime { get; init; }
    public bool HasPassword { get; init; }
}