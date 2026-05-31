using PasswordManager.Services;
using System.ComponentModel;
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

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountInfoService _accountInfoService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly ICacheService _cacheService;
        private readonly IUserService _userService;
        //private readonly IWebSiteFetchService _webSiteFetchService;
        public MainWindow(
            IAccountInfoService accountInfoService,
            IAppSettingsService appSettingsService,
            ICacheService cacheService,
            IUserService userService
            //IWebSiteFetchService webSiteFetchService
            )
        {
            InitializeComponent();
            _accountInfoService = accountInfoService;
            _appSettingsService = appSettingsService;
            _cacheService = cacheService;
            _userService = userService;
            //_webSiteFetchService = webSiteFetchService;

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            var settings = new AppSettings
            {
                Width = Width,
                Height = Height,
                Left = Left,
                Top = Top,
                WindowState = WindowState
            };

            _appSettingsService.Save(settings);
        }
    }
}