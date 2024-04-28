namespace CloudSharp.Share.DTO;

public class GuildRoleDto
{
    public required ulong GuildId { get; init; }
    public required ulong GuildRoleId { get; init; }
    public required string RoleName { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime UpdateOn { get; init; }
    
    public required uint RoleColorRed { get; init; }
    public required uint RoleColorBlue { get; init; }
    public required uint RoleColorGreen { get; init; }
    public required uint RoleColorAlpha { get; init; }
    
}