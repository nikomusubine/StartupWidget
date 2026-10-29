using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using StartupWidget.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StartupWidget.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainWindowViewModel? __viewModel;
        public MainWindowViewModel? ViewModel 
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
        public MainPage()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
        }

        private void InitializeUI()
        {
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
                Text = "設定(_S)"
            });

            menubar.Items.Add(Option);

            return menubar;
        }

        private FrameworkElement CreateBody()
        {
            return new ListView()
            {
                Margin = new Thickness(5, 10, 5, 10),
                FontSize = 12,
                ItemsSource = new List<string>{
                    "hoge","hoge2","hoge3"
                }
            };
        }
    }
}
