using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows;
using Application = System.Windows.Application;
using System.Windows.Data;

namespace StartupWidget.Services
{
    public class LanguageService
    {

        public LanguageService()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="langCode"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ChangeLanguage(string langCode)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", $"{langCode}.xml");

            if (!File.Exists(xmlPath))
            {
                throw new FileNotFoundException(xmlPath);
            }
            XmlDocument? _currentDocument;
            _currentDocument = new XmlDocument();
            _currentDocument.Load(xmlPath);

            if (Application.Current.Resources["Lang"] is XmlDataProvider provider)
            {
                provider.Document = _currentDocument;
                provider.Refresh();
            }
        }
        public static string GetString(string xpath, params object[] args)
        {
            if (Application.Current.Resources["Lang"] is XmlDataProvider provider)
            {
            XmlDocument? _currentDocument = provider.Document;

                if (_currentDocument == null) return $"[{xpath}]";

                XmlNode? node = _currentDocument.SelectSingleNode($"Language/{xpath}/@Text");
                return (node != null) ? string.Format(node.InnerText, args) : $"[{xpath}]";
            }
            else
            {
                return $"[{xpath}]";
            }
        }


        public static List<string> GetAvailableLanguages()
        {
            List<string> list = new();
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");

            if (!Directory.Exists(dir)) return list;

            foreach(var file in Directory.GetFiles(dir, "*.xml"))
            {
                string langCode= Path.GetFileNameWithoutExtension(file);

                try
                {
                    CultureInfo culture = CultureInfo.GetCultureInfo(langCode);
                    list.Add(culture.NativeName);
                }catch (CultureNotFoundException)
                {
                    //不適切なファイル名は無視
                }
            }
            return list;
        }
    }
}
