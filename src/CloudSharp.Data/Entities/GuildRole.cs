using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildRoleId))]
public class GuildRole
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildRoleId { get; init; }
    [StringLength(256, MinimumLength = 1)]
    public required string RoleName { get; set; }
    
    public required uint RoleColorRed { get; set; }
    public required uint RoleColorBlue { get; set; }
    public required uint RoleColorGreen { get; set; }
    public required uint RoleColorAlpha { get; set; }
    
    public required ulong GuildId { get; init; }
    
    [ForeignKey(nameof(GuildId))] 
    public Guild Guild { get; init; } = null!;
    public ICollection<GuildMemberRole> GuildMemberRoles { get; } = new List<GuildMemberRole>();
    public ICollection<GuildChannelRole> GuildChannelRoles { get; } = new List<GuildChannelRole>();
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}