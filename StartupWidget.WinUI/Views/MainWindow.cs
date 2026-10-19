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
    public partial class MainWindow: Window
    {
        private int _clickCount = 0;
        public MainWindow()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Title = "StartupWidget - C# UI";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(CreateMenu());
            // 1. ルートとなるレイアウトパネルの作成
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
                    //CreateMenu(),
                    //CreateBody()
                }
            };

            this.Content = rootGrid;
            
        }

        private UIElement CreateMenu()
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

        private UIElement CreateBody()
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
