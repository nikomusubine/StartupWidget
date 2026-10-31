using StartupWidget.Services;
using System;

namespace StartupWidget.Models.Types
{
    public readonly record struct Color
    {
        public static Color Black = new(0xFF, 0, 0, 0);
public static Color White = new(0xFF, 0xFF, 0xFF, 0xFF);
        public static Color Transparent = new(0, 0, 0, 0);

        readonly public byte A;
        readonly public byte R;
        readonly public byte G;
        readonly public byte B;

        public Color(byte a,byte r, byte g,byte b)
        {
            A = a;R = r;G = g;B = b;
        }

        public Color(Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public string ToHtml()
        {
            return $"#{A:X2}{R:X2}{G:X2}{B:X2}";
        }

        private static string ExceptionTextPath = "Exception/Color/FromHtml/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static Color FromHtml(string htmlColor)
        {
            if (string.IsNullOrWhiteSpace(htmlColor))
                throw new FormatException(LanguageService.GetString(ExceptionTextPath+"EmptyText"));
            if (!htmlColor.StartsWith("#"))
                throw new FormatException(LanguageService.GetString(ExceptionTextPath+"NoSharp"));

            string hex = htmlColor.TrimStart('#');

            if (hex.Length == 6)
            {
                return new Color(
                    0xFF,
                    byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
                    );
            }
            else if (hex.Length == 8)
            {
                return new Color(
                    byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                    );

            }
            throw new FormatException(LanguageService.GetString(ExceptionTextPath+"InvalidText"));
        }

        public static bool TryFromHtml(string htmlColor, out Color color)
        {
            color = new Color();
            try
            {
                color=FromHtml(htmlColor);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public unsafe static Color FromArgb(int argb)
        {
            byte* ptr = (byte*)&argb;
            byte b = *ptr;
            byte g = *(ptr + 1);
            byte r = *(ptr + 2);
            byte a = *(ptr + 3);
            return new Color(a, r, g, b);
        }
    }
}
