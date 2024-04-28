using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.Entities;

[PrimaryKey(nameof(ShareId))]
[Index(nameof(FilePath))]
public class Share
{
    [Key]
    public required Guid ShareId { get; init; }
    [ForeignKey(nameof(MemberId))]
    public required Guid MemberId { get; init; }
    public Member Member { get; init; } = null!;
    
    [Key]
    [MaxLength]
    public required string FilePath { get; init; }
    [StringLength(256, MinimumLength = 5)]
    public required string? Password { get; set; }
    public required DateTime ExpireTime { get; set; }
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

}