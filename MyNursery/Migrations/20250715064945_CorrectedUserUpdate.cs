using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedUserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "18aae5fc-669e-4dc0-8884-a11a5f44eeec", null, "Parent", "PARENT" },
                    { "3becf2c1-09e7-471c-ba2b-d3f632eef0fd", null, "Admin", "ADMIN" },
                    { "e894a381-4532-48af-829f-67001bca029c", null, "Staff", "STAFF" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18aae5fc-669e-4dc0-8884-a11a5f44eeec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3becf2c1-09e7-471c-ba2b-d3f632eef0fd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e894a381-4532-48af-829f-67001bca029c");

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
    }
}
