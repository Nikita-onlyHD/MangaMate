using System.Configuration;
using System.Data;
using System.Windows;
using MangaMate.Database;
using MangaMate.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MangaMate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            using var context = new ContextFactory().CreateDbContext(Array.Empty<string>());

            var authenticationView = new AuthenticationView();
            authenticationView.Show();
            authenticationView.IsVisibleChanged += (s, ev) =>
            {
                if (authenticationView.IsVisible == false && authenticationView.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                    authenticationView.Close();
                }
            };
        }
    }

}
