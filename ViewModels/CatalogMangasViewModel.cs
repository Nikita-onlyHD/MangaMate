using MangaMate.Database.Models;
using MangaMate.Repository;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MangaMate.ViewModels
{
    public class CatalogMangasViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        private ObservableCollection<Book> _mangas;
        public ObservableCollection<Book> Mangas
        {
            get => _mangas;
            set
            {
                _mangas = value;
                OnPropertyChanged(nameof(Mangas));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterMangas();
            }
        }

        private List<BookState> _bookStates;
        public List<BookState> BookStates
        {
            get => _bookStates;
            set
            {
                _bookStates = value;
                OnPropertyChanged(nameof(BookStates));
            }
        }

        private List<Genre> _genres;
        public List<Genre> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        private BookState _selectedBookState;
        public BookState SelectedBookState
        {
            get => _selectedBookState;
            set
            {
                _selectedBookState = value;
                OnPropertyChanged(nameof(SelectedBookState));
                FilterMangas();
            }
        }

        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                _selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
                FilterMangas();
            }
        }

        private int? _selectedYear;
        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                FilterMangas();
            }
        }

        private double? _minRating;
        public double? MinRating
        {
            get => _minRating;
            set
            {
                _minRating = value;
                OnPropertyChanged(nameof(MinRating));
                FilterMangas();
            }
        }

        public ICommand OpenMangaCommand { get; }

        public CatalogMangasViewModel()
        {
            Mangas = new ObservableCollection<Book>();
            BookStates = new List<BookState>();
            Genres = new List<Genre>();

            OpenMangaCommand = new Command(ExecuteOpenMangaCommand);

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var bookStates = await BookRepository.GetAllBookStatesAsync(_cts.Token);
                var genres = await BookRepository.GetAllGenresAsync(_cts.Token);
                var books = await BookRepository.GetAllBooksAsync(_cts.Token);

                var mangas = books.Where(b => b.BookType.Name == "Манга").ToList();

                BookStates = bookStates;
                Genres = genres;
                Mangas = new ObservableCollection<Book>(mangas);
            }
            catch (OperationCanceledException)
            {
                // Operation was canceled
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        private void FilterMangas()
        {
            var view = CollectionViewSource.GetDefaultView(Mangas);
            if (view == null) return;

            view.Filter = obj =>
            {
                if (obj is not Book manga) return false;

                // Search by title
                if (!string.IsNullOrEmpty(SearchText) &&
                    !manga.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Filter by state
                if (SelectedBookState != null && manga.BookState.Id != SelectedBookState.Id)
                {
                    return false;
                }

                // Filter by genre
                if (SelectedGenre != null &&
                    (manga.Genres == null || !manga.Genres.Any(g => g.Id == SelectedGenre.Id)))
                {
                    return false;
                }

                // Filter by year
                if (SelectedYear.HasValue)
                {
                    if (!int.TryParse(manga.Release, out var year) || year != SelectedYear.Value)
                    {
                        return false;
                    }
                }

                // Filter by rating (assuming average rating is stored somewhere)
                // You'll need to implement this based on your actual data model
                if (MinRating.HasValue)
                {
                    // Placeholder - implement actual rating logic
                    var mangaRating = GetMangaRating(manga.Id);
                    if (mangaRating < MinRating.Value)
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        private double GetMangaRating(int mangaId)
        {
            // Implement actual rating calculation based on your data model
            // This is just a placeholder
            return 4.5; // Example rating
        }

        private void ExecuteOpenMangaCommand(object? obj)
        {
            if (obj is Book manga)
            {
                Mediator.Instance.Notify("OpenMangaDetails", manga);
            }
        }
    }
}
