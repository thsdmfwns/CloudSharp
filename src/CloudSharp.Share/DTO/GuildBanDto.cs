namespace CloudSharp.Share.DTO;

public record GuildBanDto
{
    public required ulong GuildBanId { get; init; }
    public required GuildDto Guild { get; init; }
    public required MemberDto Issuer { get; init; }
    public required MemberDto BannedMember { get; init; }
    public required bool IsUnbanned { get; init; }
    public required string Note { get; init; }
    public required DateTimeOffset BanEnd { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
}