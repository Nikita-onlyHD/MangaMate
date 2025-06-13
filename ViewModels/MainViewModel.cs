using MangaMate.Views;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangaMate.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region property
        private readonly CancellationTokenSource _cts = new();

        private ViewModelBase _currentChildView;

        public ViewModelBase CurrentChildView
        {
            get => _currentChildView;
            set
            {
                if (_currentChildView != value)
                {
                    _currentChildView = value;
                    OnPropertyChanged(nameof(CurrentChildView));
                }
            }
        }

        private string _caption;
        public string Caption
        {
            get => _caption;
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    OnPropertyChanged(nameof(Caption));
                }
            }
        }

        private string _login = UserContext.Login;
        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        private byte[]? _avatar = UserContext.Avatar;
        public byte[]? Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                OnPropertyChanged(nameof(Avatar));
                OnPropertyChanged(nameof(AvatarImage));
            }
        }

        private HomeViewModel _homeViewModel = new();
        private ProfileEditViewModel _profileEditViewModel = new();
        private BookEditViewModel _bookEditViewModel = new();
        private CatalogMangasViewModel _catalogMangasViewModel = new();
        private MangaDetailsViewModel _mangaDetailsViewModel = new();
        private MangaChaptersViewModel _mangaChaptersViewModel = new();
        private MangaChapterManagerViewModel _mangaChapterManagerViewModel = new();
        private MangaReaderViewModel _mangaReaderViewModel = new();

        public BitmapImage AvatarImage
        {
            get
            {
                if (Avatar == null || Avatar.Length == 0)
                    return new BitmapImage();

                var image = new BitmapImage();
                using (var mem = new MemoryStream(Avatar))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
        }

        #endregion

        #region Command
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowProfileEditViewCommand { get; }
        public ICommand ShowBookEditViewCommand { get; }
        public ICommand ShowCatalogMangasViewCommand { get; }
        public ICommand ShowMangaDetailsViewCommand { get; }
        public ICommand ShowMangaChaptersViewCommand { get; }
        public ICommand ShowMangaChapterManagerViewCommand { get; }
        public ICommand ShowMangaReaderViewCommand { get; }

        #endregion

        public MainViewModel()
        {
            //Initialize commands
            ShowHomeViewCommand = new Command(ExecuteShowHomeViewCommand, (_) => true);
            ShowProfileEditViewCommand = new Command(ExecuteShowProfileEditViewCommand, (_) => true);
            ShowBookEditViewCommand = new Command(ExecuteShowBookEditViewCommand, (_) => true);
            ShowCatalogMangasViewCommand = new Command(ExecuteShowCatalogMangasViewCommand, (_) => true);
            ShowMangaDetailsViewCommand = new Command(ExecuteShowMangaDetailsViewCommand, (_) => true);
            ShowMangaChaptersViewCommand = new Command(ExecuteShowMangaChaptersViewCommand, (_) => true);
            ShowMangaChapterManagerViewCommand = new Command(ExecuteShowMangaChapterManagerViewCommand, (_) => true);
            ShowMangaReaderViewCommand = new Command(ExecuteShowMangaReaderViewCommand, (_) => true);

            //Default view
            ExecuteShowHomeViewCommand(null);
            UserContext.PropertyChanged += UpdateProperties;

            Mediator.Instance.Register("ShowManga", ExecuteShowMangaDetailsViewCommand);
            Mediator.Instance.Register("ShowMangaChapters", ExecuteShowMangaChaptersViewCommand);
            Mediator.Instance.Register("ShowMangaChapterManager", ExecuteShowMangaChapterManagerViewCommand);
            Mediator.Instance.Register("ShowMangaReader", ExecuteShowMangaReaderViewCommand);
            Mediator.Instance.Register("BackToCatalog", ExecuteShowCatalogMangasViewCommand);
        }

        private void UpdateProperties(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserContext.Login):
                    Login = UserContext.Login;
                    break;
                case nameof(UserContext.Avatar):
                    Avatar = UserContext.Avatar;
                    break;
                default:
                    break;
            }
        }

        private void ExecuteShowHomeViewCommand(object? obj)
        {
            CurrentChildView = _homeViewModel;
            Caption = "Home";
        }

        private void ExecuteShowProfileEditViewCommand(object? obj)
        {
            CurrentChildView = _profileEditViewModel;
            Caption = "Profile Edit";
        }

        private void ExecuteShowBookEditViewCommand(object? obj)
        {
            CurrentChildView = _bookEditViewModel;
            Caption = "Book Edit";
        }

        private void ExecuteShowCatalogMangasViewCommand(object? obj)
        {
            CurrentChildView = _catalogMangasViewModel;
            Caption = "Catalog Mangas";
        }

        private void ExecuteShowMangaDetailsViewCommand(object? obj)
        {
            CurrentChildView = _mangaDetailsViewModel;
            Caption = "Manga Details";
        }

        private void ExecuteShowMangaChaptersViewCommand(object? obj)
        {
            CurrentChildView = _mangaChaptersViewModel;
            Caption = "Manga Chapters";
        }

        private void ExecuteShowMangaChapterManagerViewCommand(object? obj)
        {
            CurrentChildView = _mangaChapterManagerViewModel;
            Caption = "Manga Chapter Manger";
        }

        private void ExecuteShowMangaReaderViewCommand(object? obj)
        {
            CurrentChildView = _mangaReaderViewModel;
            Caption = "Manga Reader";
        }
    }
}
