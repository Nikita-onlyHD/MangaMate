using System.Windows;
using System.Windows.Input;
using MangaMate.Database.Models;
using MangaMate.Database.Repositories;
using MangaMate.Views;

namespace MangaMate.ViewModels;

internal class AuthenticationViewModel : ViewModelBase
{
    #region properties
    private readonly CancellationTokenSource _cts = new();

    private bool _isViewVisible = true;

    public bool IsViewVisible
    {
        get => _isViewVisible;
        set
        {
            if (_isViewVisible != value)
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
    }

    private Visibility _loginMode = Visibility.Visible;

    public Visibility LoginMode
    {
        get => _loginMode;
        set
        {
            if (_loginMode != value)
            {
                _loginMode = value;
                OnPropertyChanged(nameof(LoginMode));
            }
        }
    }


    private Visibility _registerMode = Visibility.Hidden;

    public Visibility RegisterMode
    {
        get => _registerMode;
        set
        {
            if (_registerMode != value)
            {
                _registerMode = value;
                OnPropertyChanged(nameof(RegisterMode));
            }
        }
    }

    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
    }

    private string _login = string.Empty;

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

    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }

    private string _errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }
    #endregion

    #region Commands
    public ICommand LoginCommand { get; init; }
    public ICommand RegisterCommand { get; init; }
    public ICommand ChangeViewCommand { get; init; }
    #endregion

    public AuthenticationViewModel()
    {
        LoginCommand = new Command(async _ => await AuthenticateAsync(_cts.Token), CanAuthenticate);
        RegisterCommand = new Command(async _ => await RegisterAsync(_cts.Token), CanRegister);
        ChangeViewCommand = new Command(_ => ChangeView(), _ => true);
    }

    private void ChangeView()
    {
        if (LoginMode == Visibility.Visible)
        {
            LoginMode = Visibility.Hidden;
            RegisterMode = Visibility.Visible;
        } else
        {
            LoginMode = Visibility.Visible;
            RegisterMode = Visibility.Hidden;
        }
    }

    private bool CanRegister(object? obj)
    {
        bool validData;
        if (string.IsNullOrWhiteSpace(Login) ||
            Login.Length < 3 ||
            string.IsNullOrWhiteSpace(Password) ||
            Password.Length < 3 ||
            string.IsNullOrWhiteSpace(Email) ||
            Email.Length < 3)
        {
            validData = false;
        }
        else
        {
            validData = true;
        }

        return validData;
    }

    private async Task RegisterAsync(CancellationToken token)
    {
        var user = new User()
        {
            Login = Login,
            Email = Email,
            Password = Password,
        };

        var result = await UserRepo.AddUserAsync(user, token);
        ClearInputs();
        ChangeView();
    }

    private bool CanAuthenticate(object? obj)
    {
        bool validData;
        if (string.IsNullOrWhiteSpace(Login) || Login.Length < 3 || string.IsNullOrWhiteSpace(Password) || Password.Length < 3)
        {
            validData = false;
        }
        else
        {
            validData = true;
        }

        return validData;
    }

    private async Task AuthenticateAsync(CancellationToken token)
    {

        var user = await UserRepo.GetUser(Login, Password, token);
        if (user != null)
        {
            IsViewVisible = false;
            UserContext.Id = user.Id;
            UserContext.Email = user.Email;
            UserContext.Login = user.Login;
            ClearInputs();
        }
        else
        {
            ErrorMessage = "Неверный логин или пароль";
        }
        
    }

    private void ClearInputs()
    {
        Login = string.Empty;
        Password = string.Empty;
        Email = string.Empty;
    }
}
