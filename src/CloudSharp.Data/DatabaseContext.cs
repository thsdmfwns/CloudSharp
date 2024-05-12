using CloudSharp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    public DbSet<Member> Members { get; init; }
    public DbSet<Entities.Share> Shares { get; init; }
    public DbSet<Guild> Guilds { get; init; }
    public DbSet<GuildChannel> GuildChannels { get; init; }
    public DbSet<GuildChannelRole> GuildChannelRoles { get; init; }
    public DbSet<GuildMember> GuildMembers { get; init; }
    public DbSet<GuildMemberRole> GuildMemberRoles { get; init; }
    public DbSet<GuildRole> GuildRoles { get; init; }

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
            .HasForeignKey(e => e.GuildMemberId)
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