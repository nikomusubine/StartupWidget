using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using StartupWidget.ViewModels;
using StartupWidget.WinUI.Views.Helpers;
using StartupWidget.WinUI.Views.TemplateBuilders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StartupWidget.WinUI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private SettingsWindowViewModel? __viewModel;
        public SettingsWindowViewModel? ViewModel
        {
            get
            {
                return __viewModel;
            }
            set
            {
                __viewModel = value;
                DataContext = value;
            }
        }
        public SettingsPage()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            // 1. ルートとなるレイアウトパネルの作成
            FrameworkElement body = CreateBody();
            Grid.SetRow(body, 0);
            FooterThreeButtons footer = new FooterThreeButtons();
            Grid.SetRow(footer, 1);

            var rootGrid = new Grid
            {
                Padding = new Thickness(0),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                RowDefinitions = {
                    RowColumnDefinitionHelper.RowDefinitionStar(),
                    new RowDefinition
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
                Margin = new Thickness(10, 10, 10, 40),
                IsAddTabButtonVisible = false,
                CanReorderTabs = false,
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
    }
}
