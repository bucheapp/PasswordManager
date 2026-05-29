using System;
using System.Windows;

namespace PasswordManager.Services
{
    internal interface IAppSettingsService
    {
        void Save(AppSettings appSettings);
        AppSettings Load();
    }

    public class  AppSettings
    {
        public double Height { set; get; }
        public double Width { set; get; }
        public WindowState WindowState { set; get; }
    }
}
