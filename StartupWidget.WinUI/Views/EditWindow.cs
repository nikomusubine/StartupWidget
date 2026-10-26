using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using StartupWidget.ViewModels;
using StartupWidget.WinUI.Views.Helpers;
using StartupWidget.WinUI.Views.TemplateBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views
{
    public class EditWindow : Window
    {
        public EditWindowViewModel DataContext = null!;
        public EditWindow()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
        }

        ProcessItemPropertyPanel propertyPanel = null!;
        private void InitializeUI()
        {
            this.Title = "Edit - StartupWidget";
            var appWindow = this.AppWindow;
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
