using MangaMate.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MangaMate.Database
{
    class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) 
        {
            Database.EnsureCreated();
        }

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

            builder.Entity<Book>().ToTable("books");
            builder.Entity<Book>().HasKey(e => e.Id);
            builder.Entity<Book>().HasIndex(e => e.Title).IsUnique();

            builder.Entity<UserBook>().ToTable("user_books");
            builder.Entity<UserBook>().HasKey(e => new { e.User, e.Book });

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
                            bookGenre.HasKey(e => new { e.BookFK, e.GenreFK });
                        });
        }
    }
}
