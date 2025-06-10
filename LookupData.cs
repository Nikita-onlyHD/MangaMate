using MangaMate.Database.Models;
using MangaMate.Repository;

namespace MangaMate
{
    public static class LookupData
    {
        private static IList<Genre>? _genres;
        private static IList<BookState>? _states;

        public static async Task<IList<Genre>> GetGenresAsync(CancellationToken token = default)
        {
            if (_genres is null)
                _genres = await BookRepository.GetAllGenresAsync(token);
            return _genres;
        }

        public static async Task<IList<BookState>> GetStatesAsync(CancellationToken token = default)
        {
            if (_states is null)
                _states = await BookRepository.GetAllBookStatesAsync(token);
            return _states;
        }

        public static void ClearCache()
        {
            _genres = null;
            _states = null;
        }
    }

}
