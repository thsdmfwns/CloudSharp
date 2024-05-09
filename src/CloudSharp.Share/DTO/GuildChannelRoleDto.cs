namespace CloudSharp.Share.DTO;

public record GuildChannelRoleDto
{
    public required ulong GuildChannelRoleId { get; init; }
    public required Guid GuildChannelId { get; set; }
    public required GuildRoleDto GuildRole { get; init; } 
    public required DateTimeOffset CreatedOn { get; init; }
};