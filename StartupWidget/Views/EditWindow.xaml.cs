using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StartupWidget.Views
{
    /// <summary>
    /// EditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<CloseSettingsWindow>(this, (r, m) => this.Close());
        }
    }
}
