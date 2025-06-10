using MangaMate.Database;
using MangaMate.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MangaMate.Repository
{
    public static class BookRepository
    {
        public static async Task<List<Book>> GetAllBooksAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.Books
                .Include(b => b.BookType)
                .Include(b => b.BookState)
                .Include(b => b.Genres)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public static async Task<Book?> GetBookByIdAsync(int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.Books
                .Include(b => b.BookType)
                .Include(b => b.BookState)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public static async Task<List<BookType>> GetAllBookTypesAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.BookTypes.OrderBy(bt => bt.Name).ToListAsync();
        }

        public static async Task<List<BookState>> GetAllBookStatesAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.BookStates.OrderBy(bs => bs.Name).ToListAsync();
        }

        public static async Task<List<Genre>> GetAllGenresAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public static async Task<Book> SaveBookAsync(Book book, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            if (book.Id == 0)
            {
                context.Books.Add(book);
            }
            else
            {
                context.Books.Update(book);
            }

            await context.SaveChangesAsync();
            return book;
        }

        public static async Task SaveBookGenresAsync(int bookId, List<int> genreIds, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            // Удаляем существующие связи
            var existingBookGenres = await context.Set<BookGenre>()
                .Where(bg => bg.BookId == bookId)
                .ToListAsync();

            context.Set<BookGenre>().RemoveRange(existingBookGenres);

            // Добавляем новые связи
            var newBookGenres = genreIds.Select(genreId => new BookGenre
            {
                BookId = bookId,
                GenreId = genreId
            }).ToList();

            context.Set<BookGenre>().AddRange(newBookGenres);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteBookAsync(int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            var book = await context.Books.FindAsync(id);
            if (book != null)
            {
                context.Books.Remove(book);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<bool> BookExistsAsync(string title, int? excludeId = null, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.Books
                .AnyAsync(b => b.Title.ToLower() == title.ToLower() &&
                         (excludeId == null || b.Id != excludeId));
        }

        // 

        public static async Task<List<Book>> GetMangasAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            return await context.Books
                .Include(b => b.BookType)
                .Include(b => b.BookState)
                .Include(b => b.Genres)
                .Where(b => b.BookType.Name == "Манга")
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public static async Task<List<int>> GetMangaReleaseYearsAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            var years = await context.Books
                .Where(b => b.BookType.Name == "Манга")
                .Select(b => b.Release)
                .Distinct()
                .ToListAsync();

            var result = new List<int>();
            foreach (var yearStr in years)
            {
                if (int.TryParse(yearStr, out var year))
                {
                    result.Add(year);
                }
            }

            return result.OrderBy(y => y).ToList();
        }

    }
}