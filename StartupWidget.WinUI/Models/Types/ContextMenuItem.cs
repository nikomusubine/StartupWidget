using Microsoft.UI.Xaml.Media;
using System;

namespace StartupWidget.Models.Types
{
    public delegate void MenuClickEventHandler(object sender, EventArgs e);
    public class ContextMenuItem : ContextMenuItemBase
    {
        public string Header { get; set; }
        public ImageSource? Icon { get; set; }
        public event MenuClickEventHandler? MenuClicked;

        public ContextMenuItem(string header, ImageSource? icon)
        {
            Header = header;
            Icon = icon;
        }
        public ContextMenuItem(string header) : this(header, null)
        {
        }

        public virtual void MenuClick()
        {
            MenuClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}