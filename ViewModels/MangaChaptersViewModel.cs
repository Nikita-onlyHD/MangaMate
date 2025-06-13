using MangaMate.Database.Models;
using MangaMate.Repository;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MangaMate.ViewModels
{
    public sealed class MangaChaptersViewModel : ViewModelBase
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
                _ = LoadAsync(_manga, _cts.Token);
            }
        }
        public ObservableCollection<int> Chapters { get; } = [];

        public ICommand OpenReaderCommand { get; }
        public ICommand OpenMgrCommand { get; }
        public ICommand BackCommand { get; }

        public MangaChaptersViewModel()
        {
            BackCommand = new Command(ExecuteOpenMangaCommand);
            OpenMgrCommand = new Command(ExecuteOpenMangaChapterManagerCommand);
            OpenReaderCommand = new Command(ch => Mediator.Instance.Notify("ShowMangaReader", (Manga, (int)ch!, 1)));

            Mediator.Instance.Register("ShowMangaChapters", p => SetManga((p as Book)!));

            _ = LoadAsync(_manga, _cts.Token);
        }

        private async Task LoadAsync(Book? manga, CancellationToken token)
        {
            try
            {
                if (manga == null)
                {
                    return;
                }

                var list = await BookRepository.GetChaptersAsync(manga.Id, token);
                Chapters.Clear();
                foreach (var c in list) Chapters.Add(c);
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

        private void ExecuteOpenMangaChapterManagerCommand(object? obj)
        {
            var manga = Manga;
            Mediator.Instance.Notify("ShowMangaChapterManager", manga);
        }

        private void ExecuteOpenMangaCommand(object? obj)
        {
            var manga = Manga;

            Mediator.Instance.Notify("ShowManga", manga);
        }

        private void SetManga(Book manga)
        {
            Manga = manga;
        }
    }
}
