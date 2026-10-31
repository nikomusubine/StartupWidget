using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using StartupWidget.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace StartupWidget.WinUI.Views
{
    public partial class MainWindow: Window
    {

        public Frame RootFrame = null!;
        public MainWindow()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Title = "StartupWidget";

            RootFrame = new Frame();
            this.Content = RootFrame;
        }
    }
}
