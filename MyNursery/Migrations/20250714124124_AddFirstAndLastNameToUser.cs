using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstAndLastNameToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bede1fd9-9837-40aa-b479-ebec457f060c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2bbe639-9990-4416-9326-a7f396c78fff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e49184e8-b931-41b5-8a7c-b7449fbc9b1c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5edc6d2c-55aa-4148-90d3-55b15c4522fc", null, "Parent", "PARENT" },
                    { "9bdb3758-fbfd-44af-9a5a-559c88e4acc8", null, "Staff", "STAFF" },
                    { "a35cd4a6-2ef5-4099-82a3-2976a4078e40", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5edc6d2c-55aa-4148-90d3-55b15c4522fc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9bdb3758-fbfd-44af-9a5a-559c88e4acc8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a35cd4a6-2ef5-4099-82a3-2976a4078e40");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "bede1fd9-9837-40aa-b479-ebec457f060c", null, "Staff", "STAFF" },
                    { "c2bbe639-9990-4416-9326-a7f396c78fff", null, "Parent", "PARENT" },
                    { "e49184e8-b931-41b5-8a7c-b7449fbc9b1c", null, "Admin", "ADMIN" }
                });
        }
    }
}
