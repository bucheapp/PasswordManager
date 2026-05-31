using Microsoft.Extensions.DependencyInjection;
using PasswordManager.Repositories;
using PasswordManager.Services;
using System.Configuration;
using System.Data;
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

            // SQL
            string connectionString = "Data Source=password.db;";

            // Repository

            services.AddSingleton<IAccountInfoRepository>(_ => new AccountInfoRepository(connectionString));
            services.AddSingleton<ICacheRepository>(_ => new CacheRepository(connectionString));
            services.AddSingleton<IUserRepository>(_ => new UserRepository(connectionString));

            // Service
            services.AddSingleton<IAccountInfoService, AccountInfoService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IUserService, UserService>();
            //services.AddSingleton<IWebSiteFetchService, WebSiteFetchService>();

            // Window
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();

            var settings = new AppSettingsService().Load();

            if (settings == null)
            {
                return;
            }

            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            mainWindow.Width = settings.Width;
            mainWindow.Height = settings.Height;
            mainWindow.Left = settings.Left ?? mainWindow.Left;
            mainWindow.Top = settings.Top ?? mainWindow.Top;
            mainWindow.WindowState = settings.WindowState;

            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
