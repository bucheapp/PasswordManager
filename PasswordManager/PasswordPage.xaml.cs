using PasswordManager.Models;
using PasswordManager.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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
    /// PasswordPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PasswordPage : Page
    {
        private readonly IServiceInfoService _serviceInfoService;
        private readonly IAccountInfoService _accountInfoService;
        public PasswordPage(
            IServiceInfoService serviceInfoService,
            IAccountInfoService accountInfoService
            )
        {
            InitializeComponent();
            _serviceInfoService = serviceInfoService;
            _accountInfoService = accountInfoService;
        }

        public void Init()
        {
            List<ServiceInfo> serviceInfos = _serviceInfoService.GetAll();

            foreach (var serviceInfo in serviceInfos)
            {
                var serviceInfoParts = CreateServiceInfoParts(serviceInfo.Title);
                ServicePanel.Children.Add(serviceInfoParts);
                List<AccountInfo> accountInfos = _accountInfoService.GetByServiceInfoId(serviceInfo.Id);
                foreach (var accountInfo in accountInfos)
                {
                    InsertAccountInfoParts(serviceInfoParts, accountInfo.Name, accountInfo.AuthType);
                }
            }
        }

        public void Add_Click(object sender, RoutedEventArgs e)
        {
            CreateServiceInfoWindow serviceInfoWindow = new CreateServiceInfoWindow("","","","");
            if (serviceInfoWindow.ShowDialog() == true)
            {
                string title = serviceInfoWindow.ServiceTitle;
                string url = serviceInfoWindow.ServiceUrl;
                string name = serviceInfoWindow.AccountName;
                string password = serviceInfoWindow.AccountPassword;
                string confirmPassword = serviceInfoWindow.ConfirmPassword;

                long serviceInfoId = -1;
                try
                {
                    if (password != confirmPassword)
                    {
                        throw new ArgumentException("Passwords do not match.");
                    }

                    ServiceInfo? serviceInfo = _serviceInfoService.Get(title);
                    if(serviceInfo == null)
                    {
                        serviceInfo = new ServiceInfo();
                        serviceInfo.Title = title;

                        if (!string.IsNullOrEmpty(url))
                        {
                            serviceInfo.Url = url;
                        }

                        _serviceInfoService.Create(serviceInfo);
                        serviceInfoId = serviceInfo.Id;
                    }

                    AccountInfo accountInfo = new();
                    accountInfo.Name = name;
                    // Check if it's in email format.
                    try
                    {
                        var addr = new MailAddress(name);
                        accountInfo.AuthType = AuthType.EmailPassword;
                    } catch
                    {
                        accountInfo.AuthType = AuthType.UsernamePassword;
                    }

                    accountInfo.Password = password;
                    accountInfo.ServiceInfoId = serviceInfo.Id;

                    _accountInfoService.Create(accountInfo);

                    if(serviceInfoId == -1)
                    {
                        Border? serviceInfoParts = GetServiceInfoPartsByHeader(serviceInfo.Title);
                        if (serviceInfoParts != null)
                        {
                            InsertAccountInfoParts(serviceInfoParts, accountInfo.Name, accountInfo.AuthType);
                        }
                    } else
                    {
                        var serviceInfoParts = CreateServiceInfoParts(serviceInfo.Title);
                        ServicePanel.Children.Add(serviceInfoParts);
                        InsertAccountInfoParts(serviceInfoParts,accountInfo.Name, accountInfo.AuthType);
                    }
                } catch(ArgumentException e2)
                {
                    if(serviceInfoId != -1)
                    {
                        _serviceInfoService.Delete(serviceInfoId);
                    }
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        public void Insert_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountInfoWindow accountInfoWindow = new CreateAccountInfoWindow("");
            accountInfoWindow.Show();
            if (accountInfoWindow.ShowDialog() == true)
            {
                string name = accountInfoWindow.AccountName;
                string password = accountInfoWindow.AccountPassword;
            } else
            {

            }
        }

        private Border CreateServiceInfoParts(string title)
        {
            var border = new Border
            {
                Margin = new Thickness(5),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10)
            };

            var expander = new Expander
            {
                Header = title,
                IsExpanded = false,
                Padding = new Thickness(10)
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            var addButton = new Button
            {
                Width = 23,
                Height = 23,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 0, 0),
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Content = "\uE710",
                FontSize = 8,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand
            };

            addButton.Click += Add_Click;

            var template = new ControlTemplate(typeof(Button));

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetValue(Border.BackgroundProperty,
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(20));

            var presenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            presenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            presenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(presenterFactory);
            template.VisualTree = borderFactory;

            var hoverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            hoverTrigger.Setters.Add(
                new Setter(
                    Border.BackgroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F57C00")),
                    "border"));

            var pressedTrigger = new Trigger
            {
                Property = Button.IsPressedProperty,
                Value = true
            };
            pressedTrigger.Setters.Add(
                new Setter(
                    Border.BackgroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E65100")),
                    "border"));

            template.Triggers.Add(hoverTrigger);
            template.Triggers.Add(pressedTrigger);

            addButton.Template = template;

            stackPanel.Children.Add(addButton);
            expander.Content = stackPanel;
            border.Child = expander;

            return border;
        }

        private void InsertAccountInfoParts(Border serviceInfoParts,string name,AuthType authType)
        {
            if (serviceInfoParts.Child is not Expander expander)
                return;

            if (expander.Content is not StackPanel stackPanel)
                return;

            string icon;
            string text;

            switch (authType)
            {
                case AuthType.EmailPassword:
                    icon = "\uE715";
                    text = name;
                    break;

                case AuthType.UsernamePassword:
                    icon = "\uE77B";
                    text = name;
                    break;

                default:
                    return;
            }

            var itemBorder = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#F5F7FA")),
                BorderBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Margin = authType == AuthType.EmailPassword
                    ? new Thickness(0, 0, 0, 5)
                    : new Thickness(0, 0, 0, 5)
            };

            var grid = new Grid();

            grid.ColumnDefinitions.Add(
                new ColumnDefinition { Width = GridLength.Auto });

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

            var iconText = new TextBlock
            {
                Text = icon,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 14,
                Margin = new Thickness(0, 0, 8, 0)
            };

            var valueText = new TextBlock
            {
                Text = text
            };

            Grid.SetColumn(valueText, 1);

            grid.Children.Add(iconText);
            grid.Children.Add(valueText);

            itemBorder.Child = grid;

            int insertIndex = Math.Max(0, stackPanel.Children.Count - 1);
            stackPanel.Children.Insert(insertIndex, itemBorder);
        }

        private Border? GetServiceInfoPartsByHeader(string header)
        {
            foreach (var child in ServicePanel.Children)
            {
                if (child is Border border &&
                    border.Child is Expander expander &&
                    expander.Header?.ToString() == header)
                {
                    return border;
                }
            }

            return null;
        }
    }
}
