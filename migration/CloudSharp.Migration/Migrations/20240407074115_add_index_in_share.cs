using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudSharp.Migraition.Migrations
{
    /// <inheritdoc />
    public partial class add_index_in_share : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Shares",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_FilePath",
                table: "Shares",
                column: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shares_FilePath",
                table: "Shares");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Shares",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");
        }
    }
}
