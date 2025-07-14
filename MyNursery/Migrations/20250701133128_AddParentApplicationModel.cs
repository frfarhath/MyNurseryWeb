using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddParentApplicationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the lines below if they still try to delete existing roles.
            // Commented out to avoid touching existing roles.

            /*
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0c2026d-790f-4a00-be43-54b5ccb30516");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cb2a9e42-6f0d-42cd-9a14-f1e319a52f8a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db0696d8-7123-4d95-bb1e-e1c000cdd8a6");
            */

            migrationBuilder.CreateTable(
                name: "ParentApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WedMorning = table.Column<bool>(type: "bit", nullable: false),
                    WedAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    WedFull = table.Column<bool>(type: "bit", nullable: false),
                    ThuMorning = table.Column<bool>(type: "bit", nullable: false),
                    ThuAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    ThuFull = table.Column<bool>(type: "bit", nullable: false),
                    FriMorning = table.Column<bool>(type: "bit", nullable: false),
                    FriAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    FriFull = table.Column<bool>(type: "bit", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsentPhotos = table.Column<bool>(type: "bit", nullable: false),
                    ConsentTrips = table.Column<bool>(type: "bit", nullable: false),
                    ConsentEmergency = table.Column<bool>(type: "bit", nullable: false),
                    DeclarationAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentApplications", x => x.Id);
                });

            // Removed: InsertData for AspNetRoles to prevent duplicate key errors
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentApplications");

            // You can also remove these if the roles were not inserted in the Up() method
            /*
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84d634f8-7803-473d-a83d-9e197afcd982");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2a05247-7288-434e-b88d-756c93b1f932");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d98fa01f-622f-42ba-a64e-52498a2f583b");
            */
        }
    }
}
