using System.Windows;
using System.Windows.Media;

namespace StartupWidget.Models.Types
{
    [Serializable]
    public class SerializableFont
    {
        public SerializableFont(string name, double size)
        {
            Name = name;
            Size = size;
        }

        public SerializableFont()
        {
            Name = SystemFonts.MessageFontFamily.Source;
            Size = (double)SystemFonts.MessageFontSize;
        }
        public string Name { get; set; }
        public double Size { get; set; }
    }
}
