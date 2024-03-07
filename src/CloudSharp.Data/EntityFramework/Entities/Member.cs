using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Entities;

[PrimaryKey(nameof(MemberId))]
[Index(nameof(LoginId))]
public class Member
{
    [Key]
    public required Guid MemberId { get; init; }
    [Key]
    public required string LoginId { get; init; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Nickname { get; set; }
    public required Guid? ProfileImageId { get; set; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public DateTime LastAccessed { get; set; }
};