using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(MemberId))]
[Index(nameof(LoginId))]
public class Member
{
    [Key]
    public required Guid MemberId { get; init; }
    [Key]
    [StringLength(256, MinimumLength = 5)]
    public required string LoginId { get; init; }
    [StringLength(256, MinimumLength = 5)]
    public required string Password { get; set; }
    [StringLength(256, MinimumLength = 6)]
    public required string Email { get; set; }
    [StringLength(256, MinimumLength = 3)]
    public required string Nickname { get; set; }
    public required Guid? ProfileImageId { get; set; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public DateTime LastAccessed { get; set; }
    
    public ICollection<Share> Shares { get; } = new List<Share>();
    public ICollection<GuildMember> GuildMembers { get; } = new List<GuildMember>();
    public ICollection<GuildBan> GuildBanned { get; } = new List<GuildBan>();
    public ICollection<GuildBan> GuildDoBans { get; } = new List<GuildBan>();
};