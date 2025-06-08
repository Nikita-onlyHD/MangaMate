using MangaMate.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MangaMate.Database;

class Context(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<BookType> BookTypes { get; set; }
    public DbSet<BookState> BookStates { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<UserBook> UsersBooks { get; set; }
    public DbSet<UserState> UsersStates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<BookType>().ToTable("book_types");
        builder.Entity<BookType>().HasKey(e => e.Id);
        builder.Entity<BookType>().HasIndex(e => e.Name).IsUnique();

        builder.Entity<BookState>().ToTable("book_states");
        builder.Entity<BookState>().HasKey(e => e.Id);
        builder.Entity<BookState>().HasIndex(e => e.Name).IsUnique();

        builder.Entity<UserState>().ToTable("user_states");
        builder.Entity<UserState>().HasKey(e => e.Id);
        builder.Entity<UserState>().HasIndex(e => e.Name).IsUnique();

        builder.Entity<Genre>().ToTable("genres");
        builder.Entity<Genre>().HasKey(e => e.Id);
        builder.Entity<Genre>().HasIndex(e => e.Name).IsUnique();

        builder.Entity<User>().ToTable("users");
        builder.Entity<User>().HasKey(e => e.Id);
        builder.Entity<User>().HasIndex(e => e.Login).IsUnique();
        builder.Entity<User>().Property(e => e.Role).HasConversion<string>();
        builder.Entity<User>().HasData(new User()
        {
            Id = 1,
            Login = "admin",
            Password = "admin",
            Email = "admin",
            Role = UserRole.Admin,
        });

        builder.Entity<Book>().ToTable("books");
        builder.Entity<Book>().HasKey(e => e.Id);
        builder.Entity<Book>().HasIndex(e => e.Title).IsUnique();

        builder.Entity<BookPage>().ToTable("book_pages");
        builder.Entity<BookPage>().HasKey(e => e.Id);

        builder.Entity<BookPage>()
            .HasOne(bp => bp.Book)
            .WithMany(b => b.BookPages)
            .HasForeignKey(bp => bp.BookId);

        builder.Entity<UserState>()
            .HasMany(e => e.UserBooks)
            .WithOne(e => e.UserState)
            .HasForeignKey(e => e.StateId);

        builder.Entity<User>()
            .HasMany(e => e.Books)
            .WithMany(e => e.Users)
            .UsingEntity<UserBook>();

        builder.Entity<BookType>()
            .HasMany(e => e.Books)
            .WithOne(e => e.BookType)
            .HasForeignKey(e => e.BookTypeId)
            .IsRequired();

        builder.Entity<BookState>()
            .HasMany(e => e.Books)
            .WithOne(e => e.BookState)
            .HasForeignKey(e => e.BookStateId)
            .IsRequired();

        builder.Entity<Genre>()
            .HasMany(genre => genre.Books)
            .WithMany(book => book.Genres)
            .UsingEntity<BookGenre>(
                bookGenre => bookGenre
                    .HasOne(bookGenre => bookGenre.BookFK)
                    .WithMany(book => book.BookGenres)
                    .HasForeignKey(bookGenre => bookGenre.BookId),
                bookGenre => bookGenre
                    .HasOne(bookGenre => bookGenre.GenreFK)
                    .WithMany(genre => genre.BookGenres)
                    .HasForeignKey(bookGenre => bookGenre.GenreId),
                bookGenre =>
                    {
                        bookGenre.ToTable("books_genres");
                    });

        builder.Entity<User>()
            .HasMany(user => user.Books)
            .WithMany(book => book.Users)
            .UsingEntity<UserBook>(
                userBook => userBook
                    .HasOne(userBook => userBook.BookFK)
                    .WithMany(book => book.UserBooks)
                    .HasForeignKey(userBook => userBook.BookId),
                userBook => userBook
                    .HasOne(userBook => userBook.UserFK)
                    .WithMany(user => user.UserBooks)
                    .HasForeignKey(userBook => userBook.UserId),
                bookGenre =>
                {
                    bookGenre.ToTable("users_books");
                });
    }
}
