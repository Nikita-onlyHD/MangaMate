using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaMate.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarForBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "books",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "books");
        }
    }
}
