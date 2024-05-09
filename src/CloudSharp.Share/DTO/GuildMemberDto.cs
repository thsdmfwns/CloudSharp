namespace CloudSharp.Share.DTO;

public record GuildMemberDto
{
    public required ulong GuildId { get; init; }
    
    public required ulong GuildMemberId { get; init; }

    public required Guid MemberId { get; init; }
    
    public required bool IsBanned { get; init; }
    
    public required DateTimeOffset CreatedOn { get; init; }
    
    public required DateTimeOffset? UpdatedOn { get; init; }

    public required IReadOnlyList<GuildMemberRoleDto> HadRoles { get; init; }

    
};