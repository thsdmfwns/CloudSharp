using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

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
    public required string FilePath { get; init; }
    public required string? Password { get; set; }
    public required DateTime ExpireTime { get; set; }
    
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

}