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

        #region Manga catalog & chapters

        /// <summary>
        /// Каталог манги с поиском/фильтрами.
        /// </summary>
        public static async Task<List<Book>> GetMangaCatalogAsync(
            CancellationToken token,
            string? search = null,
            int? releaseYear = null,
            int? statusId = null,
            IEnumerable<int>? genreIds = null,
            double? ratingFrom = null,
            double? ratingTo = null)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            // базовый запрос – только «Манга»
            var q = ctx.Books
                .Include(b => b.BookState)
                .Include(b => b.Genres)
                .Include(b => b.BookType)
                .Where(b => b.BookType.Name == "Манга")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(b => EF.Functions.ILike(b.Title, $"%{search}%"));

            if (releaseYear is not null)
                q = q.Where(b => b.Release == releaseYear.ToString()); // Release хранится строкой

            if (statusId is not null)
                q = q.Where(b => b.BookStateId == statusId);

            if (genreIds?.Any() == true)
                q = q.Where(b => b.BookGenres!.Any(bg => genreIds.Contains(bg.GenreId)));

            // средний рейтинг по UserBooks
            if (ratingFrom is not null || ratingTo is not null)
{
    q = q.Where(b =>
        (ratingFrom == null || 
            ctx.UsersBooks.Where(ub => ub.BookId == b.Id)
                          .Select(ub => (double?)ub.Assessment)
                          .Average() >= ratingFrom)
        &&
        (ratingTo == null || 
            ctx.UsersBooks.Where(ub => ub.BookId == b.Id)
                          .Select(ub => (double?)ub.Assessment)
                          .Average() <= ratingTo)
    );
}

            return await q.OrderBy(b => b.Title).ToListAsync(token);
        }

        /// <summary>Список глав книги.</summary>
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

        /// <summary>Страницы конкретной главы (отсортированы).</summary>
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

        /// <summary>Удаляет главу целиком.</summary>
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

        /// <summary>
        /// Добавление главы из *.cbr (RAR) архива. 
        /// Требует пакета SharpCompress.
        /// </summary>
        public static async Task AddChapterFromCbrAsync(
            int bookId, int chapter, string filePath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await using var ctx = new ContextFactory().CreateDbContext([]);

            using var archive = SharpCompress.Archives.Rar.RarArchive.Open(filePath);

            var pages = new List<BookPage>();

            foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
            {
                // имя файла – число страницы, например 001.jpg
                var name = Path.GetFileNameWithoutExtension(entry.Key);
                if (!int.TryParse(name, out var pageNumber))
                    continue;

                await using var ms = new MemoryStream();
                await using var entryStream = entry.OpenEntryStream();
                await entryStream.CopyToAsync(ms, token);

                pages.Add(new BookPage
                {
                    BookId = bookId,
                    Chapter = chapter,
                    Page = pageNumber,
                    Data = ms.ToArray()
                });
            }

            if (pages.Count == 0)
                throw new InvalidOperationException("В архиве не найдено изображений страниц.");

            // на всякий случай удаляем существующую главу, если была
            var existing = ctx.Set<BookPage>()
                .Where(p => p.BookId == bookId && p.Chapter == chapter);
            ctx.RemoveRange(existing);

            await ctx.Set<BookPage>().AddRangeAsync(pages, token);
            await ctx.SaveChangesAsync(token);
        }

        #endregion

    }
}