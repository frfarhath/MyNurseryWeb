using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "105892f9-9466-43b8-b505-912afcb1ade9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6712d157-3ffb-4422-aa04-3e383bd413df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93569171-dbdf-4fb7-b6d2-444dabb27176");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8450c384-7ea5-4d53-8542-69351e6e80f5", null, "Staff", "STAFF" },
                    { "a21c919d-e7a2-4d99-9b67-1d69c2a960ae", null, "Admin", "ADMIN" },
                    { "b9cf38fa-504d-498a-af2d-c6ddf57bb1a7", null, "Parent", "PARENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8450c384-7ea5-4d53-8542-69351e6e80f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a21c919d-e7a2-4d99-9b67-1d69c2a960ae");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9cf38fa-504d-498a-af2d-c6ddf57bb1a7");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "105892f9-9466-43b8-b505-912afcb1ade9", null, "Admin", "ADMIN" },
                    { "6712d157-3ffb-4422-aa04-3e383bd413df", null, "Staff", "STAFF" },
                    { "93569171-dbdf-4fb7-b6d2-444dabb27176", null, "Parent", "PARENT" }
                });
        }
    }
}
