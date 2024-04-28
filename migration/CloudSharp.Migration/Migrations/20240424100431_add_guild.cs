using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace CloudSharp.Migraition.Migrations
{
    /// <inheritdoc />
    public partial class add_guild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Shares",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Members",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "Members",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "LoginId",
                table: "Members",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Members",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GuildName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    GuildProfileImageId = table.Column<Guid>(type: "char(36)", nullable: true),
                    OwnMemberId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_Guilds_Members_OwnMemberId",
                        column: x => x.OwnMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildChannels",
                columns: table => new
                {
                    GuildChannelId = table.Column<Guid>(type: "char(36)", nullable: false),
                    GuildChannelName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannels", x => x.GuildChannelId);
                    table.ForeignKey(
                        name: "FK_GuildChannels_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildMembers",
                columns: table => new
                {
                    GuildMemberId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MemberName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    IsBanned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    MemberId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMembers", x => x.GuildMemberId);
                    table.ForeignKey(
                        name: "FK_GuildMembers_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildRoles",
                columns: table => new
                {
                    GuildRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    RoleColorRed = table.Column<uint>(type: "int unsigned", nullable: false),
                    RoleColorBlue = table.Column<uint>(type: "int unsigned", nullable: false),
                    RoleColorGreen = table.Column<uint>(type: "int unsigned", nullable: false),
                    RoleColorAlpha = table.Column<uint>(type: "int unsigned", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRoles", x => x.GuildRoleId);
                    table.ForeignKey(
                        name: "FK_GuildRoles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildChannelRoles",
                columns: table => new
                {
                    GuildChannelRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GuildChannelId = table.Column<Guid>(type: "char(36)", nullable: false),
                    GuildRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannelRoles", x => x.GuildChannelRoleId);
                    table.ForeignKey(
                        name: "FK_GuildChannelRoles_GuildChannels_GuildChannelId",
                        column: x => x.GuildChannelId,
                        principalTable: "GuildChannels",
                        principalColumn: "GuildChannelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildChannelRoles_GuildRoles_GuildRoleId",
                        column: x => x.GuildRoleId,
                        principalTable: "GuildRoles",
                        principalColumn: "GuildRoleId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuildMemberRoles",
                columns: table => new
                {
                    GuildMemberRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    GuildMemberId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    GuildRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMemberRoles", x => x.GuildMemberRoleId);
                    table.ForeignKey(
                        name: "FK_GuildMemberRoles_GuildMembers_GuildMemberRoleId",
                        column: x => x.GuildMemberRoleId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildMemberRoles_GuildRoles_GuildRoleId",
                        column: x => x.GuildRoleId,
                        principalTable: "GuildRoles",
                        principalColumn: "GuildRoleId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannelRoles_GuildChannelId",
                table: "GuildChannelRoles",
                column: "GuildChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannelRoles_GuildRoleId",
                table: "GuildChannelRoles",
                column: "GuildRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChannels_GuildId",
                table: "GuildChannels",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberRoles_GuildRoleId",
                table: "GuildMemberRoles",
                column: "GuildRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_GuildId",
                table: "GuildMembers",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_IsBanned",
                table: "GuildMembers",
                column: "IsBanned");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_MemberId",
                table: "GuildMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoles_GuildId",
                table: "GuildRoles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_OwnMemberId",
                table: "Guilds",
                column: "OwnMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildChannelRoles");

            migrationBuilder.DropTable(
                name: "GuildMemberRoles");

            migrationBuilder.DropTable(
                name: "GuildChannels");

            migrationBuilder.DropTable(
                name: "GuildMembers");

            migrationBuilder.DropTable(
                name: "GuildRoles");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Shares",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Members",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "Members",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "LoginId",
                table: "Members",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Members",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256);
        }
    }
}
