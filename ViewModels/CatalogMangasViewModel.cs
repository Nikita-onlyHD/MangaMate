using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using MangaMate.Database.Models;
using MangaMate.Repository;

namespace MangaMate.ViewModels
{
    public class CatalogMangasViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        #region Properties

        private ObservableCollection<Book> _mangas = new();
        public ObservableCollection<Book> Mangas
        {
            get => _mangas;
            set
            {
                _mangas = value;
                OnPropertyChanged(nameof(Mangas));
            }
        }

        private ICollectionView _mangasView;
        public ICollectionView MangasView
        {
            get => _mangasView;
            set
            {
                _mangasView = value;
                OnPropertyChanged(nameof(MangasView));
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }

        private int? _selectedReleaseYearFrom;
        public int? SelectedReleaseYearFrom
        {
            get => _selectedReleaseYearFrom;
            set
            {
                _selectedReleaseYearFrom = value;
                OnPropertyChanged(nameof(SelectedReleaseYearFrom));
                ApplyFilters();
            }
        }

        private int? _selectedReleaseYearTo;
        public int? SelectedReleaseYearTo
        {
            get => _selectedReleaseYearTo;
            set
            {
                _selectedReleaseYearTo = value;
                OnPropertyChanged(nameof(SelectedReleaseYearTo));
                ApplyFilters();
            }
        }

        private double? _selectedRatingFrom;
        public double? SelectedRatingFrom
        {
            get => _selectedRatingFrom;
            set
            {
                _selectedRatingFrom = value;
                OnPropertyChanged(nameof(SelectedRatingFrom));
                ApplyFilters();
            }
        }

        private double? _selectedRatingTo;
        public double? SelectedRatingTo
        {
            get => _selectedRatingTo;
            set
            {
                _selectedRatingTo = value;
                OnPropertyChanged(nameof(SelectedRatingTo));
                ApplyFilters();
            }
        }

        private ObservableCollection<BookState> _bookStates = new();
        public ObservableCollection<BookState> BookStates
        {
            get => _bookStates;
            set
            {
                _bookStates = value;
                OnPropertyChanged(nameof(BookStates));
            }
        }

        private BookState? _selectedBookState;
        public BookState? SelectedBookState
        {
            get => _selectedBookState;
            set
            {
                _selectedBookState = value;
                OnPropertyChanged(nameof(SelectedBookState));
                ApplyFilters();
            }
        }

        private ObservableCollection<Genre> _genres = new();
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        private ObservableCollection<Genre> _selectedGenres = new();
        public ObservableCollection<Genre> SelectedGenres
        {
            get => _selectedGenres;
            set
            {
                _selectedGenres = value;
                OnPropertyChanged(nameof(SelectedGenres));
                ApplyFilters();
            }
        }

        private Book? _selectedManga;
        public Book? SelectedManga
        {
            get => _selectedManga;
            set
            {
                _selectedManga = value;
                OnPropertyChanged(nameof(SelectedManga));
            }
        }

        #endregion

        #region Commands

        public ICommand OpenMangaDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ToggleGenreCommand { get; }

        #endregion

        public event Action<Book>? MangaSelected;

        public CatalogMangasViewModel()
        {
            OpenMangaDetailsCommand = new Command(ExecuteOpenMangaDetails, CanExecuteOpenMangaDetails);
            ClearFiltersCommand = new Command(ExecuteClearFilters, (_) => true);
            ToggleGenreCommand = new Command(ExecuteToggleGenre, (_) => true);

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var mangas = await BookRepository.GetAllBooksAsync(_cts.Token);
                var bookStates = await BookRepository.GetAllBookStatesAsync(_cts.Token);
                var genres = await BookRepository.GetAllGenresAsync(_cts.Token);

                Mangas = new ObservableCollection<Book>(mangas);
                BookStates = new ObservableCollection<BookState>(bookStates);
                Genres = new ObservableCollection<Genre>(genres);

                MangasView = CollectionViewSource.GetDefaultView(Mangas);
                MangasView.Filter = FilterMangas;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private bool FilterMangas(object item)
        {
            if (item is not Book manga) return false;

            // Фильтр по названию
            if (!string.IsNullOrEmpty(SearchText) && 
                !manga.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;

            // Фильтр по году релиза
            if (int.TryParse(manga.Release, out int releaseYear))
            {
                if (SelectedReleaseYearFrom.HasValue && releaseYear < SelectedReleaseYearFrom.Value)
                    return false;
                if (SelectedReleaseYearTo.HasValue && releaseYear > SelectedReleaseYearTo.Value)
                    return false;
            }

            // Фильтр по статусу
            if (SelectedBookState != null && manga.BookStateId != SelectedBookState.Id)
                return false;

            // Фильтр по жанрам
            if (SelectedGenres.Any())
            {
                var mangaGenreIds = manga.Genres?.Select(g => g.Id) ?? Enumerable.Empty<int>();
                var selectedGenreIds = SelectedGenres.Select(g => g.Id);
                if (!selectedGenreIds.All(id => mangaGenreIds.Contains(id)))
                    return false;
            }

            return true;
        }

        private void ApplyFilters()
        {
            MangasView?.Refresh();
        }

        private bool CanExecuteOpenMangaDetails(object? parameter)
        {
            return parameter is Book || SelectedManga != null;
        }

        private void ExecuteOpenMangaDetails(object? parameter)
        {
            var manga = parameter as Book ?? SelectedManga;
            if (manga != null)
            {
                MangaSelected?.Invoke(manga);
            }
        }

        private void ExecuteClearFilters(object? parameter)
        {
            SearchText = string.Empty;
            SelectedReleaseYearFrom = null;
            SelectedReleaseYearTo = null;
            SelectedRatingFrom = null;
            SelectedRatingTo = null;
            SelectedBookState = null;
            SelectedGenres.Clear();
        }

        private void ExecuteToggleGenre(object? parameter)
        {
            if (parameter is Genre genre)
            {
                if (SelectedGenres.Contains(genre))
                    SelectedGenres.Remove(genre);
                else
                    SelectedGenres.Add(genre);
            }
        }
    }
}