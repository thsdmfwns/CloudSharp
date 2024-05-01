using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildChannelRoleId))]
public class GuildChannelRole
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildChannelRoleId { get; set; }
    [ForeignKey(nameof(GuildChannelId))] 
    public required Guid GuildChannelId { get; set; }
    public GuildChannel GuildChannel { get; init; } = null!;
    
    [ForeignKey(nameof(GuildRoleId))] 
    public required ulong GuildRoleId { get; init; }
    public GuildRole GuildRole { get; init; } = null!;
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
}