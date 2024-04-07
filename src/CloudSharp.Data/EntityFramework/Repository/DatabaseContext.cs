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
    public DbSet<Entities.Share> Shares { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var members = modelBuilder.Entity<Member>();
        var shares = modelBuilder.Entity<Entities.Share>();

        shares
            .HasOne(e => e.Member)
            .WithMany(e => e.Shares)
            .HasForeignKey(e => e.MemberId)
            .IsRequired();
    }
}