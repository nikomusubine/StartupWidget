using CommunityToolkit.Mvvm.Messaging;
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
    /// AddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<Messages.CloseAddWindow>(this, (r, m) => this.Close());
        }
    }
}
