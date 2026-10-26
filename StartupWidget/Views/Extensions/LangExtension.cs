using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;

namespace StartupWidget.Views.Extensions
{
    public class LangExtension: Binding
    {
        public LangExtension(string path) 
        {
            XPath = $"/Language/{path}/@Text";
            Path = new PropertyPath("Value");
            Source = Application.Current.Resources["Lang"];
        }
    }
}
