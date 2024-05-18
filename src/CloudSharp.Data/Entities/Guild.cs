using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(GuildId))]
public class Guild
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong GuildId { get; init; }
    [StringLength(256, MinimumLength = 3)]
    public required string GuildName { get; set; }
    public required Guid? GuildProfileImageId { get; set; }
    
    public ICollection<GuildChannel> GuildChannels { get; } = new List<GuildChannel>();
    public ICollection<GuildRole> GuildRoles { get; } = new List<GuildRole>();
    public ICollection<GuildMember> GuildMembers { get; } = new List<GuildMember>();
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}