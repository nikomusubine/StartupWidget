using StartupWidget.Attributes;
using StartupWidget.Models.Types;
using System;
using System.Collections.Generic;
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
            BackgroundColor = Color.Black;
            BackgroundColor = Color.White;
            WindowSize = 300;
            IsMagnet = true;
            IsFixed = false;
            MagnetThreshold = 12;


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
                return ForegroundColor.ToHtml();
            }
            set
            {
                Color color;
                if (Color.TryFromHtml(value, out color))
                {
                    ForegroundColor = color;
                }
                else
                {
                    ForegroundColor = Color.Black;
                }
            }
        }

        [XmlIgnore] public Color BackgroundColor { get; set; }

        [SettingElementIgnore]
        public string BackgroundColorSetting
        {
            get
            {
                return BackgroundColor.ToHtml();
            }
            set
            {
                Color color;
                if (Color.TryFromHtml(value, out color))
                {
                    BackgroundColor = color;
                }
                else
                {
                    BackgroundColor = Color.White;
                }
            }
        }
        [IntBounds(0, 10000)]
        public int WindowSize { get; set; }

        [DoubleBounds(0.0f, 100f)]
        public double Transparency { get; set; }
        [XmlIgnore] public ShortcutKeys ShortcutKeys { get; set; }

        public bool IsFixed { get; set; }
        public bool IsMagnet { get; set; }
        [DoubleBounds(0.0f, 100f)]
        public double MagnetThreshold { get; set; }


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