using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget
{
    public static class Constants
    {
#if DEBUG
        public static string SettingsLocation = Environment.SpecialFolder.LocalApplicationData + "\\Nikochan\\StartupWidget\\";
#else
        public static string SettingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,Environment.SpecialFolderOption.Create) + "\\Nikochan\\StartupWidget\\";
#endif
        public const string AppName = "nikomusubine.startupwidget";

        public const nuint AppUniqueNumber = 0x2503;
    }
}
