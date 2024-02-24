using System.ComponentModel.DataAnnotations;
using CloudSharp.Share.DTO;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Api.Entities;

[PrimaryKey(nameof(MemberId))]
public class Member
{
    [Key]
    public required Guid MemberId { get; init; }
    [Key]
    public required string LoginId { get; init; }
    public required string Password { get; set; }
    public required MemberRole Role { get; set; }
    public required string Email { get; set; }
    public required string Nickname { get; set; }
    public Guid? ProfileImageId { get; set; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public DateTime LastAccessed { get; set; }
};