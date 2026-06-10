using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using PasswordManager.Models;
using PasswordManager.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
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
        private readonly IUserService _userService;
        private readonly IWindowService _windowService;
        private readonly IServiceInfoToTextService _serviceInfoToTextService;
        private readonly IWebSiteFetchService _webSiteFetchService;
        private readonly ICacheService _cacheService;
        public PasswordPage(
            IServiceInfoService serviceInfoService,
            IAccountInfoService accountInfoService,
            IUserService userService,
            IWindowService windowService,
            IServiceInfoToTextService serviceInfoToTextService,
            IWebSiteFetchService webSiteFetchService,
            ICacheService cacheService
            )
        {
            InitializeComponent();
            _serviceInfoService = serviceInfoService;
            _accountInfoService = accountInfoService;
            _userService = userService;
            _windowService = windowService;
            _serviceInfoToTextService = serviceInfoToTextService;
            _webSiteFetchService = webSiteFetchService;
            _cacheService = cacheService;
        }

        public void Init()
        {
            List<ServiceInfo> serviceInfos = _serviceInfoService.GetAll();

            if(serviceInfos.Count == 0)
            {
                EmptyMessage.Visibility = Visibility.Visible;
            }

            foreach (var serviceInfo in serviceInfos)
            {
                var serviceInfoParts = CreateServiceInfoParts(serviceInfo.Id,serviceInfo.Title);
                ServicePanel.Children.Add(serviceInfoParts);
                if (serviceInfo.Url != null)
                {
                    AddFavicon(serviceInfoParts, serviceInfo.Url);
                }
                List<AccountInfo> accountInfos = _accountInfoService.GetByServiceInfoId(serviceInfo.Id);
                foreach (var accountInfo in accountInfos)
                {
                    InsertAccountInfoParts(serviceInfoParts,accountInfo.Id, accountInfo.Name, accountInfo.AuthType);
                }
            }
        }

        public void Add_Click(object sender, RoutedEventArgs e)
        {
            ShowCreateServiceInfoWindow("","","","");
        }
        private void ShowCreateServiceInfoWindow(string prevTitle,string prevUrl,string prevName,string prevPassword)
        {
            CreateServiceInfoWindow serviceInfoWindow = new CreateServiceInfoWindow(prevTitle,prevUrl,prevName,prevPassword);
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
                    if (serviceInfo == null)
                    {
                        serviceInfo = new ServiceInfo();
                        serviceInfo.Title = title;

                        if (!string.IsNullOrWhiteSpace(url))
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
                    }
                    catch
                    {
                        accountInfo.AuthType = AuthType.UsernamePassword;
                    }

                    accountInfo.Password = password;
                    accountInfo.ServiceInfoId = serviceInfo.Id;

                    _accountInfoService.Create(accountInfo);

                    if (serviceInfoId == -1)
                    {
                        Border? serviceInfoParts = GetServiceInfoPartsByHeader(serviceInfo.Title);
                        if (serviceInfoParts != null)
                        {
                            InsertAccountInfoParts(serviceInfoParts,accountInfo.Id, accountInfo.Name, accountInfo.AuthType);
                        }
                    }
                    else
                    {
                        var serviceInfoParts = CreateServiceInfoParts(serviceInfoId,serviceInfo.Title);
                        ServicePanel.Children.Add(serviceInfoParts);

                        if(serviceInfo.Url != null)
                        {
                            AddFavicon(serviceInfoParts,url);
                        }

                        InsertAccountInfoParts(serviceInfoParts,accountInfo.Id, accountInfo.Name, accountInfo.AuthType);
                        EmptyMessage.Visibility = Visibility.Collapsed;
                    }
                }
                catch (Exception e2) when (e2 is ArgumentException || e2 is InvalidOperationException)
                {
                    if (serviceInfoId != -1)
                    {
                        _serviceInfoService.Delete(serviceInfoId);
                    }
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowCreateServiceInfoWindow(title,url,name,password);
                }
            }
        }
        public void AddFavicon(Border serviceInfoParts,string url)
        {
            var expander = serviceInfoParts.Child as Expander;
            var headerPanel = expander?.Header as StackPanel;

            if (headerPanel != null)
            {
                Cache? cache = _cacheService.Load(url);
                if (cache == null)
                {
                    _webSiteFetchService.Fetch(url)
                    .ContinueWith(task =>
                    {
                        cache = _cacheService.Add(task.Result);

                        Dispatcher.Invoke(() =>
                        {
                            var image = new Image
                            {
                                Width = 19,
                                Height = 19,
                                Margin = new Thickness(0, 0, 5, 0),
                                Source = new BitmapImage(
                                    new Uri("pack://siteoforigin:,,,/favicon/" + cache?.ImageUrl ?? ""))
                            };

                            headerPanel.Children.Insert(0, image);
                        });
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        var image = new Image
                        {
                            Width = 19,
                            Height = 19,
                            Margin = new Thickness(0, 0, 5, 0),
                            Source = new BitmapImage(
                                new Uri("pack://siteoforigin:,,,/favicon/" + cache.ImageUrl))
                        };

                        headerPanel.Children.Insert(0, image);
                    });
                }
            }
        }
        public void Insert_Click(object sender, RoutedEventArgs e)
        {
            var current = sender as FrameworkElement;

            while (current != null && current is not Expander)
            {
                current = current.Parent as FrameworkElement;
            }

            var expander = current as Expander;

            if(expander != null)
            {
                ShowCreateAccountInfoWindow(expander, "", "");
            }
        }
        private void ShowCreateAccountInfoWindow(Expander parent,string prevName,string prevPassword)
        {
            CreateAccountInfoWindow accountInfoWindow = new CreateAccountInfoWindow(parent.Header?.ToString() ?? "", prevName,prevPassword);
            if (accountInfoWindow.ShowDialog() == true)
            {
                string name = accountInfoWindow.AccountName;
                string password = accountInfoWindow.AccountPassword;
                string confirmPassword = accountInfoWindow.ConfirmPassword;

                try
                {
                    if (password != confirmPassword)
                    {
                        throw new ArgumentException("Passwords do not match.");
                    }

                    AccountInfo accountInfo = new();
                    accountInfo.Name = name;
                    // Check if it's in email format.
                    try
                    {
                        var addr = new MailAddress(name);
                        accountInfo.AuthType = AuthType.EmailPassword;
                    }
                    catch
                    {
                        accountInfo.AuthType = AuthType.UsernamePassword;
                    }

                    accountInfo.Password = password;
                    accountInfo.ServiceInfoId = (long)parent.Tag;

                    _accountInfoService.Create(accountInfo);

                    if (parent.Parent != null)
                    {
                        InsertAccountInfoParts((Border)parent.Parent,accountInfo.Id, accountInfo.Name, accountInfo.AuthType);
                    }
                }
                catch (Exception e2) when (e2 is ArgumentException || e2 is InvalidOperationException)
                {
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowCreateAccountInfoWindow(parent, name, password);
                }
            }
        }
        private Border CreateServiceInfoParts(long id, string title)
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
                IsExpanded = false,
                Padding = new Thickness(10),
                Tag = id
            };

            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var textBlock = new TextBlock
            {
                Text = title,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(textBlock);

            expander.Header = headerPanel;

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            ControlTemplate CreateTemplate(string color, string hover, string pressed)
            {
                var template = new ControlTemplate(typeof(Button));

                var borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.Name = "border";
                borderFactory.SetValue(Border.BackgroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)));
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
                hoverTrigger.Setters.Add(new Setter(
                    Border.BackgroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(hover)),
                    "border"));

                var pressedTrigger = new Trigger
                {
                    Property = Button.IsPressedProperty,
                    Value = true
                };
                pressedTrigger.Setters.Add(new Setter(
                    Border.BackgroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(pressed)),
                    "border"));

                template.Triggers.Add(hoverTrigger);
                template.Triggers.Add(pressedTrigger);

                return template;
            }

            var addButton = new Button
            {
                Width = 23,
                Height = 23,
                Margin = new Thickness(5, 5, 0, 0),
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Content = "\uE710",
                FontSize = 8,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand,
                Template = CreateTemplate("#FF9800", "#F57C00", "#E65100")
            };
            addButton.Click += Insert_Click;

            var editButton = new Button
            {
                Width = 23,
                Height = 23,
                Margin = new Thickness(5, 5, 0, 0),
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                Content = "\uEB7E",
                FontSize = 10,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand,
                Template = CreateTemplate("#2196F3", "#1976D2", "#0D47A1")
            };
            editButton.Click += EditServiceInfo_Click;

            var deleteButton = new Button
            {
                Width = 23,
                Height = 23,
                Margin = new Thickness(5, 5, 0, 0),
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                Content = "\uE74D",
                FontSize = 10,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand,
                Template = CreateTemplate("#E53935", "#D32F2F", "#B71C1C")
            };
            deleteButton.Click += DeleteServiceInfo_Click;

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            buttonPanel.Children.Add(addButton);
            buttonPanel.Children.Add(editButton);
            buttonPanel.Children.Add(deleteButton);

            stackPanel.Children.Add(buttonPanel);

            expander.Content = stackPanel;
            border.Child = expander;

            return border;
        }
        public void EditServiceInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject obj)
                return;
            var expander = FindParent<Expander>(obj);
            if (expander == null)
                return;
            var id = expander.Tag;

            ServiceInfo? serviceInfo = _serviceInfoService.Get((long)id);

             if (serviceInfo != null)
             {
                 ShowUpdateServiceInfoWindow(expander,serviceInfo,serviceInfo.Title,serviceInfo.Url ?? "");
             }
        }
        public void ShowUpdateServiceInfoWindow(Expander expander,ServiceInfo serviceInfo,string prevTitle,string prevUrl)
        {
            UpdateServiceInfoWindow serviceInfoWindow = new UpdateServiceInfoWindow(prevTitle,prevUrl);
            if (serviceInfoWindow.ShowDialog() == true)
            {
                string newTitle = serviceInfoWindow.NewServiceTitle;
                string? newUrl = serviceInfoWindow.NewServiceUrl;
                if(string.IsNullOrWhiteSpace(newUrl))
                {
                    newUrl = null;
                }

                try
                {
                    if (serviceInfo.Title == newTitle && serviceInfo.Url == newUrl)
                    {
                        throw new InvalidOperationException("No changes were made.");
                    }

                    ServiceInfo newServiceInfo = new ServiceInfo();
                    newServiceInfo.Id = serviceInfo.Id;
                    newServiceInfo.Title = serviceInfo.Title;
                    newServiceInfo.Url = serviceInfo.Url;

                    if (newServiceInfo.Title != newTitle)
                    {
                        newServiceInfo.Title = newTitle;
                    }
                    if (newServiceInfo.Url != newUrl)
                    {
                        newServiceInfo.Url = newUrl;
                    }

                    _serviceInfoService.Update(newServiceInfo);

                    if (expander.Header is StackPanel headerPanel)
                    {
                        if(newUrl != null)
                        {
                            if (headerPanel.Children[0] is Image image)
                            {
                                Cache? cache = _cacheService.Load(newUrl);
                                if (cache == null)
                                {
                                    _webSiteFetchService.Fetch(newUrl)
                                    .ContinueWith(task =>
                                    {
                                        cache = _cacheService.Add(task.Result);
                                        Dispatcher.Invoke(() =>
                                            {
                                                image.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/favicon/" + cache?.ImageUrl));
                                            });
                                    });
                                } else
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        image.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/favicon/" + cache.ImageUrl));
                                        headerPanel.Children.Insert(0, image);
                                    });
                                }
                            } else
                            {
                                AddFavicon((Border)expander.Parent, newUrl);
                            }
                        }
                        foreach (var child in  headerPanel.Children) {
                            if(child is TextBlock textBlock)
                            {
                                textBlock.Text = newTitle;
                            }
                        }
                    }
                }
                catch (Exception e2) when (e2 is ArgumentException || e2 is InvalidOperationException)
                {
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    ShowUpdateServiceInfoWindow(expander,serviceInfo,newTitle,newUrl ?? "");
                }
            }
        }
        public void DeleteServiceInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject obj)
                return;

            var expander = FindParent<Expander>(obj);
            if (expander == null)
                return;

            var id = expander.Tag;

            var result = MessageBox.Show(
                "Are you sure you want to delete this service information? All related account information will also be deleted.",
                "Delete Service Information",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _serviceInfoService.Delete((long)id);
                Border border = (Border)expander.Parent;
                (border.Parent as StackPanel)?.Children.Remove(border);

                List<ServiceInfo> serviceInfos = _serviceInfoService.GetAll();
                if (serviceInfos.Count == 0)
                {
                    EmptyMessage.Visibility = Visibility.Visible;
                }
            }
                
        }
        private void InsertAccountInfoParts(Border serviceInfoParts,long id, string name, AuthType authType)
        {
            if (serviceInfoParts.Child is not Expander expander)
                return;

            if (expander.Content is not StackPanel stackPanel)
                return;

            string icon;

            switch (authType)
            {
                case AuthType.EmailPassword:
                    icon = "\uE715";
                    break;

                case AuthType.UsernamePassword:
                    icon = "\uE77B";
                    break;

                default:
                    return;
            }

            var itemBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F7FA")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 5),
                Tag = id
            };

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var iconText = new TextBlock
            {
                Text = icon,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 14,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var valueText = new TextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(valueText, 1);

            var rightIcons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var copyIcon = new TextBlock
            {
                Text = "\uE8C8",
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 14,
                Margin = new Thickness(0, 0, 10, 0),
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };

            copyIcon.MouseUp += CopyAccountInfo_Click;

            var editIcon = new TextBlock
            {
                Text = "\uEB7E",
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 14,
                Margin = new Thickness(0, 0, 10, 0),
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };

            editIcon.MouseUp += EditAccountInfo_Click;

            var deleteIcon = new TextBlock
            {
                Text = "\uE74D",
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 14,
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };

            deleteIcon.MouseUp += DeleteAccountInfo_Click;

            rightIcons.Children.Add(copyIcon);
            rightIcons.Children.Add(editIcon);
            rightIcons.Children.Add(deleteIcon);

            Grid.SetColumn(rightIcons, 2);

            grid.Children.Add(iconText);
            grid.Children.Add(valueText);
            grid.Children.Add(rightIcons);

            itemBorder.Child = grid;

            int insertIndex = Math.Max(0, stackPanel.Children.Count - 1);
            stackPanel.Children.Insert(insertIndex, itemBorder);
        }
        private void CopyAccountInfo_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock tb)
                return;

            var itemBorder = FindParent<Border>(tb);
            if (itemBorder == null)
                return;

            if (itemBorder.Tag is not long id)
                return;

            AccountInfo? accountInfo = _accountInfoService.Get(id);
            if (accountInfo == null)
                return;

            Clipboard.SetText(accountInfo.Password);
        }
        public void EditAccountInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBlock tb)
                return;
            var itemBorder = FindParent<Border>(tb);
            if (itemBorder == null)
                return;

            if (itemBorder.Tag is not long id)
                return;

            AccountInfo? accountInfo = _accountInfoService.Get(id);

            if (accountInfo == null)
                return;

            ShowUpdateAccountInfoWindow(itemBorder, accountInfo,accountInfo.Name,accountInfo.Password);
        }
        private void ShowUpdateAccountInfoWindow(Border itemBorder,AccountInfo accountInfo,string prevName,string prevPassword)
        {
            UpdateAccountInfoWindow accountInfoWindow = new UpdateAccountInfoWindow(prevName,prevPassword);
            if (accountInfoWindow.ShowDialog() == true)
            {
                string newName = accountInfoWindow.NewAccountName;
                string newPassword = accountInfoWindow.NewAccountPassword;
                string confirmPassword = accountInfoWindow.ConfirmPassword;
                try
                {
                    if (accountInfo.Password != newPassword && newPassword != confirmPassword)
                    {
                        throw new ArgumentException("Passwords do not match.");
                    }

                    if (accountInfo.Name == newName && accountInfo.Password == newPassword)
                    {
                        throw new InvalidOperationException("No changes were made.");
                    }

                    AccountInfo newAccountInfo = new AccountInfo();
                    newAccountInfo.Id = accountInfo.Id;
                    newAccountInfo.Name = accountInfo.Name;
                    newAccountInfo.Password = accountInfo.Password;
                    newAccountInfo.AuthType = accountInfo.AuthType;
                    newAccountInfo.ServiceInfoId = accountInfo.ServiceInfoId;

                    if (newAccountInfo.Name != newName)
                    {
                        newAccountInfo.Name = newName;
                    }

                    if (newAccountInfo.Password != newPassword)
                    {
                        newAccountInfo.Password = newPassword;
                    }

                    _accountInfoService.Update(newAccountInfo);

                    var grid = itemBorder.Child as Grid;
                    if (grid == null)
                        return;

                    var valueText = grid.Children
                        .OfType<TextBlock>()
                        .FirstOrDefault(x => Grid.GetColumn(x) == 1);

                    if (valueText != null)
                    {
                        valueText.Text = newName;
                    }
                }
                catch (Exception e2) when (e2 is ArgumentException || e2 is InvalidOperationException)
                {
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowUpdateAccountInfoWindow(itemBorder,accountInfo,newName,newPassword);
                }
            }
        }
        public void DeleteAccountInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBlock tb)
                return;

            var itemBorder = FindParent<Border>(tb);
            if (itemBorder == null)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to delete this account information?",
                "Delete Account Information",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            if (itemBorder.Tag is not long id)
                return;

            AccountInfo? accountInfo = _accountInfoService.Get(id);
            if (accountInfo == null)
                return;

            _accountInfoService.Delete(accountInfo.Id);

            (itemBorder.Parent as StackPanel)?.Children.Remove(itemBorder);
        }
        private T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T target)
                    return target;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        public void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save file",
                Filter = "Text file (*.txt)|*.txt|All file (*.*)|*.*"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filePath = dialog.FileName;

                File.WriteAllText(filePath, _serviceInfoToTextService.Convert(_serviceInfoService.GetAll()));
            }
        }
        public void AddAccountInfo_Click(object sender, RoutedEventArgs e)
        {
            ShowCreateServiceInfoWindow("", "", "", "");
        }
        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        public void CreateUser_Click(object sender, RoutedEventArgs e)
        {
        }
        public void ShowCreateUserWindow(User? prevCurrentUser)
        {
            var createUserWindow = _windowService.ShowCreateUserWindow("", "");
            if (createUserWindow == null)
            {
                return;
            }

            string password = createUserWindow.Password;
            User? currentUser = createUserWindow.CreatedUser;

            if (currentUser != null && password != null)
            {
                try
                {
                    _accountInfoService.SetDB(currentUser.Id, password);
                    _serviceInfoService.SetDB(currentUser.Id, password);
                }
                catch (SqliteException e2)
                {
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowCreateUserWindow(currentUser);
                    return;
                }
                SetNewPage(currentUser, password);
            }
        }
        public void SelectUser_Click(object sender, RoutedEventArgs e)
        {
            List<User> users = _userService.GetAll();
            ShowSelectUserWindow(users,null);
        }
        private void ShowSelectUserWindow(List<User> users,User? prevCurrentUser)
        {
            var selectUserWindow = _windowService.ShowSelectUserWindow(users, prevCurrentUser);
            if (selectUserWindow == null)
            {
                return;
            }

            string password = selectUserWindow.Password;
            User? currentUser = selectUserWindow.SelectedUser;

            if (currentUser != null && password != null)
            {
                try
                {
                    _accountInfoService.SetDB(currentUser.Id, password);
                    _serviceInfoService.SetDB(currentUser.Id, password);
                }
                catch (SqliteException e2)
                {
                    MessageBox.Show(
                        e2.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ShowSelectUserWindow(users,currentUser);
                    return;
                }
                SetNewPage(currentUser, password);
            }
        }
        private void SetNewPage(User currentUser,string password)
        {
            Window.GetWindow(this).Title = $"Password Manager - {currentUser?.Name}";
            ServicePanel.Children.Clear();
            Init();
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
