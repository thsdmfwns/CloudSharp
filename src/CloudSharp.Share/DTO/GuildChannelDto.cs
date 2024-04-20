namespace CloudSharp.Share.DTO;

public class GuildChannelDto
{
    public required ulong GuildId { get; init; }
    public required Guid ChannelId { get; init; }
    public required string ChannelName { get; init; }
    public required IReadOnlyList<GuildRoleDto> ChannelRoles { get; init; }
}