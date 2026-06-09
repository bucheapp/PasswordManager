using System;
using System.Windows;

namespace PasswordManager.Services
{
    public interface ISettingsService
    {
        void SaveWindowSettings(WindowSettings windowSettings);
        WindowSettings LoadWindowSettings();
        void SaveAppSettings(AppSettings appSettings);
        AppSettings LoadAppSettings();
    }

    public class AppSettings
    {
        public long DefaultUserId { get; set; }
    }
    public class  WindowSettings
    {
        public double Height { set; get; }
        public double Width { set; get; }
        public double? Left { set; get; }
        public double? Top { set; get; }
        public WindowState WindowState { set; get; }
    }
}
