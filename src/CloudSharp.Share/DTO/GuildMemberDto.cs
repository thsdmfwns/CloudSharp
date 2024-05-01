namespace CloudSharp.Share.DTO;

public record GuildMemberDto
{
    public required ulong GuildId { get; init; }
    
    public required ulong GuildMemberId { get; init; }

    public required Guid MemberId { get; init; }
    
    public required bool IsBanned { get; init; }
    
    public required DateTime CreatedOn { get; init; }
    
    public required DateTime? UpdatedOn { get; init; }

    public required IReadOnlyList<GuildMemberRoleDto> HadRoles { get; init; }
};