namespace CloudSharp.Share.DTO;

public record GuildRoleDto
{
    public required ulong GuildId { get; init; }
    public required ulong GuildRoleId { get; init; }
    public required string RoleName { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required DateTimeOffset? UpdatedOn { get; init; }
    
    public required uint RoleColorRed { get; init; }
    public required uint RoleColorBlue { get; init; }
    public required uint RoleColorGreen { get; init; }
    public required uint RoleColorAlpha { get; init; }
    
}