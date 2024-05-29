namespace CloudSharp.Share.DTO;

public record GuildBanDto
{
    public required ulong GuildBanId { get; init; }
    public required ulong GuildId { get; init; }
    public required Guid? IssuerMemberId { get; init; }
    public required MemberDto BannedMember { get; init; }
    public required bool IsUnbanned { get; init; }
    public required string Note { get; init; }
    public required DateTimeOffset BanEnd { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
}