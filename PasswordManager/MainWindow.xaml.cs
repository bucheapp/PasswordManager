using PasswordManager.Services;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using PasswordManager.Models;
using System.Windows.Markup.Localizer;
using System.Printing;
using PasswordManager.Repositories;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountInfoService _accountInfoService;
        private readonly ISettingsService _settingsService;
        private readonly ICacheService _cacheService;
        private readonly IUserService _userService;
        //private readonly IWebSiteFetchService _webSiteFetchService;
        private User? _currentUser;

        public MainWindow(
            IAccountInfoService accountInfoService,
            ISettingsService settingsService,
            ICacheService cacheService,
            IUserService userService
            //IWebSiteFetchService webSiteFetchService
            )
        {
            InitializeComponent();
            _accountInfoService = accountInfoService;
            _settingsService = settingsService;
            _cacheService = cacheService;
            _userService = userService;
            //_webSiteFetchService = webSiteFetchService;

            var windowSettings = _settingsService.LoadWindowSettings();

            WindowStartupLocation = WindowStartupLocation.Manual;

            Width = windowSettings.Width;
            Height = windowSettings.Height;
            Left = windowSettings.Left ?? Left;
            Top = windowSettings.Top ?? Top;
            WindowState = windowSettings.WindowState;

            Init();

            Closing += MainWindow_Closing;

            Show();
        }

        private void Init()
        {
            List<User> users = _userService.GetAll();
            string password = "";

            if (users.Count == 0)
            {
                var window = ShowCreateNameWindow(null,null);
                if(window != null)
                {
                    password = window.Password;
                }
            } else
            {
                var window = ShowSelectUserWindow(users);
                if (window != null)
                {
                    _currentUser = window.SelectedUser;
                    password = window.Password;
                }
            }

            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAA");
            Title = $"Password Manager - {_currentUser?.Name}";
            _accountInfoService.SetDB(_currentUser?.Id ?? users[0].Id,password);

            MainFrame.Navigate(new PasswordPage());
        }

        private SelectUserWindow? ShowSelectUserWindow(List<User> users)
        {
            AppSettings appSettings = _settingsService.LoadAppSettings();

            Console.WriteLine($"Default User ID: {appSettings.DefaultUserId}");

            User? defaultUser = _userService.Get(appSettings.DefaultUserId);
            if(defaultUser == null)
            {
                AppSettings newAppSettings = new()
                {
                    DefaultUserId = users[0].Id
                };

                _settingsService.SaveAppSettings(newAppSettings);

                return ShowSelectUserWindow(users);
            }

            var window = new SelectUserWindow(users, defaultUser);

            if (window.ShowDialog() == true)
            {
                return window;
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
                return null;
            }
        }

        private CreateUserWindow? ShowCreateNameWindow(string? prevName,string? prevPassword)
        {
            var window = new CreateUserWindow();

            if (prevName != null && prevPassword != null)
            {
                window.SetPreviousData(prevName, prevPassword);
            }

            if (window.ShowDialog() == true)
            {
                string name = window.UserName;
                string password = window.Password;
                string confirmPassword = window.ConfirmPassword;

                User user = new()
                {
                    Id = 1,
                    Name = name,
                    DisplayIndex = 0
                };

                AppSettings appSettings = new()
                {
                    DefaultUserId = 1
                };
                _settingsService.SaveAppSettings(appSettings);

                try
                {
                    if (password != confirmPassword)
                    {
                        throw new ArgumentException("Passwords do not match.");
                    }
                    _userService.Create(user, password);
                    _currentUser = user;

                    return window;
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(
                        e.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return ShowCreateNameWindow(name,password);
                }
            } else
            {
                System.Windows.Application.Current.Shutdown();
                return null;
            }
        }
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            var settings = new WindowSettings
            {
                Width = Width,
                Height = Height,
                Left = Left,
                Top = Top,
                WindowState = WindowState
            };

            _settingsService.SaveWindowSettings(settings);
        }
    }
}