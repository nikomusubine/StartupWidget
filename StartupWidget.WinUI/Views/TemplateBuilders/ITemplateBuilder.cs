using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    internal interface ITemplateBuilder
    {
        FrameworkElement Content { get; }
    }
}
