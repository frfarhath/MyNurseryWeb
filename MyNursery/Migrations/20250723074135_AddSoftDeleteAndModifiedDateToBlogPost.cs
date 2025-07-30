using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteAndModifiedDateToBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "BlogPosts");
        }
    }
}
