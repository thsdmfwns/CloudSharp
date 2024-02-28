using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Api.Entities;

[PrimaryKey(nameof(Id))]
public class MemberRole
{
    [Key]
    public required ulong Id { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
}