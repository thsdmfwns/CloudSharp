﻿// <auto-generated />
using System;
using CloudSharp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CloudSharp.Data.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CloudSharp.Data.Entities.Guild", b =>
                {
                    b.Property<ulong>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GuildName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<Guid?>("GuildProfileImageId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("GuildId");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildChannel", b =>
                {
                    b.Property<Guid>("GuildChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GuildChannelName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("GuildChannelId");

                    b.HasIndex("GuildId");

                    b.ToTable("GuildChannels");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildChannelRole", b =>
                {
                    b.Property<ulong>("GuildChannelRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("GuildChannelId")
                        .HasColumnType("char(36)");

                    b.Property<ulong>("GuildRoleId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("GuildChannelRoleId");

                    b.HasIndex("GuildChannelId");

                    b.HasIndex("GuildRoleId");

                    b.ToTable("GuildChannelRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMember", b =>
                {
                    b.Property<ulong>("GuildMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("char(36)");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("GuildMemberId");

                    b.HasIndex("GuildId");

                    b.HasIndex("IsBanned");

                    b.HasIndex("MemberId");

                    b.ToTable("GuildMembers");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMemberBan", b =>
                {
                    b.Property<ulong>("GuildMemberBanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTimeOffset>("BanEnds")
                        .HasColumnType("datetime");

                    b.Property<ulong>("BanIssuerGuildMemberId")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<ulong>("GuildMemberId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("GuildMemberBanId");

                    b.HasIndex("BanIssuerGuildMemberId");

                    b.HasIndex("GuildMemberId");

                    b.ToTable("GuildMemberBans");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMemberRole", b =>
                {
                    b.Property<ulong>("GuildMemberRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("GuildMemberId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildRoleId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("GuildMemberRoleId");

                    b.HasIndex("GuildMemberId");

                    b.HasIndex("GuildRoleId");

                    b.ToTable("GuildMemberRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildRole", b =>
                {
                    b.Property<ulong>("GuildRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("RoleColorAlpha")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("RoleColorBlue")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("RoleColorGreen")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("RoleColorRed")
                        .HasColumnType("int unsigned");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("GuildRoleId");

                    b.HasIndex("GuildId");

                    b.ToTable("GuildRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.Member", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime>("LastAccessed")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LoginId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<Guid?>("ProfileImageId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("MemberId");

                    b.HasIndex("LoginId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.Share", b =>
                {
                    b.Property<Guid>("ShareId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ExpireTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Password")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ShareId");

                    b.HasIndex("FilePath");

                    b.HasIndex("MemberId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildChannel", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.Guild", "Guild")
                        .WithMany("GuildChannels")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildChannelRole", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.GuildChannel", "GuildChannel")
                        .WithMany("GuildChannelRoles")
                        .HasForeignKey("GuildChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CloudSharp.Data.Entities.GuildRole", "GuildRole")
                        .WithMany("GuildChannelRoles")
                        .HasForeignKey("GuildRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildChannel");

                    b.Navigation("GuildRole");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMember", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.Guild", "Guild")
                        .WithMany("GuildMembers")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CloudSharp.Data.Entities.Member", "Member")
                        .WithMany("GuildMembers")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMemberBan", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.GuildMember", "BanIssuer")
                        .WithMany("GuildMemberBans")
                        .HasForeignKey("BanIssuerGuildMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CloudSharp.Data.Entities.GuildMember", "GuildMember")
                        .WithMany("GuildMemberBanned")
                        .HasForeignKey("GuildMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BanIssuer");

                    b.Navigation("GuildMember");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMemberRole", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.GuildMember", "GuildMember")
                        .WithMany("GuildMemberRoles")
                        .HasForeignKey("GuildMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CloudSharp.Data.Entities.GuildRole", "GuildRole")
                        .WithMany("GuildMemberRoles")
                        .HasForeignKey("GuildRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildMember");

                    b.Navigation("GuildRole");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildRole", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.Guild", "Guild")
                        .WithMany("GuildRoles")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.Share", b =>
                {
                    b.HasOne("CloudSharp.Data.Entities.Member", "Member")
                        .WithMany("Shares")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.Guild", b =>
                {
                    b.Navigation("GuildChannels");

                    b.Navigation("GuildMembers");

                    b.Navigation("GuildRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildChannel", b =>
                {
                    b.Navigation("GuildChannelRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildMember", b =>
                {
                    b.Navigation("GuildMemberBanned");

                    b.Navigation("GuildMemberBans");

                    b.Navigation("GuildMemberRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.GuildRole", b =>
                {
                    b.Navigation("GuildChannelRoles");

                    b.Navigation("GuildMemberRoles");
                });

            modelBuilder.Entity("CloudSharp.Data.Entities.Member", b =>
                {
                    b.Navigation("GuildMembers");

                    b.Navigation("Shares");
                });
#pragma warning restore 612, 618
        }
    }
}
