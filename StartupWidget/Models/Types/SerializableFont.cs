using System.Windows;
using System.Windows.Media;

namespace StartupWidget.Models.Types
{
    [Serializable]
    public class SerializableFont
    {
        public SerializableFont(string name, float size)
        {
            Name = name;
            Size = size;
        }

        public SerializableFont()
        {
            Name = SystemFonts.MessageFontFamily.Source;
            Size = (float)SystemFonts.MessageFontSize;
        }
        public string Name { get; set; }
        public float Size { get; set; }
    }
}
