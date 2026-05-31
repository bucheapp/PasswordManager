using System;
using System.Windows;

namespace PasswordManager.Services
{
    public interface IAppSettingsService
    {
        void Save(AppSettings appSettings);
        AppSettings? Load();
    }

    public class  AppSettings
    {
        public double Height { set; get; }
        public double Width { set; get; }
        public double? Left { set; get; }
        public double? Top { set; get; }
        public WindowState WindowState { set; get; }
    }
}
