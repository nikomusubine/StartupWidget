using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StartupWidget.WinUI.Views.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    public class ProcessItemPropertyPanel:StackPanel
    {
        public ProcessItemPropertyPanel()
        {
            CreatePanel();
        }

        public FrameworkElement Content
        {
            get
            {
                return Root;
            }
        }

        private StackPanel Root=null!;
        private ContentControl NameControl = null!;
        private TextBox NameBox = null!;
        private ContentControl ProcessPathControl = null!;
        private FilePathTextBox ProcessPathBox= null!;
        private ContentControl ArgumentControl = null!;
        private TextBox ArgumentBox = null!;
        private ContentControl WorkingDirectoryControl = null!;
        private TextBox WorkingDirectoryBox = null!;
        private ContentControl IconPathControl = null!;
        private TextBox IconPathBox = null!;
        private ContentControl RunAsAdminControl = null!;
        private CheckBox RunAsAdminBox = null!;
        private Button RemoveAppButton = null!;

        private void CreatePanel()
        {
            CreatePanelContent();

            Margin = new Thickness(5, 0, 5, 0);
            this.HorizontalAlignment = HorizontalAlignment.Stretch;            
            Children.Add(NameControl);
            Children.Add(ProcessPathControl);
        }

        private void CreatePanelContent()
        {
            NameBox = new TextBox();
            NameControl = new ContentControl
            {
                Tag="Name",
                Content=NameBox,                
                HorizontalAlignment=HorizontalAlignment.Stretch,
            };
            ProcessPathBox = new FilePathTextBox();

            ProcessPathControl = new ContentControl
            {
                Tag = "FilePath",
                HorizontalAlignment=HorizontalAlignment.Stretch,
                Content = ProcessPathBox
            };
        }
    }
}
