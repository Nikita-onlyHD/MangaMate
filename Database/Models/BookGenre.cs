namespace MangaMate.Database.Models
{
    class BookGenre
    {
        public int BookId { get; set; }

        public Book? BookFK { get; set; }

        public int GenreId { get; set; }

        public Genre? GenreFK { get; set; }
    }
}
