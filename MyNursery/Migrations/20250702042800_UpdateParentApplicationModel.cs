using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyNursery.Migrations
{
    public partial class UpdateParentApplicationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove previous deletions to avoid touching existing roles
            // migrationBuilder.DeleteData(...) statements removed

            migrationBuilder.AddColumn<string>(
                name: "ChildAddress",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChildGender",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FundingOption",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MonAfternoon",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MonFull",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MonMorning",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Parent1Email",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Parent1FullName",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Parent1Phone",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Parent1Relationship",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Parent2Email",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parent2FullName",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parent2Phone",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parent2Relationship",
                table: "ParentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreferredStartDate",
                table: "ParentApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "TueAfternoon",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TueFull",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TueMorning",
                table: "ParentApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ChildAddress", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "ChildGender", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "FundingOption", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "MonAfternoon", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "MonFull", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "MonMorning", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent1Email", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent1FullName", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent1Phone", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent1Relationship", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent2Email", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent2FullName", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent2Phone", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "Parent2Relationship", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "PreferredStartDate", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "TueAfternoon", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "TueFull", table: "ParentApplications");
            migrationBuilder.DropColumn(name: "TueMorning", table: "ParentApplications");

            // Do not insert roles again here either
        }
    }
}
