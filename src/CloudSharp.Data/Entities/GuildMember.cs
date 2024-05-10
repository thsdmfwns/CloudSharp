using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildMemberId))]
[Index(nameof(IsBanned))]
public class GuildMember
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberId { get; init; }
    [StringLength(256, MinimumLength = 3)]
    public required string MemberName { get; set; }
    public bool IsBanned { get; set; }
    
    [ForeignKey(nameof(GuildId))] 
    public required ulong GuildId { get; init; }
    public Guild Guild { get; init; } = null!;

    [ForeignKey(nameof(MemberId))] 
    public required Guid MemberId { get; init; }
    public Member Member { get; init; } = null!;

    public ICollection<GuildMemberRole> GuildMemberRoles { get; } = new List<GuildMemberRole>();
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}