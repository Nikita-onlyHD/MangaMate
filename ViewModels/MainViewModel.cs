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

        public string Login
        {
            get => UserContext.Login;
            set
            {
                if (UserContext.Login != value)
                {
                    UserContext.Login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        public byte[]? Avatar
        {
            get => UserContext.Avatar;
            set
            {
                UserContext.Avatar = value;
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

        #endregion

        public MainViewModel()
        {
            //Initialize commands
            ShowHomeViewCommand = new Command(ExecuteShowHomeViewCommand, (_) => true);
            ShowProfileEditViewCommand = new Command(ExecuteShowProfileEditViewCommand, (_) => true);

            //Default view
            ExecuteShowHomeViewCommand(null);

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
    }
}
