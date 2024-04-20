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
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<GuildChannel> GuildChannels { get; set; }
    public DbSet<GuildChannelRole> GuildChannelRoles { get; set; }
    public DbSet<GuildMember> GuildMembers { get; set; }
    public DbSet<GuildMemberRole> GuildMemberRoles { get; set; }
    public DbSet<GuildRole> GuildRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var member = modelBuilder.Entity<Member>();
        var share = modelBuilder.Entity<Entities.Share>();
        var guild = modelBuilder.Entity<Guild>();
        var guildChannel = modelBuilder.Entity<GuildChannel>();
        var guildChannelRole = modelBuilder.Entity<GuildChannelRole>();
        var guildMember = modelBuilder.Entity<GuildMember>();
        var guildMemberRole = modelBuilder.Entity<GuildMemberRole>();
        var guildRole = modelBuilder.Entity<GuildRole>();

        #region share
        share
            .HasOne(e => e.Member)
            .WithMany(e => e.Shares)
            .HasForeignKey(e => e.MemberId)
            .IsRequired();
        #endregion

        #region guild
        guild
            .HasOne(e => e.OwnMember)
            .WithMany(e => e.Guilds)
            .HasForeignKey(e => e.OwnMemberId)
            .IsRequired();
        #endregion

        #region guildChannel

        guildChannel
            .HasOne(e => e.Guild)
            .WithMany(e => e.GuildChannels)
            .HasForeignKey(e => e.GuildId)
            .IsRequired();

        #endregion

        #region guildChannelRole

        guildChannelRole
            .HasOne(e => e.GuildChannel)
            .WithMany(e => e.GuildChannelRoles)
            .HasForeignKey(e => e.GuildChannelId)
            .IsRequired();
        
        guildChannelRole
            .HasOne(e => e.GuildRole)
            .WithMany(e => e.GuildChannelRoles)
            .HasForeignKey(e => e.GuildRoleId)
            .IsRequired();

        #endregion
        
        #region guildMember

        guildMember
            .HasOne(e => e.Guild)
            .WithMany(e => e.GuildMembers)
            .HasForeignKey(e => e.GuildId)
            .IsRequired();
        
        guildMember
            .HasOne(e => e.Member)
            .WithMany(e => e.GuildMembers)
            .HasForeignKey(e => e.MemberId)
            .IsRequired();

        #endregion

        #region guildMemberRole

        guildMemberRole
            .HasOne(e => e.GuildMember)
            .WithMany(e => e.GuildMemberRoles)
            .HasForeignKey(e => e.GuildMemberRoleId)
            .IsRequired();
        
        guildMemberRole
            .HasOne(e => e.GuildRole)
            .WithMany(e => e.GuildMemberRoles)
            .HasForeignKey(e => e.GuildRoleId)
            .IsRequired();

        #endregion

        #region guildRole

        guildRole
            .HasOne(e => e.Guild)
            .WithMany(e => e.GuildRoles)
            .HasForeignKey(e => e.GuildId)
            .IsRequired();

        #endregion
    }
}