using System.ComponentModel.DataAnnotations.Schema;

namespace MangaMate.Database.Models;

class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Release {  get; set; } = null!;

    [ForeignKey("BookType")]
    public int BookTypeId { get; set; } 

    public BookType BookType { get; set; } = null!;

    [ForeignKey("BookState")]
    public int BookStateId { get; set; }

    public BookState BookState {  get; set; } = null!;

    public ICollection<Genre>? Genres { get; set; }

    public ICollection<BookGenre>? BookGenres { get; set; }

    public ICollection<User>? Users { get; set; }

    public ICollection<UserBook>? UserBooks { get; set; }
}
