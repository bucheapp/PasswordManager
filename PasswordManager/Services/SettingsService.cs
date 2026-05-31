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
        public WindowSettings? LoadWindowSettings()
        {
            if (File.Exists(WindowSettingsFileName))
            {
                string json = File.ReadAllText(WindowSettingsFileName);
                return JsonSerializer.Deserialize<WindowSettings>(json);
            }
            else
            {
                WindowSettings windowSettings = new WindowSettings();
                windowSettings.Width = 800;
                windowSettings.Height = 450;
                windowSettings.Left = null;
                windowSettings.Top = null;
                windowSettings.WindowState = WindowState.Normal;
                return windowSettings;
            }
        }
        public AppSettings? LoadAppSettings()
        {
            if (File.Exists(AppSettingsFileName))
            {
                string json = File.ReadAllText(AppSettingsFileName);
                return JsonSerializer.Deserialize<AppSettings>(json);
            }

            return null;
        }
    }
}
