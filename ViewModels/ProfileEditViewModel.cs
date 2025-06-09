using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MangaMate.Database.Models;
using MangaMate.Database.Repositories;
using Microsoft.Win32;

namespace MangaMate.ViewModels
{
    class ProfileEditViewModel : ViewModelBase
    {
        private readonly CancellationTokenSource _cts = new();

        private string _oldPassword = string.Empty;
        public string OldPassword
        {
            get => _oldPassword;
            set
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }

        private string _newPassword = string.Empty;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        private string _newLogin = string.Empty;
        public string NewLogin
        {
            get => _newLogin;
            set
            {
                _newLogin = value;
                OnPropertyChanged(nameof(NewLogin));
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
                    return null;

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

        public ICommand SavePasswordCommand { get; }
        public ICommand SaveLoginCommand { get; }
        public ICommand SelectImageCommand { get; }

        public ProfileEditViewModel()
        {
            SavePasswordCommand = new Command(async _ => await SavePasswordAsync(_cts.Token), (_) => true);
            SaveLoginCommand = new Command(async _ => await SaveAsyncLogin(_cts.Token), (_) => true);
            SelectImageCommand = new Command(async _ => await SelectAsyncImage(_cts.Token), (_) => true);
        }

        private async Task SavePasswordAsync(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 3 || OldPassword.Length < 3)
            {
                MessageBox.Show("Введите старый и новый пароли", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = await UserRepo.GetUser(UserContext.Login, token);
            if (user != null && user.Password == OldPassword)
            {
                await UserRepo.UpdateUserAsync(UserContext.Login, token, newPassword: NewPassword);
                MessageBox.Show("Пароль успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            OldPassword = string.Empty;
            NewPassword = string.Empty;
        }

        private async Task SaveAsyncLogin(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(NewLogin) || NewLogin == UserContext.Login)
            {
                MessageBox.Show("Введити новый логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await UserRepo.UpdateUserAsync(UserContext.Login, token, newLogin: NewLogin);
            UserContext.Login = NewLogin;
            MessageBox.Show("Никнейм успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            NewLogin = string.Empty;
        }

        private async Task SelectAsyncImage(CancellationToken token)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Выберите изображение для аватара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Avatar = File.ReadAllBytes(openFileDialog.FileName);
                    await UserRepo.UpdateUserAsync(UserContext.Login, token, newAvatar: Avatar);
                    UserContext.Avatar = Avatar;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
