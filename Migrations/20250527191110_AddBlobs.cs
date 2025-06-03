using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaMate.Migrations
{
    /// <inheritdoc />
    public partial class AddBlobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentChapter",
                table: "users_books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "users",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "books",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentChapter",
                table: "users_books");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "books");
        }
    }
}
