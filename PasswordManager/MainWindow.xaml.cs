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

            Init();

            Closing += MainWindow_Closing;
        }

        private void Init()
        {
            List<User> users = _userService.GetAll();
            if (users.Count == 0)
            {
                ShowCreateNameWindow();
            }
        }


        private void ShowCreateNameWindow()
        {
            var window = new CreateUserWindow();

            if (window.ShowDialog() == true)
            {
                string name = window.UserName;
                string password = window.Password;

                User user = new User();
                user.Name = name;
                user.Index = 0;

                AppSettings appSettings = new AppSettings();
                appSettings.DefaultUserId = 0;
                _settingsService.SaveAppSettings(appSettings);

                try
                {
                    _userService.Create(user, password);
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(
                        e.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowCreateNameWindow();
                }
            } else
            {
                System.Windows.Application.Current.Shutdown();
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