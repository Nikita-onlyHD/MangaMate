using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaMate.Database.Models
{
    class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Release {  get; set; } = null!;

        public int BookTypeId { get; set; } 

        public BookType BookType { get; set; } = null!;

        public int BookStateId { get; set; }

        public BookState BookState {  get; set; } = null!;

        public ICollection<Genre>? Genres { get; set; }

        public ICollection<BookGenre>? BookGenres { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
