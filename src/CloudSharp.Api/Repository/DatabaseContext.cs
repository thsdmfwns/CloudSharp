using CloudSharp.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Api.Repository;

public class DatabaseContext : DbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberRole> MemberRoles { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;port=11001;database=library;user=root;password=q1w2e3r4");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var members = modelBuilder.Entity<Member>();
        var memberRoles = modelBuilder.Entity<MemberRole>();

        
        //seeding
        memberRoles.HasData(
            new MemberRole
            {
                Id = 1,
                Name = "admin",
            },
            new MemberRole
            {
                Id = 2,
                Name = "member",
            }
            );
    }
}