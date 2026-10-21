using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views
{
    public class SettingsWindow : Window
    {
        public SettingsWindow()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Title = "Settings - StartupWidget";
            var appWindow = this.AppWindow;
            BuildUI();
        }

        private void BuildUI()
        {
            // 1. ルートとなるレイアウトパネルの作成
            FrameworkElement body = CreateBody();
            Grid.SetRow(body, 0);
            FrameworkElement footer = CreateFooter();
            Grid.SetRow(footer, 1);

            var rootGrid = new Grid
            {
                Padding = new Thickness(0),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                RowDefinitions = {
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    },new RowDefinition
                    {
                        Height = GridLength.Auto
                    }
                },
                Children =
                {
                    body,
                    footer,
                }
            };
            this.Content = rootGrid;

        }

        private FrameworkElement CreateBody()
        {
            TabView tabView = new TabView
            {
                Margin = new Thickness(10,10,10,40),
                IsAddTabButtonVisible=false,
                CanReorderTabs=false,
            };

            TabViewItem general = new TabViewItem
            {
                IsClosable = false,
            };
            general.Header = "General";
            tabView.TabItems.Add(general);


            TabViewItem mainWindow = new TabViewItem
            {
                IsClosable = false,
            };
            mainWindow.Header = "MainWindow";
            tabView.TabItems.Add(mainWindow);

            TabViewItem widget = new TabViewItem
            {
                IsClosable = false,
            };
            widget.Header = "Widget";
            tabView.TabItems.Add(widget);
            return tabView;
        }

        private FrameworkElement CreateFooter()
        {

            Button OKButton = new Button
            {
                Content = "OK",
            };
            Grid.SetColumn(OKButton, 0);
            Button ApplyButton = new Button
            {
                Content = "Apply",
            };
            Grid.SetColumn(ApplyButton, 1);
            Button CancelButton = new Button
            {
                Content = "Cancel",
            };
            Grid.SetColumn(CancelButton, 2);

            Grid grid = new Grid
            {
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                Height = 35,
                ColumnDefinitions =
                {
                   new ColumnDefinition
                    {
                        Width = GridLength.Auto,
                    },new ColumnDefinition
                    {
                        Width = GridLength.Auto,
                    },new ColumnDefinition
                    {
                        Width = GridLength.Auto,
                    }
                },
                Children =
                {
                    OKButton, CancelButton, ApplyButton
                },
            };
            return grid;
        }
    }
}
