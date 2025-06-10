using MangaMate.Database.Models;
using System.IO;
using System.Windows.Documents;
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

        #endregion

        public MainViewModel()
        {
            //Initialize commands
            ShowHomeViewCommand = new Command(ExecuteShowHomeViewCommand, (_) => true);
            ShowProfileEditViewCommand = new Command(ExecuteShowProfileEditViewCommand, (_) => true);
            ShowBookEditViewCommand = new Command(ExecuteShowBookEditViewCommand, (_) => true);
            ShowCatalogMangasViewCommand = new Command(ExecuteShowCatalogMangasViewCommand, (_) => true);

            //Default view
            ExecuteShowHomeViewCommand(null);
            UserContext.PropertyChanged += UpdateProperties;

            Mediator.Instance.Register("OpenCatalogManga", _ => CurrentChildView = new CatalogMangasViewModel());
            //Mediator.Instance.Register("OpenMangaDetail", o => CurrentChildView = new MangaDetailViewModel((Book)o!));
            //Mediator.Instance.Register("OpenChapters", o =>
            //{
            //    var (book, chapter) = ((Book book, int? chapter))o!;
            //    CurrentChildView = new ChaptersViewModel(book, chapter);
            //});
            //Mediator.Instance.Register("OpenReader", o =>
            //{
            //    var (book, chapter, page) = ((Book, int, int))o!;
            //    CurrentChildView = new ReaderViewModel(book, chapter, page);
            //});
            //Mediator.Instance.Register("OpenChapterMgr", o =>
            //    CurrentChildView = new ChapterManagerViewModel((Book)o!));
            //Mediator.Instance.Register("BackToCatalog", _ => CurrentChildView = new CatalogMangasViewModel());
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
            CurrentChildView = new HomeViewModel();
            Caption = "Home";
        }

        private void ExecuteShowProfileEditViewCommand(object? obj)
        {
            CurrentChildView = new ProfileEditViewModel();
            Caption = "Profile Edit";
        }

        private void ExecuteShowBookEditViewCommand(object? obj)
        {
            CurrentChildView = new BookEditViewModel();
            Caption = "Book Edit";
        }

        private void ExecuteShowCatalogMangasViewCommand(object? obj)
        {
            CurrentChildView = new CatalogMangasViewModel();
            Caption = "Book Edit";
        }
    }
}
