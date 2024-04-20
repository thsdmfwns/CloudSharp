using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

[PrimaryKey(nameof(GuildMemberId))]
public class GuildMember
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberId { get; init; }
    [StringLength(256, MinimumLength = 3)]
    public required string MemberName { get; set; }
    
    [ForeignKey(nameof(GuildId))] 
    public required ulong GuildId { get; init; }
    public Guild Guild { get; init; } = null!;

    [ForeignKey(nameof(MemberId))] 
    public required Guid MemberId { get; init; }
    public Member Member { get; init; } = null!;

    public ICollection<GuildMemberRole> GuildMemberRoles { get; } = new List<GuildMemberRole>();
    
    public DateTime CreateOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}