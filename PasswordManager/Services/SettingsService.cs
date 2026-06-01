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
    public class SettingsService : ISettingsService
    {
        private const string WindowSettingsFileName = "windowsettings.json";
        private const string AppSettingsFileName = "appsettings.json";
        public void SaveWindowSettings(WindowSettings windowSettings)
        {
            string json = JsonSerializer.Serialize(windowSettings);
            File.WriteAllText(WindowSettingsFileName, json);
        }

        public void SaveAppSettings(AppSettings appSettings)
        {
            string json = JsonSerializer.Serialize(appSettings);
            File.WriteAllText(AppSettingsFileName, json);
        }
        public WindowSettings LoadWindowSettings()
        {
            WindowSettings defaultWindowSettings = new WindowSettings()
            {
                Width = 800,
                Height = 450,
                Left = null,
                Top = null,
                WindowState = WindowState.Normal
            };

            if (File.Exists(WindowSettingsFileName))
            {
                string json = File.ReadAllText(WindowSettingsFileName);
                WindowSettings windowSettings = JsonSerializer.Deserialize<WindowSettings>(json);
                if(windowSettings == null)
                {
                    return defaultWindowSettings;
                }
                return windowSettings;
            }
            else
            {
                return defaultWindowSettings;
            }
        }
        public AppSettings LoadAppSettings()
        {
            AppSettings defaultAppSettings = new AppSettings()
            {
                DefaultUserId = 0
            };

            if (File.Exists(AppSettingsFileName))
            {
                string json = File.ReadAllText(AppSettingsFileName);
                AppSettings? appSettings = JsonSerializer.Deserialize<AppSettings>(json);
                if (appSettings == null)
                {
                    SaveAppSettings(defaultAppSettings);
                    return defaultAppSettings;
                }
                return appSettings;
            }

            SaveAppSettings(defaultAppSettings);
            return defaultAppSettings;
        }
    }
}
