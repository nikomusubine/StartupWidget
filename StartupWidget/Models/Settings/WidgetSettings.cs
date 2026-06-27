using StartupWidget.Attributes;
using StartupWidget.Models.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace StartupWidget.Models.Settings
{
    public class WidgetSettings
    {
        public WidgetSettings()
        {
            Transparency = 0.0f;
            BackgroundImageFilePath = "";
            ForegroundColor = Color.Black;
            BackgroundColor = Color.White;
            WindowSize = 300;

            ShortcutKeys = new ShortcutKeys();

        }
        [Attributes.StringOption("Image File(*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr)|" +
                         "*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr")]
        public string BackgroundImageFilePath { get; set; }
        [XmlIgnore] public Color ForegroundColor { get; set; }
        [SettingElementIgnore]
        public string ForegroundColorSetting
        {
            get
            {
                return ColorTranslator.ToHtml(ForegroundColor);
            }
            set
            {
                ForegroundColor = ColorTranslator.FromHtml(value);
            }
        }

        [XmlIgnore] public Color BackgroundColor { get; set; }

        [SettingElementIgnore]
        public string BackgroundColorSetting
        {
            get
            {
                return ColorTranslator.ToHtml(BackgroundColor);
            }
            set
            {
                BackgroundColor = ColorTranslator.FromHtml(value);
            }
        }
        [IntBounds(0,10000)]
        public int WindowSize { get; set; } 

        [FloatBounds(0.0f, 100f)]
        public float Transparency { get; set; }
        [XmlIgnore]public ShortcutKeys ShortcutKeys { get; set; }

        [SettingElementIgnore]
        public string ShortcutKeySetting
        {
            get
            {
                return ShortcutKeys.ToString();
            }
            set
            {
                ShortcutKeys = ShortcutKeys.Parse(value);
            }

        }
        [SettingElementIgnore] public bool IsWidgetShow { get; set; }
        [SettingElementIgnore] public Position WidgetPosition { get; set; }
    }
}
