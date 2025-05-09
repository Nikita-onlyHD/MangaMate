namespace MangaMate.Database.Models
{
    sealed class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public ICollection<Book>? Books { get; set; }
    }
}
