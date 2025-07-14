using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "ParentApplications");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bf40b68-8718-40bb-9b75-01730067366e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "674a7a87-d2db-4fbb-b50a-ddc98b126879");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5f91144-3a9a-4146-b38f-94045282ea87");

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionalImage1Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionalImage2Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");

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

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParentApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChildAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChildFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildGender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsentEmergency = table.Column<bool>(type: "bit", nullable: false),
                    ConsentPhotos = table.Column<bool>(type: "bit", nullable: false),
                    ConsentTrips = table.Column<bool>(type: "bit", nullable: false),
                    DeclarationAccepted = table.Column<bool>(type: "bit", nullable: false),
                    FriAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    FriFull = table.Column<bool>(type: "bit", nullable: false),
                    FriMorning = table.Column<bool>(type: "bit", nullable: false),
                    FundingOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicalConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    MonFull = table.Column<bool>(type: "bit", nullable: false),
                    MonMorning = table.Column<bool>(type: "bit", nullable: false),
                    Parent1Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent1FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent1Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent1Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent2Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent2FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent2Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent2Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThuAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    ThuFull = table.Column<bool>(type: "bit", nullable: false),
                    ThuMorning = table.Column<bool>(type: "bit", nullable: false),
                    TueAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    TueFull = table.Column<bool>(type: "bit", nullable: false),
                    TueMorning = table.Column<bool>(type: "bit", nullable: false),
                    WedAfternoon = table.Column<bool>(type: "bit", nullable: false),
                    WedFull = table.Column<bool>(type: "bit", nullable: false),
                    WedMorning = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentApplications", x => x.Id);
                });

        }
    }
}
