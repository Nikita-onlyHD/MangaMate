using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaMate.Migrations
{
    /// <inheritdoc />
    public partial class AddDicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "book_states",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Онгоинг" },
                    { 2, "Анонс" },
                    { 3, "Выпуск прекращен" },
                    { 4, "Завершён" },
                    { 5, "Приостоновлен" }
                });

            migrationBuilder.InsertData(
                table: "book_types",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Манга" },
                    { 2, "Книга" }
                });

            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Боевик" },
                    { 2, "Сёнэн" },
                    { 3, "Экшен" },
                    { 4, "Приключения" },
                    { 5, "Фэнтези" },
                    { 6, "Фантастика" },
                    { 7, "Романтика" },
                    { 8, "Комедия" },
                    { 9, "Драма" },
                    { 10, "Повседневность" },
                    { 11, "Мистика" },
                    { 12, "Триллер" },
                    { 13, "Детектив" },
                    { 14, "Исекай" },
                    { 15, "Психология" },
                    { 16, "Ужасы" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "book_states",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "book_states",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "book_states",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "book_states",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "book_states",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "book_types",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "book_types",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "genres",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
