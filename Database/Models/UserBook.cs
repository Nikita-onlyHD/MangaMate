﻿namespace MangaMate.Database.Models
{
    public class UserBook
    {
        public int UserId { get; set; }

        public User UserFK { get; set; } = null!;

        public int BookId { get; set; }

        public Book BookFK { get; set; } = null!;

        public int StateId { get; set; }

        public UserState? UserState { get; set; }

        public int Assessment {  get; set; }

        public bool Favourite { get; set; }

        public int CurrentChapter { get; set; }
    }
}
