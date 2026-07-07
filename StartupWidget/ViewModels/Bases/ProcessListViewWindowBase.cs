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
                double type1,type2;
                type1 = (generalSettings.Font.Size / 2) * 15.4;
                type2 = generalSettings.IconSize * 1.8;
                return Math.Max(type1, type2);
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
