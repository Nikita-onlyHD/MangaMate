namespace MangaMate.Database.Models
{
    public class Genre
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Book>? Books { get; set; }

        public ICollection<BookGenre>? BookGenres { get; set; }
    }
}
