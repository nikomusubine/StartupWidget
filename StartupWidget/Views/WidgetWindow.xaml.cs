using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Settings;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.ViewModels;
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
    /// WidgetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WidgetWindow : Window
    {
        bool isRegisteredOnce = false;
        HotkeyRegisterService? hotkeyRegisterService;
        const int HotkeyID = 0x2502;
        const int WM_HOTKEY = 0x0312;
        public WidgetWindow()
        {
            InitializeComponent();
            this.IsVisibleChanged += VisibleChangedHandler;
            this.Loaded += LoadedHandler;
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { _ReloadHotkey(); });
        }

        private IntPtr Handle
        {
            get
            {
                var helper = new System.Windows.Interop.WindowInteropHelper(this);
                return helper.Handle;
            }
        }
        private void _ReloadHotkey()
        {
            ShortcutKeys keys = ((WidgetWindowViewModel)DataContext).ShortcutKeys;
            
            if (hotkeyRegisterService != null && !hotkeyRegisterService.disposed)
            {
                hotkeyRegisterService.Dispose();
            }
            if (keys.Keys != Key.None && keys.Modifires != KeyModifiers.None)
            {
                if (this.Handle == IntPtr.Zero)
                {
                    return;
                }
                hotkeyRegisterService = new HotkeyRegisterService(this.Handle,
                                                                    HotkeyID,
                                                                    keys.Modifires,
                                                                    keys.Keys);
                hotkeyRegisterService.HotKeyPressed += HotKeyPressedHandler;
            }
        }
        private void _DisposeHotkey()
        {
            if (hotkeyRegisterService != null)
            {
                hotkeyRegisterService.Dispose();
            }
        }

        private void ReloadHotkey(bool isShow)
        {
            if (isShow)
            {
                _ReloadHotkey();
            }
            else
            {
                _DisposeHotkey();
            }
        }

        private void VisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReloadHotkey((bool)e.NewValue);
            ((WidgetWindowViewModel)DataContext).HWnd = this.Handle;
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
        }

        private void HotKeyPressedHandler(object? sender, EventArgs e)
        {
            if (!this.IsActive)
            {
                this.Show();
                this.WindowState= WindowState.Normal;
                this.Activate();
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }
        private void ListViewItem_PreviewMouseRightButtonUp(object sender, MouseEventArgs e)
        {
            ((WidgetWindowViewModel)DataContext).RightClickCommand.Execute(sender);
        }
    }
}
