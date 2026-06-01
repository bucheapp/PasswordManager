using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PasswordManager.Repositories;
using PasswordManager.Services;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries_V2.Init();

            var services = new ServiceCollection();

            if (!Directory.Exists("db"))
            {
                Directory.CreateDirectory("db");
            }

            // SQL
            string connectionAccountInfoString = "Data Source=db/accountinfo.db;";
            string connectionCacheString = "Data Source=db/cache.db;";
            string connectionUserString = "Data Source=db/user.db;";

            // Repository

            services.AddSingleton<IAccountInfoRepository>(_ => new AccountInfoRepository(connectionAccountInfoString));
            services.AddSingleton<ICacheRepository>(_ => new CacheRepository(connectionCacheString));
            services.AddSingleton<IUserRepository>(_ => new UserRepository(connectionUserString));

            // Service
            services.AddSingleton<IAccountInfoService, AccountInfoService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IUserService, UserService>();
            //services.AddSingleton<IWebSiteFetchService, WebSiteFetchService>();

            // Window
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();

            //Services.GetRequiredService<MainWindow>();

            base.OnStartup(e);
        }
    }
}
