using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildChannelId))]
public class GuildChannel
{
    [Key] 
    public Guid GuildChannelId { get; init; } = Guid.NewGuid();

    [StringLength(256, MinimumLength = 3)]
    public required string GuildChannelName { get; set; }
    
    [ForeignKey(nameof(GuildId))] 
    public required ulong GuildId { get; init; }
    public Guild Guild { get; init; } = null!;
    
    public ICollection<GuildChannelRole> GuildChannelRoles { get; } = new List<GuildChannelRole>();
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}