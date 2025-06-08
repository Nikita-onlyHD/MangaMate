using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MangaMate.Database.Models;
using MangaMate.Repository;
using System.Windows.Input;
using MangaMate.Database;

namespace MangaMate.ViewModels
{
    class BookEditViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        // Текущая книга
        private Book? _selectedBook;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _author = string.Empty;
        private string _release = string.Empty;
        private BookType? _selectedBookType;
        private BookState? _selectedBookState;
        private bool _isNewBook = true;
        private bool _isEditMode = false;

        // Коллекции
        public ObservableCollection<Book> Books { get; set; } = new();
        public ObservableCollection<BookType> BookTypes { get; set; } = new();
        public ObservableCollection<BookState> BookStates { get; set; } = new();
        public ObservableCollection<Genre> AvailableGenres { get; set; } = new();
        public ObservableCollection<GenreSelection> GenreSelections { get; set; } = new();

        // Команды
        public ICommand NewBookCommand { get; }
        public ICommand SaveBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public BookEditViewModel()
        {
            NewBookCommand = new Command(_ => CreateNewBook(), (_) => true);
            SaveBookCommand = new Command(async _ => await SaveBookAsync(_cts.Token), (_) => CanSaveBook());
            EditBookCommand = new Command( _ => EnableEditMode(), (_) => SelectedBook != null && !IsEditMode);
            CancelEditCommand = new Command( _ => CancelEdit(), (_) => IsEditMode);
            DeleteBookCommand = new Command(async _ => await DeleteBookAsync(_cts.Token), _ => SelectedBook != null && !IsEditMode);

            LoadDataAsync(_cts.Token);
        }

        // Свойства
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    LoadBookData();
                    OnPropertyChanged(nameof(IsBookSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public string Release
        {
            get => _release;
            set => SetProperty(ref _release, value);
        }

        public BookType? SelectedBookType
        {
            get => _selectedBookType;
            set => SetProperty(ref _selectedBookType, value);
        }

        public BookState? SelectedBookState
        {
            get => _selectedBookState;
            set => SetProperty(ref _selectedBookState, value);
        }

        public bool IsNewBook
        {
            get => _isNewBook;
            set
            {
                if (SetProperty(ref _isNewBook, value))
                {
                    OnPropertyChanged(nameof(FormTitle));
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    OnPropertyChanged(nameof(IsFormEnabled));
                    OnPropertyChanged(nameof(FormTitle));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsBookSelected => SelectedBook != null;
        public bool IsFormEnabled => IsNewBook || IsEditMode;
        public string FormTitle => IsNewBook ? "Создание новой книги" :
                                  IsEditMode ? "Редактирование книги" : "Просмотр книги";

        // Методы
        private async void LoadDataAsync(CancellationToken token)
        {
            try
            {
                var books = await BookRepository.GetAllBooksAsync(token);
                var bookTypes = await BookRepository.GetAllBookTypesAsync(token);
                var bookStates = await BookRepository.GetAllBookStatesAsync(token);
                var genres = await BookRepository.GetAllGenresAsync(token);

                Books.Clear();
                foreach (var book in books)
                    Books.Add(book);

                BookTypes.Clear();
                foreach (var bookType in bookTypes)
                    BookTypes.Add(bookType);

                BookStates.Clear();
                foreach (var bookState in bookStates)
                    BookStates.Add(bookState);

                AvailableGenres.Clear();
                GenreSelections.Clear();
                foreach (var genre in genres)
                {
                    AvailableGenres.Add(genre);
                    GenreSelections.Add(new GenreSelection { Genre = genre, IsSelected = false });
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadBookData()
        {
            if (SelectedBook == null)
            {
                ClearForm();
                return;
            }

            Title = SelectedBook.Title;
            Description = SelectedBook.Description;
            Author = SelectedBook.Author;
            Release = SelectedBook.Release;
            SelectedBookType = BookTypes.FirstOrDefault(bt => bt.Id == SelectedBook.BookTypeId);
            SelectedBookState = BookStates.FirstOrDefault(bs => bs.Id == SelectedBook.BookStateId);

            // Обновляем выбор жанров
            foreach (var genreSelection in GenreSelections)
            {
                genreSelection.IsSelected = SelectedBook.Genres?.Any(g => g.Id == genreSelection.Genre.Id) ?? false;
            }

            IsNewBook = false;
            IsEditMode = false;
        }

        private void CreateNewBook()
        {
            ClearForm();
            IsNewBook = true;
            IsEditMode = true;
        }

        private void ClearForm()
        {
            SelectedBook = null;
            Title = string.Empty;
            Description = string.Empty;
            Author = string.Empty;
            Release = string.Empty;
            SelectedBookType = null;
            SelectedBookState = null;

            foreach (var genreSelection in GenreSelections)
                genreSelection.IsSelected = false;
        }

        private void EnableEditMode()
        {
            IsEditMode = true;
        }

        private void CancelEdit()
        {
            if (IsNewBook)
            {
                ClearForm();
                IsNewBook = false;
            }
            else
            {
                LoadBookData();
            }
            IsEditMode = false;
        }

        private async Task SaveBookAsync(CancellationToken token)
        {
            try
            {
                // Проверка на существование книги с таким названием
                var bookExists = await BookRepository.BookExistsAsync(Title, excludeId: SelectedBook?.Id, token);
                if (bookExists)
                {
                    // Здесь можно показать сообщение пользователю
                    System.Windows.MessageBox.Show("Книга с таким названием уже существует!",
                        "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                var book = SelectedBook ?? new Book();
                book.Title = Title;
                book.Description = Description;
                book.Author = Author;
                book.Release = Release;
                book.BookTypeId = SelectedBookType?.Id ?? 0;
                book.BookStateId = SelectedBookState?.Id ?? 0;

                var savedBook = await BookRepository.SaveBookAsync(book, token);

                // Сохраняем жанры
                var selectedGenreIds = GenreSelections
                    .Where(gs => gs.IsSelected)
                    .Select(gs => gs.Genre.Id)
                    .ToList();

                await BookRepository.SaveBookGenresAsync(savedBook.Id, selectedGenreIds, token);

                // Обновляем список книг
                await RefreshBooksAsync(token);

                // Находим и выбираем сохраненную книгу
                SelectedBook = Books.FirstOrDefault(b => b.Id == savedBook.Id);
                IsNewBook = false;
                IsEditMode = false;

                System.Windows.MessageBox.Show("Книга успешно сохранена!",
                    "Успех", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при сохранении книги: {ex.Message}",
                    "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task DeleteBookAsync(CancellationToken token)
        {
            if (SelectedBook == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Вы уверены, что хотите удалить книгу '{SelectedBook.Title}'?",
                "Подтверждение удаления",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await BookRepository.DeleteBookAsync(SelectedBook.Id, token);
                    await RefreshBooksAsync(token);
                    ClearForm();
                    IsNewBook = false;
                    IsEditMode = false;

                    System.Windows.MessageBox.Show("Книга успешно удалена!",
                        "Успех", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка при удалении книги: {ex.Message}",
                        "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task RefreshBooksAsync(CancellationToken token)
        {
            var books = await BookRepository.GetAllBooksAsync(token);
            Books.Clear();
            foreach (var book in books)
                Books.Add(book);
        }

        private bool CanSaveBook()
        {
            return IsFormEnabled &&
                   !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(Author) &&
                   SelectedBookType != null &&
                   SelectedBookState != null;
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }



    // Вспомогательные классы
    public class GenreSelection : ViewModelBase
    {
        private bool _isSelected;

        public Genre Genre { get; set; } = null!;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
