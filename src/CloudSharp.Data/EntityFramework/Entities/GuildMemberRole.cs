using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

[PrimaryKey(nameof(GuildMemberRoleId))]
public class GuildMemberRole
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildMemberRoleId { get; init; }

    [ForeignKey(nameof(GuildMemberId))] 
    public required ulong GuildMemberId { get; init; }
    public GuildMember GuildMember { get; init; } = null!;
    
    [ForeignKey(nameof(GuildRoleId))] 
    public required ulong GuildRoleId { get; init; }
    public GuildRole GuildRole { get; init; } = null!;
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
}