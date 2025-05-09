namespace MangaMate.Database.Models
{
    sealed class BookState
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Book>? Books { get; set; }
    }
}
