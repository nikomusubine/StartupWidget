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
    public sealed partial class EditPage : Page
    {
        private EditWindowViewModel? __viewModel;
        public EditWindowViewModel? ViewModel
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
        public EditPage()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
        }

        ProcessItemPropertyPanel propertyPanel = null!;
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
            FrameworkElement listview = CreateListView();
            propertyPanel = new ProcessItemPropertyPanel();
            Grid.SetColumn(listview, 0);
            Grid.SetColumn(propertyPanel, 1);
            Grid grid = new Grid
            {
                Margin = new Thickness(0, 20, 0, 45),
                ColumnDefinitions =
                {
                    RowColumnDefinitionHelper.ColumnDefinitionStar(1),
                    RowColumnDefinitionHelper.ColumnDefinitionStar(1),
                },
                Children =
                {
                    listview,
                    propertyPanel
                }
            };

            return grid;
        }

        private FrameworkElement CreateListView()
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
