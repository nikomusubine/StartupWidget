using StartupWidget.Models.Settings;
using StartupWidget.Models.Types;
using StartupWidget.Services.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StartupWidget.Services
{
    public class SettingsService
    {
        private static string SettingsSavePath = Constants.SettingsLocation + "Settings.xml";
        public SettingsService()
        {
            GeneralSettings = new GeneralSettings();
            MainWindowSettings = new MainWindowSettings();
            WidgetSettings = new WidgetSettings();
            persistenceService = new PersistenceService(typeof(SettingsService), SettingsSavePath);
        }
        private PersistenceService persistenceService;
        public GeneralSettings GeneralSettings { get; set; }
        public MainWindowSettings MainWindowSettings { get; set; }
        public WidgetSettings WidgetSettings { get; set; }


        public void UpdateSettings(GeneralSettings generalSettings, MainWindowSettings mainWindowSettings, WidgetSettings widgetSettings)
        {
            GeneralSettings = generalSettings;
            MainWindowSettings = mainWindowSettings;
            WidgetSettings = widgetSettings;
            SaveSettings();
        }
        public void SaveSettings()
        {
            persistenceService.Save(this);
        }
        public void LoadSettings()
        {
            if (File.Exists(SettingsSavePath))
            {
                SettingsService? settings = (SettingsService?)persistenceService.Load();
                if (settings != null)
                {
                    GeneralSettings = settings.GeneralSettings;
                    MainWindowSettings = settings.MainWindowSettings;
                    WidgetSettings = settings.WidgetSettings;
                }
            }
        }
    }
}
