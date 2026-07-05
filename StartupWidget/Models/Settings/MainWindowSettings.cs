using System.Xml.Serialization;
using StartupWidget.Models.Types;

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
                return ForegroundColor.ToHtml();
            }
            set
            {
                ForegroundColor = Color.FromHtml(value);
            }
        }

        [XmlIgnore] public Color BackgroundColor { get; set; }

        [Attributes.SettingElementIgnore]
        public string BackgroundColorSetting
        {
            get
            {
                return BackgroundColor.ToHtml();
            }
            set
            {
                BackgroundColor=Color.FromHtml(value);
            }
        }
    }
}
