namespace CloudSharp.Share.DTO;

public record GuildMemberRoleDto
{
    public required ulong GuildMemberRoleId { get; init; }
    public required ulong GuildMemberId { get; init; }
    public required GuildRoleDto GuildRole { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
}