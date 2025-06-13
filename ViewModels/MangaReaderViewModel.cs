using MangaMate.Database.Models;
using MangaMate.Repository;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangaMate.ViewModels
{
    public sealed class MangaReaderViewModel : ViewModelBase
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

        private int _chapter;
        public int Chapter 
        { 
            get => _chapter; 
            set 
            { 
                _chapter = value; 
                OnPropertyChanged(nameof(Chapter)); 
            } 
        }

        private List<BookPage>? _pages;
        public List<BookPage>? Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged(nameof(Pages));
            }
        }

        private int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set 
            { 
                _pageIndex = value;
                OnPropertyChanged(nameof(PageIndex)); 
                OnPropertyChanged(nameof(PageImage)); 
            }
        }

        
        private int _startPage = 1;
        public int StartPage
        {
            get => _startPage;
            set
            {
                _startPage = value;
                _ = LoadAsync(StartPage);
                OnPropertyChanged(nameof(StartPage));
            }
        }

        public BitmapImage PageImage
        {
            get
            {
                if (Pages == null) return new();
                var data = Pages[PageIndex].Data;
                using var ms = new MemoryStream(data);
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                return img;
            }
        }

        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ChaptersCommand { get; }

        public MangaReaderViewModel()
        {
            NextPageCommand = new Command(_ => { if (PageIndex < Pages.Count - 1) PageIndex++; });
            PrevPageCommand = new Command(_ => { if (PageIndex > 0) PageIndex--; });
            BackCommand = new Command(ExecuteOpenMangaChaptersCommand);

            Mediator.Instance.Register("ShowMangaReader", (o) =>
            {
                var (book, chapter, page) = ((Book, int, int))o!;
                SetManga(book, chapter, page);
            });

            _ = LoadAsync(StartPage);
        }

        private void SetManga(Book manga, int chapter, int startPage)
        {
            Manga = manga;
            Chapter = chapter;
            StartPage = startPage;
        }

        private void ExecuteOpenMangaChaptersCommand(object? obj)
        {
            var manga = Manga;

            Mediator.Instance.Notify("ShowMangaChapters", manga);
        }

        private async Task LoadAsync(int page)
        {
            if (Manga == null || Chapter <= 0) return;

            Pages = await BookRepository.GetPagesAsync(Manga.Id, Chapter, CancellationToken.None);
            PageIndex = Math.Max(0, Math.Min(page - 1, Pages.Count - 1));
            OnPropertyChanged(nameof(PageImage));
        }
    }
}
