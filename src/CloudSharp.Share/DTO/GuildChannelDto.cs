namespace CloudSharp.Share.DTO;

public record GuildChannelDto
{
    public required ulong GuildId { get; init; }
    public required Guid ChannelId { get; init; }
    public required string ChannelName { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required DateTimeOffset? UpdatedOn { get; init; }
    public required IReadOnlyList<GuildChannelRoleDto> ChannelRoles { get; init; }

    
}