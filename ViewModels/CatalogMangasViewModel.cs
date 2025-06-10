namespace MangaMate.ViewModels
{
    using MangaMate.Database.Models;
    using MangaMate.Repository;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public sealed class CatalogMangasViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        public ObservableCollection<Book> Mangas { get; } = [];

        #region Фильтры
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); Reload(); }
        }
        private string _searchText = string.Empty;

        public int? SelectedYear
        {
            get => _year; set { _year = value; OnPropertyChanged(nameof(SelectedYear)); Reload(); }
        }
        private int? _year;

        public BookState? SelectedState
        {
            get => _state; set { _state = value; OnPropertyChanged(nameof(SelectedState)); Reload(); }
        }
        private BookState? _state;

        public List<Genre> AllGenres { get; } = [];
        public ObservableCollection<Genre> SelectedGenres { get; } = [];
        #endregion

        public ICommand OpenDetailCommand { get; }

        public CatalogMangasViewModel()
        {
            OpenDetailCommand = new Command(b =>
                Mediator.Instance.Notify("OpenMangaDetail", b));

            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            // жанры/статусы разово
            var token = _cts.Token;
            AllGenres.Clear();
            AllGenres.AddRange(await BookRepository.GetAllGenresAsync(token));

            await Reload();
        }

        private async Task Reload()
        {
            var token = _cts.Token;
            var books = await BookRepository.GetMangaCatalogAsync(
                token,
                SearchText,
                SelectedYear,
                SelectedState?.Id,
                SelectedGenres.Select(g => g.Id).ToArray());

            Mangas.Clear();
            foreach (var b in books) Mangas.Add(b);
        }
    }

}