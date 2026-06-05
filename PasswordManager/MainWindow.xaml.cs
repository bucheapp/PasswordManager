using Microsoft.Data.Sqlite;
using PasswordManager.Models;
using PasswordManager.Repositories;
using PasswordManager.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountInfoService _accountInfoService;
        private readonly IServiceInfoService _serviceInfoService;
        private readonly ISettingsService _settingsService;
        private readonly ICacheService _cacheService;
        private readonly IUserService _userService;
        private readonly PasswordPage _passwordPage;
        //private readonly IWebSiteFetchService _webSiteFetchService;
        private User? _currentUser;

        public MainWindow(
            IAccountInfoService accountInfoService,
            IServiceInfoService serviceInfoService,
            ISettingsService settingsService,
            ICacheService cacheService,
            IUserService userService
,
            PasswordPage passwordPage
            //IWebSiteFetchService webSiteFetchService
            )
        {
            InitializeComponent();
            _accountInfoService = accountInfoService;
            _serviceInfoService = serviceInfoService;
            _settingsService = settingsService;
            _cacheService = cacheService;
            _userService = userService;
            _passwordPage = passwordPage;
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
            string? password = null;

            if (users.Count == 0)
            {
                var createUserWindow = ShowCreateUserWindow("","");
                if(createUserWindow != null)
                {
                    password = createUserWindow.Password;
                } else
                {
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            } else
            {
                var selectUserWindow = ShowSelectUserWindow(users);
                if (selectUserWindow != null)
                {
                    _currentUser = selectUserWindow.SelectedUser;
                    password = selectUserWindow.Password;
                } else
                {
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            }

            Title = $"Password Manager - {_currentUser?.Name}";
            
            if(password != null)
            {
                try
                {
                    _accountInfoService.SetDB(_currentUser?.Id ?? users[0].Id, password);
                    _serviceInfoService.SetDB(_currentUser?.Id ?? users[0].Id, password);
                }
                catch (SqliteException e)
                {
                    MessageBox.Show(
                        e.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Init();

                    return;
                }

                List<ServiceInfo> serviceInfos = _serviceInfoService.GetAll();
                _passwordPage.Init();
                MainFrame.Navigate(_passwordPage);
            }
        }

        private SelectUserWindow? ShowSelectUserWindow(List<User> users)
        {
            AppSettings appSettings = _settingsService.LoadAppSettings();

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
                if (string.IsNullOrEmpty(window.Password))
                {
                    MessageBox.Show("Please enter your password.");
                    return ShowSelectUserWindow(users);
                }
                return window;
            }
            else
            {
                return null;
            }
        }

        private CreateUserWindow? ShowCreateUserWindow(string prevName,string prevPassword)
        {
            var window = new CreateUserWindow(prevName,prevPassword);

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

                    return ShowCreateUserWindow(name,password);
                }
            } else
            {
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