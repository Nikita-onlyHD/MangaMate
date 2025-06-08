namespace MangaMate.Database.Models
{
    public enum UserRole
    {
        User,
        Admin
    }

    public sealed class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public byte[]? Avatar { get; set; }

        public UserRole Role { get; set; }

        public ICollection<Book>? Books { get; set; }

        public ICollection<UserBook>? UserBooks { get; set; }
    }
}
