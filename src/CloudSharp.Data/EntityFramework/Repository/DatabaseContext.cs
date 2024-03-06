using CloudSharp.Data.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Repository;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberRole> MemberRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var members = modelBuilder.Entity<Member>();
        var memberRoles = modelBuilder.Entity<MemberRole>();
        
        //Member
        members
            .HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId)
            .IsRequired();
        
        
        //MemberRole
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