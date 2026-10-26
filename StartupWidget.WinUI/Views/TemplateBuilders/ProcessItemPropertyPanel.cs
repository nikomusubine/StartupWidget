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
            this.Loaded += CalculateSizeOnLoad;
        }

        public FrameworkElement Content
        {
            get
            {
                return Root;
            }
        }
        private SharedSizeGroup sharedSizeGroup = null!;

        private StackPanel Root=null!;
        private OptionElement NameControl = null!;
        private TextBox NameBox = null!;
        private OptionElement ProcessPathControl = null!;
        private FilePathTextBox ProcessPathBox= null!;
        private OptionElement ArgumentControl = null!;
        private TextBox ArgumentBox = null!;
        private OptionElement WorkingDirectoryControl = null!;
        private FilePathTextBox WorkingDirectoryBox = null!;
        private OptionElement IconPathControl = null!;
        private FilePathTextBox IconPathBox = null!;
        private OptionElement RunAsAdminControl = null!;
        private CheckBox RunAsAdminBox = null!;
        private Button RemoveAppButton = null!;

        private void CalculateSizeOnLoad(object sender, RoutedEventArgs e)
        {
            sharedSizeGroup.Calculate();
        }
        private void CreatePanel()
        {
            CreatePanelContent();

            Margin = new Thickness(5, 0, 5, 0);
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            sharedSizeGroup = new SharedSizeGroup(SizeType.Width);
            Children.Add(NameControl);
            NameControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(ProcessPathControl);
            ProcessPathControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(ArgumentControl);
            ArgumentControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(WorkingDirectoryControl);
            WorkingDirectoryControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(IconPathControl);
            IconPathControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(RunAsAdminControl);
            RunAsAdminControl.JoinSharedSizeGroup(sharedSizeGroup);
            Children.Add(RemoveAppButton);
        }

        private void CreatePanelContent()
        {
            NameBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            NameControl = new OptionElement
            {
                Label="Name",
                
                ValueContent=NameBox,                
                HorizontalAlignment=HorizontalAlignment.Stretch,
            };
            
            ProcessPathBox = new FilePathTextBox();
            ProcessPathControl = new OptionElement
            {
                Label = "FilePath",
                HorizontalAlignment=HorizontalAlignment.Stretch,
                ValueContent = ProcessPathBox
            };

            ArgumentBox = new TextBox();
            ArgumentControl = new OptionElement
            {
                Label = "Argument",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ValueContent = ArgumentBox
            };

            WorkingDirectoryBox = new FilePathTextBox();
            WorkingDirectoryControl = new OptionElement
            {
                Label = "WorkingDirectory",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ValueContent = WorkingDirectoryBox,
            };

            IconPathBox = new FilePathTextBox();
            IconPathControl = new OptionElement
            {
                Label = "Icon",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ValueContent = IconPathBox
            };

            RunAsAdminBox = new CheckBox();
            RunAsAdminControl = new OptionElement
            {
                Label = "Run as Admin",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ValueContent = RunAsAdminBox
            };

            RemoveAppButton = new Button
            {
                Content="Remove this app",
            };


        }
    }
}
