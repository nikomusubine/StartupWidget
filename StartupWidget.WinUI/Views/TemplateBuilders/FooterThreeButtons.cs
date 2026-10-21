using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    public class FooterThreeButtons: ITemplateBuilder
    {
        public FooterThreeButtons()
        {
            CreateButtons();
        }

        public FrameworkElement Content
        {
            get
            {
                return Rootgrid;
            }
        }

        public Grid Rootgrid = null!;
        public Button OKButton = null!;
        public Button ApplyButton = null!;
        public Button CancelButton = null!;

        private void CreateButtons()
        {
            OKButton = new Button
            {
                Content = "OK",
            };
            Grid.SetColumn(OKButton, 0);
            ApplyButton = new Button
            {
                Content = "Apply",
            };
            Grid.SetColumn(ApplyButton, 1);
            CancelButton = new Button
            {
                Content = "Cancel",
            };
            Grid.SetColumn(CancelButton, 2);

            Rootgrid = new Grid
            {
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
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
        }
    }
}
