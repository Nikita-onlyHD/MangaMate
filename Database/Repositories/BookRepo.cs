using MangaMate.Database;
using MangaMate.Database.Models;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

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

        public static async Task<List<int>> GetChaptersAsync(int bookId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            return await ctx.Set<BookPage>()
                .Where(p => p.BookId == bookId)
                .Select(p => p.Chapter)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync(token);
        }

        public static async Task DeleteChapterAsync(
            int bookId, int chapter, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            var pages = ctx.Set<BookPage>()
                .Where(p => p.BookId == bookId && p.Chapter == chapter);
            ctx.RemoveRange(pages);
            await ctx.SaveChangesAsync(token);
        }

        public static async Task AddChapterFromCbrAsync(
            int bookId, int chapter, string filePath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            IArchive archive;
            try
            {
                archive = SharpCompress.Archives.ArchiveFactory.Open(filePath);
            }
            catch (InvalidFormatException ex)
            {
                throw new InvalidOperationException("Файл не является корректным RAR-архивом", ex);
            }

            var pages = new List<BookPage>();

            foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(entry.Key);
                    if (!int.TryParse(name, out var pageNumber))
                        continue;

                    await using var ms = new MemoryStream();
                    await entry.OpenEntryStream().CopyToAsync(ms);
                    pages.Add(new BookPage
                    {
                        BookId = bookId,
                        Chapter = chapter,
                        Page = pageNumber,
                        Data = ms.ToArray()
                    });
                }
                catch (Exception ex)
                {
                    // Логируем ошибку для проблемной записи, но продолжаем обработку
                    Debug.WriteLine($"Ошибка обработки записи {entry.Key}: {ex.Message}");
                }
            }

            if (pages.Count == 0)
                throw new InvalidOperationException("В архиве не найдено изображений страниц.");

            var existing = ctx.Set<BookPage>()
                .Where(p => p.BookId == bookId && p.Chapter == chapter);
            ctx.RemoveRange(existing);

            await ctx.Set<BookPage>().AddRangeAsync(pages, token);
            await ctx.SaveChangesAsync(token);
        }

        public static async Task<List<BookPage>> GetPagesAsync(
            int bookId, int chapter, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            return await ctx.Set<BookPage>()
                .Where(p => p.BookId == bookId && p.Chapter == chapter)
                .OrderBy(p => p.Page)
                .ToListAsync(token);
        }
    }
}