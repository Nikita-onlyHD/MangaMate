namespace MangaMate.Database.Models
{
    public sealed class UserState
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<UserBook>? UserBooks { get; set; }
    }
}
