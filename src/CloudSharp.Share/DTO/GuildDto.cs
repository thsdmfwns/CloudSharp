namespace CloudSharp.Share.DTO;

public record GuildDto
{
    public required ulong GuildId { get; init; }
    public required string GuildName { get; init; }
    public required Guid? GuildProfileId { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? UpdateOn { get; init; }
    
    public required IReadOnlyList<GuildMemberDto> Members { get; init; }
    public required IReadOnlyList<GuildChannelDto> Channels { get; init; }
    public required IReadOnlyList<GuildRoleDto> Roles { get; init; }
}