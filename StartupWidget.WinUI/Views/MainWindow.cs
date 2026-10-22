using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

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
        public MainWindow()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
            WidgetWindow widget = new WidgetWindow();
            widget.Activate();

            SettingsWindow settings = new();
            settings.Activate();

            EditWindow edit = new();
            edit.Activate();
        }

        private void InitializeUI()
        {
            this.Title = "StartupWidget";
            BuildUI();
        }

        private void BuildUI()
        {
            // 1. ルートとなるレイアウトパネルの作成
            FrameworkElement menu = CreateMenu();
            Grid.SetRow(menu, 0);
            FrameworkElement body = CreateBody();
            Grid.SetRow(body, 1);

            var rootGrid = new Grid
            {
                Padding = new Thickness(20),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                RowDefinitions = {
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    },
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    },
                },
                Children =
                {
                    menu,
                    body,
                }
            };
            this.Content = rootGrid;

        }

        private FrameworkElement CreateMenu()
        {
            MenuBar menubar = new MenuBar();

            MenuBarItem Option = new MenuBarItem
            {
                Title = "オプション(_O)",
            };


            Option.Items.Add(new MenuFlyoutItem()
            {
                Text="設定(_S)"
            });

            menubar.Items.Add(Option);

            return menubar;
        }

        private FrameworkElement CreateBody()
        {
            return new ListView()
            {
                Margin = new Thickness(5, 10, 5, 10),
                FontSize=12,
                ItemsSource = new List<string>{
                    "hoge","hoge2","hoge3"
                }
            };
        }

    }
}
