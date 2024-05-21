using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildMemberRoleId))]
public class GuildMemberRole
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberRoleId { get; init; }

    public required ulong GuildMemberId { get; init; }
    
    [ForeignKey(nameof(GuildMemberId))] 
    public GuildMember GuildMember { get; init; } = null!;
    
    public required ulong GuildRoleId { get; init; }
    
    [ForeignKey(nameof(GuildRoleId))] 
    public GuildRole GuildRole { get; init; } = null!;
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
}