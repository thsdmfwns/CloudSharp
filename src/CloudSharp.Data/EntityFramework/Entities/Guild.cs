using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

[PrimaryKey(nameof(GuildId))]
public class Guild
{
    [Key]
    public ulong GuildId { get; init; }
    [StringLength(256, MinimumLength = 3)]
    public required string GuildName { get; set; }
    public required Guid? GuildProfileImageId { get; set; }
    
    [ForeignKey(nameof(OwnMemberId))] 
    public Guid OwnMemberId { get; init; }
    public Member OwnMember { get; init; } = null!;
    public ICollection<GuildChannel> GuildChannels { get; } = new List<GuildChannel>();
    public ICollection<GuildRole> GuildRoles { get; } = new List<GuildRole>();
    public ICollection<GuildMember> GuildMembers { get; } = new List<GuildMember>();
    
    public DateTime CreateOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}