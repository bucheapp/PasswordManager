using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Services
{
    public class WindowService : IWindowService
    {
        ISettingsService _settingsService;
        IUserService _userService;

        public WindowService(ISettingsService settingsService, IUserService userService)
        {
            _settingsService = settingsService;
            _userService = userService;
        }

        public SelectUserWindow? ShowSelectUserWindow(List<Models.User> users)
        {
            AppSettings appSettings = _settingsService.LoadAppSettings();

            User? defaultUser = _userService.Get(appSettings.DefaultUserId);
            if (defaultUser == null)
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
        public CreateUserWindow? ShowCreateUserWindow(string prevName, string prevPassword)
        {
            var window = new CreateUserWindow(prevName, prevPassword);

            if (window.ShowDialog() == true)
            {
                string name = window.UserName;
                string password = window.Password;
                string confirmPassword = window.ConfirmPassword;

                User user = new()
                {
                    Name = name
                };

                AppSettings appSettings = _settingsService.LoadAppSettings();

                _settingsService.SaveAppSettings(appSettings);

                window.CreatedUser = user;
                try
                {
                    if (password != confirmPassword)
                    {
                        throw new ArgumentException("Passwords do not match.");
                    }
                    _userService.Create(user, password);

                    return window;
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(
                        e.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return ShowCreateUserWindow(name, password);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
