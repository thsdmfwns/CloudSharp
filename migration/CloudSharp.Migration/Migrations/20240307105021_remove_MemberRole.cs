using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CloudSharp.Migraition.Migrations
{
    /// <inheritdoc />
    public partial class remove_MemberRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MemberRoles_RoleId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MemberRoles");

            migrationBuilder.DropIndex(
                name: "IX_Members_RoleId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "RoleId",
                table: "Members",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "MemberRoles",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "MemberRoles",
                columns: new[] { "Id", "CreatedOn", "Name", "UpdatedOn" },
                values: new object[,]
                {
                    { 1ul, new DateTime(2024, 2, 28, 12, 43, 7, 876, DateTimeKind.Utc).AddTicks(7465), "admin", null },
                    { 2ul, new DateTime(2024, 2, 28, 12, 43, 7, 876, DateTimeKind.Utc).AddTicks(7474), "member", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_RoleId",
                table: "Members",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MemberRoles_RoleId",
                table: "Members",
                column: "RoleId",
                principalTable: "MemberRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
