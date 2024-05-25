using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildMemberBanId))]
public class GuildMemberBan
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberBanId { get; init; }
    
    public required ulong GuildMemberId { get; init; }
    
    [ForeignKey(nameof(GuildMemberId))]
    public GuildMember GuildMember { get; init; } = null!;
    
    [StringLength(256, MinimumLength = 5)]
    public required string Note { get; init; } 
    
    public required DateTimeOffset BanEnds { get; init; }
    
    public DateTimeOffset CreatedOn { get; init; } = DateTime.UtcNow;
}