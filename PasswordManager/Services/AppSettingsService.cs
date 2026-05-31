using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace PasswordManager.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private const string AppSettingsFileName = "appsettings.json";
        public void Save(AppSettings appSettings)
        {
            string json = JsonSerializer.Serialize(appSettings);
            File.WriteAllText(AppSettingsFileName, json);
        }
        public AppSettings? Load()
        {
            if (File.Exists(AppSettingsFileName)) {
                string json = File.ReadAllText(AppSettingsFileName);
                return JsonSerializer.Deserialize<AppSettings>(json);
            } else {
                AppSettings appSettings = new AppSettings();
                appSettings.Width = 800;
                appSettings.Height = 450;
                appSettings.Left = null;
                appSettings.Top = null;
                appSettings.WindowState = WindowState.Normal;
                return appSettings;
            }
        }
    }
}
