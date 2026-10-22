using StartupWidget.Attributes;
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
        [LanguageElement]
        public string Language { get; set; }

        [DoubleBounds(16,512)]
        public double IconSize { get; set; } = 48;
        public bool IsStartup { get; set; }

    }
}
