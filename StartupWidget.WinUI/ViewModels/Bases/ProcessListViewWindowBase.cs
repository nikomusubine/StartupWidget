using CommunityToolkit.Mvvm.ComponentModel;
using StartupWidget.Models.Settings;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace StartupWidget.ViewModels.Bases
{
    public partial class ProcessListViewWindowBase: ObservableObject
    {
        protected SettingsService SettingsService;
        protected GeneralSettings generalSettings;
        public ProcessListViewWindowBase(SettingsService settings)
        {
            SettingsService = settings;
            generalSettings = SettingsService.GeneralSettings;
        }


        public double IconSize
        {
            get => generalSettings.IconSize;
        }

        public double ProcessItemWidth
        {
            get
            {
                return generalSettings.IconSize * 1.8;
            }
        }
        public double TextHeight
        {
            get
            {
                return generalSettings.Font.Size * 3.5;
            }
        }
        public double ProcessItemHeight
        {
            get
            {
                return IconSize+TextHeight+3;
            }
        }
    }
}
