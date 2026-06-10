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
        private readonly IWindowService _windowService;
        private readonly PasswordPage _passwordPage;
        //private readonly IWebSiteFetchService _webSiteFetchService;

        public MainWindow(
            IAccountInfoService accountInfoService,
            IServiceInfoService serviceInfoService,
            ISettingsService settingsService,
            ICacheService cacheService,
            IUserService userService,
            IWindowService windowService,
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
            _windowService = windowService;
            _passwordPage = passwordPage;
            //_webSiteFetchService = webSiteFetchService;

            var windowSettings = _settingsService.LoadWindowSettings();

            WindowStartupLocation = WindowStartupLocation.Manual;

            Width = windowSettings.Width;
            Height = windowSettings.Height;
            Left = windowSettings.Left ?? Left;
            Top = windowSettings.Top ?? Top;
            WindowState = windowSettings.WindowState;

            Init(null);

            Closing += MainWindow_Closing;

            Show();
        }

        private void Init(User? prevSelectedUser)
        {
            List<User> users = _userService.GetAll();
            string? password = null;
            User? selectedUser = null;

            if (users.Count == 0)
            {
                var createUserWindow = _windowService.ShowCreateUserWindow("","");
                if(createUserWindow != null)
                {
                    password = createUserWindow.Password;
                    selectedUser = createUserWindow.CreatedUser;
                } else
                {
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            } else
            {
                var selectUserWindow = _windowService.ShowSelectUserWindow(users,prevSelectedUser);
                if (selectUserWindow != null)
                {
                    password = selectUserWindow.Password;
                    selectedUser = selectUserWindow.SelectedUser;
                } else
                {
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            }

            Title = $"Password Manager - {selectedUser?.Name}";

            if (password != null)
            {
                try
                {
                    _accountInfoService.SetDB(selectedUser?.Id ?? users[0].Id, password);
                    _serviceInfoService.SetDB(selectedUser?.Id ?? users[0].Id, password);
                }
                catch (SqliteException e)
                {
                    MessageBox.Show(
                        e.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Init(selectedUser);

                    return;
                }

                List<ServiceInfo> serviceInfos = _serviceInfoService.GetAll();
                _passwordPage.Init();
                MainFrame.Navigate(_passwordPage);
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