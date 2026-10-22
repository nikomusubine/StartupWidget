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
    public class WidgetWindow: Window
    {
        public WidgetWindow()
        {
#if DEBUG
            StartupWidget.WinUI.DevService.HotReloadHelper.OnHotReload += BuildUI;
#endif

            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Title = "Widget - StartupWidget";
            var appWindow = this.AppWindow;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(316, 316));
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsResizable = false;
            }
            BuildUI();
        }

        private void BuildUI()
        {
            // 1. ルートとなるレイアウトパネルの作成
            FrameworkElement body = CreateBody();
            Grid.SetRow(body, 0);

            var rootGrid = new Grid
            {
                Padding = new Thickness(0),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                RowDefinitions = {
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    }
                },
                Children =
                {
                    body,
                }
            };
            this.Content = rootGrid;

        }    

        private FrameworkElement CreateBody()
        {
            return new ListView()
            {
                Margin = new Thickness(0),
                FontSize = 12,
                ItemsSource = new List<string>{
                    "hoge","hoge2","hoge3"
                }
            };
        }
    }
}
