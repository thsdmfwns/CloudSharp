using CloudSharp.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Api.Repository;

public class DatabaseContext : DbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberRole> MemberRoles { get; set; }
}