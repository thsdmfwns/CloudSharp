using System.Text.Json.Serialization;

namespace CloudSharp.Share.DTO;

public record MemberDto
{
    [JsonPropertyName("mid")]
    public required string MemberId { get; init; }
    [JsonPropertyName("lid")]
    public required string LoginId { get; init; }
    [JsonPropertyName("email")]
    public required string Email { get; init; }
    [JsonPropertyName("nick")]
    public required string Nickname { get; init; }
    [JsonPropertyName("pid")]
    public string? ProfileImageId { get; init; }
}