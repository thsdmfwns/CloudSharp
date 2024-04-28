namespace CloudSharp.Share.DTO;

public class GuildChannelDto
{
    public required ulong GuildId { get; init; }
    public required Guid ChannelId { get; init; }
    public required string ChannelName { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? UpdateOn { get; init; }
    public required IReadOnlyList<GuildChannelRoleDto> ChannelRoles { get; init; }
}