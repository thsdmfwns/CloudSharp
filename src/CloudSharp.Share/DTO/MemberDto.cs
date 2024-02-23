namespace CloudSharp.Share.DTO;

public class MemberDto
{
    public required string MemberId { get; set; }
    public required ulong RoleId { get; set; }
    public required string Email { get; set; }
    public required string Nickname { get; set; }
    public string? ProfileImageURL { get; set; }
}