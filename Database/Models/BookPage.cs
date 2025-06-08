using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaMate.Database.Models
{
    public class BookPage
    {
        public int Id { get; set; }

        public int Chapter {  get; set; }

        public int Page { get; set; }

        public byte[] Data { get; set; } = null!;

        public int BookId { get; set; }

        public Book Book { get; set; } = null!;
    }
}
