namespace MangaMate.Database.Models
{
    class UserBook
    {
        public User User { get; set; } = null!;

        public Book Book { get; set; } = null!;

        public int StateId { get; set; }

        public UserState? UserState { get; set; }

        public int Assessment {  get; set; }

        public bool Favourite { get; set; }
    }
}
