using MangaMate.Database.Models;
using MangaMate.Repository;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MangaMate.ViewModels
{
    public sealed class MangaChapterManagerViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        private Book? _manga;
        public Book? Manga
        {
            get => _manga;
            set
            {
                _manga = value;
                OnPropertyChanged(nameof(Manga));
                _ = LoadChapters();
            }
        }

        private int _selectedChapter;

        public int SelectedChapter
        {
            get => _selectedChapter;
            set
            {
                if (SetProperty(ref _selectedChapter, value))
                {
                    _ = LoadChapters();
                    OnPropertyChanged(nameof(IsChapterSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsChapterSelected => SelectedChapter != null;

        public ObservableCollection<int> Chapters { get; } = [];

        public int NewChapterNumber { get; set; }
        public string? NewCbrPath { get; set; }

        public ICommand PickFileCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BackCommand { get; }

        public MangaChapterManagerViewModel()
        {
            PickFileCommand = new Command(_ =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "CBR (*.cbr)|*.cbr;*.7z"
                };
                if (dlg.ShowDialog() == true)
                    NewCbrPath = dlg.FileName;
            });

            AddCommand = new Command(async _ =>
            {
                if (NewChapterNumber <= 0 || string.IsNullOrWhiteSpace(NewCbrPath))
                    return;
                try
                {
                    await BookRepository.AddChapterFromCbrAsync(Manga.Id, NewChapterNumber,
                        NewCbrPath!, _cts.Token);
                    await LoadChapters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            DeleteCommand = new Command(async _ => await DeleteBookAsync(_cts.Token), _ => SelectedChapter != null);

            Mediator.Instance.Register("ShowMangaChapterManager", p => SetManga((p as Book)!));

            BackCommand = new Command(ExecuteOpenMangaChaptersCommand);

            _ = LoadChapters();
        }

        private void ExecuteOpenMangaChaptersCommand(object? obj)
        {
            var manga = Manga;

            Mediator.Instance.Notify("ShowMangaChapters", manga);
        }

        private async Task DeleteBookAsync(CancellationToken token)
        {
            await BookRepository.DeleteChapterAsync(Manga.Id, SelectedChapter, _cts.Token);
            await LoadChapters();
        }

        private async Task LoadChapters()
        {
            if (Manga == null) return;
            var list = await BookRepository.GetChaptersAsync(Manga.Id, _cts.Token);
            Chapters.Clear();
            foreach (var c in list) Chapters.Add(c);
        }

        private void SetManga(Book manga)
        {
            Manga = manga;
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
