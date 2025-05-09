using System.Configuration;
using System.Data;
using System.Windows;
using MangaMate.Database;
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
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = config.GetConnectionString("pgsql");

            var dbOptions = new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;

            using var context = new Context(dbOptions);
            throw new Exception("aboba");
        }
    }

}
