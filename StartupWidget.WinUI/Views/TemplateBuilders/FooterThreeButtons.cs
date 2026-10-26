using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using StartupWidget.WinUI.Views.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    public class FooterThreeButtons: Grid
    {
        SharedSizeGroup sharedSize = null!;
        public FooterThreeButtons()
        {
            CreateButtons();
            this.Loaded += CalculateSizeOnLoad;
        }
        public Button OKButton = null!;
        public Button ApplyButton = null!;
        public Button CancelButton = null!;

        private void CalculateSizeOnLoad(object sender, RoutedEventArgs e)
        {
            sharedSize.Calculate();
        }

        private void CreateButtons()
        {
            sharedSize = new(SizeType.Width); 

            OKButton = new Button
            {
                Content = "OK",
            };
            Grid.SetColumn(OKButton, 0);
            sharedSize.Register(OKButton);
            ApplyButton = new Button
            {
                Content = "Apply",
            };
            Grid.SetColumn(ApplyButton, 1);
            sharedSize.Register(ApplyButton);
            CancelButton = new Button
            {
                Content = "Cancel",
            };
            Grid.SetColumn(CancelButton, 2);
            sharedSize.Register(CancelButton);

            Margin = new Thickness(10);
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Right;
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
            Height = 35;
            ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto,});
            ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto,});
            ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto,});
            Children.Add(OKButton);
            Children.Add(ApplyButton);
            Children.Add(CancelButton);
        }
    }
}
