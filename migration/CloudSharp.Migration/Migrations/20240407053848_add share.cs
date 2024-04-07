using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSharp.Migraition.Migrations
{
    /// <inheritdoc />
    public partial class addshare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shares",
                columns: table => new
                {
                    ShareId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MemberId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FilePath = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: true),
                    ExpireTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shares", x => x.ShareId);
                    table.ForeignKey(
                        name: "FK_Shares_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_MemberId",
                table: "Shares",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shares");
        }
    }
}
