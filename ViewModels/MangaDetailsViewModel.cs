using MangaMate.Database.Models;
using System.Windows.Input;

namespace MangaMate.ViewModels
{
    public class MangaDetailsViewModel : ViewModelBase
    {
        private Book? _manga;
        public Book? Manga
        {
            get => _manga;
            set
            {
                _manga = value;
                OnPropertyChanged(nameof(Manga));
            }
        }

        public ICommand BackCommand { get; }
        public ICommand ChaptersCommand { get; }

        public MangaDetailsViewModel()
        {
            BackCommand =  new Command(_ => Mediator.Instance.Notify("BackToCatalog"));
            ChaptersCommand =  new Command(ExecuteOpenMangaChaptersCommand);

            Mediator.Instance.Register("ShowManga", p => SetManga((p as Book)!));
        }

        private void ExecuteOpenMangaChaptersCommand(object? obj)
        {
            var manga = Manga;

            Mediator.Instance.Notify("ShowMangaChapters", manga);
        }

        private void SetManga(Book manga)
        {
            Manga = manga;
        }
    }
}
