using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace StartupWidget.Models.Settings
{
    public class MainWindowSettings
    {
        public MainWindowSettings()
        {
            BackgroundImageFilePath = "";
            ForegroundColor = Color.Black;
            BackgroundColor = Color.White;
        }
        [Attributes.StringOption("(*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr)|" +
                                 "*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr")]
        public string BackgroundImageFilePath { get; set; }
        [XmlIgnore] public Color ForegroundColor { get; set; }
        [Attributes.SettingElementIgnore]public string ForegroundColorSetting
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

        [Attributes.SettingElementIgnore]
        public string BackgroundColorSetting
        {
            get
            {
                return ColorTranslator.ToHtml(BackgroundColor);
            }
            set
            {
                BackgroundColor=ColorTranslator.FromHtml(value);
            }
        }
    }
}
