using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0fd80603-4832-4330-8258-2167046b8132", null, "admin", "admin" },
                    { "c7a00174-8d67-40f6-a46c-35faa3005ea2", null, "staff", "staff" },
                    { "e5c0ee6e-c6c0-4cf2-82a6-d1df9da5bff3", null, "parent", "parent" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0fd80603-4832-4330-8258-2167046b8132");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7a00174-8d67-40f6-a46c-35faa3005ea2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5c0ee6e-c6c0-4cf2-82a6-d1df9da5bff3");
        }
    }
}
