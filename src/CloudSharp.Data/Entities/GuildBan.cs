using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildMemberBanId))]
public class GuildBan
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberBanId { get; init; }
    
    public ulong GuildId { get; init; }

    [ForeignKey(nameof(GuildId))] 
    public Guild Guild { get; init; } = null!;
    
    public required Guid? BanIssuerMemberId { get; init; }

    [ForeignKey(nameof(BanIssuerMemberId))]
    public Member? BanIssuer { get; init; }
    
    public required Guid BannedMemberId { get; init; }
    
    [ForeignKey(nameof(BannedMemberId))]
    public Member BannedMember { get; init; } = null!;
    
    [StringLength(256, MinimumLength = 5)]
    public required string Note { get; init; } 
    
    public required DateTimeOffset BanEnd { get; init; }

    public bool IsUnbanned { get; set; } = false;
    
    public DateTimeOffset CreatedOn { get; init; } = DateTime.UtcNow;
}