using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNursery.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlogPosts");
        }
    }
}
