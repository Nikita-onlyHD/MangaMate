namespace MangaMate.Database.Models
{
    public sealed class BookType
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Book>? Books { get; set; }
    }
}
