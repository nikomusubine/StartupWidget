using StartupWidget.Models.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StartupWidget.Models.Settings
{
    [Serializable]
    public class GeneralSettings
    {
        public GeneralSettings()
        {
            Font = new SerializableFont();
            Language = CultureInfo.CurrentUICulture.Name;
        }

        public SerializableFont Font { get; set; }

        public string Language { get; set; }

        public bool IsStartup { get; set; }
    }
}
