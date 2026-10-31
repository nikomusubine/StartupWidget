using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;

namespace StartupWidget.Models.Types
{
    //[Serializable]
    public class SerializableFont
    {
        private const double defaultFontSizePt = 12;
        public SerializableFont(string name, double size)
        {
            Name = name;
            Size = size;
        }

        public SerializableFont()
        {
            FontFamily fontFamily = new FontFamily("XamlAutoFontFamily");
            Name = fontFamily.Source;
            UISettings uiSettings = new UISettings();
            Size = defaultFontSizePt * uiSettings.TextScaleFactor;
        }
        public string Name { get; set; }
        public double Size { get; set; }
    }
}
