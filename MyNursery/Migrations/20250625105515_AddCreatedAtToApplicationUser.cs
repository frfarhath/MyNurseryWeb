using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1950b985-20ce-4c21-ab5e-d61033a9fb22", null, "admin", "admin" },
                    { "debe1e37-b0a4-44f7-b4b5-5e0367c0b03c", null, "staff", "staff" },
                    { "eb3b5bee-6a5b-44ee-9f19-beb222e830fe", null, "parent", "parent" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1950b985-20ce-4c21-ab5e-d61033a9fb22");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "debe1e37-b0a4-44f7-b4b5-5e0367c0b03c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb3b5bee-6a5b-44ee-9f19-beb222e830fe");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

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
    }
}
