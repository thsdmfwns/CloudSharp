namespace CloudSharp.Share.DTO;

public class MemberDto
{
    public required ulong Idx { get; set; }
    public required string Id { get; set; }
    public required ulong Role { get; set; }
    public required string Email { get; set; }
    public required string Nickname { get; set; }
    public required string Directory { get; set; }
    public string? ProfileImageURL { get; set; }
}