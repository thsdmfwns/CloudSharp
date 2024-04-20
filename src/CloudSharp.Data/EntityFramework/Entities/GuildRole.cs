using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

[PrimaryKey(nameof(GuildRoleId))]
public class GuildRole
{
    [Key] 
    public ulong GuildRoleId { get; init; }
    [StringLength(256, MinimumLength = 1)]
    public required string RoleName { get; set; }
    
    [ForeignKey(nameof(GuildId))] 
    public required ulong GuildId { get; init; }
    public Guild Guild { get; init; } = null!;
    public ICollection<GuildMemberRole> GuildMemberRoles { get; } = new List<GuildMemberRole>();
    public ICollection<GuildChannelRole> GuildChannelRoles { get; } = new List<GuildChannelRole>();
    
    public DateTime CreateOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}