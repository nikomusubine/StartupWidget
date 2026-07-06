using System.Drawing;

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
            Font font = SystemFonts.DefaultFont;
            Name = font.Name;
            Size = font.Size;
        }
        public string Name { get; set; }
        public float Size { get; set; }
        
        public static explicit operator Font(SerializableFont font)
        {
            return new Font(font.Name, font.Size);
        }

        public static explicit operator SerializableFont(Font font)
        {
            return new SerializableFont(font.Name, font.Size);
        }

    }
}
