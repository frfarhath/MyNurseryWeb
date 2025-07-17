using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddNUUSDashboardIdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NUUSDashboardId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NUUSDashboardId",
                table: "AspNetUsers");
        }
    }
}
