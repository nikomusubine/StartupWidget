using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StartupWidget.WinUI.Views.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    public class OptionElement : UserControl
    {
        private Grid Rootgrid = null!;
        private TextBlock LabelBlock = null!;
        private ContentControl ValueControl = null!;
        public FrameworkElement ValueContent
        {
            get
            {
                return (FrameworkElement)ValueControl.Content;
            }
            set
            {
                ValueControl.Content = value;
            }
        }

        public double LabelWidth
        {
            get
            {
                return LabelBlock.Width;
            }
            set
            {
                LabelBlock.Width = value;
            }
        }
        public string Label
        {
            get
            {
                return LabelBlock.Text;
            }
            set
            {
                LabelBlock.Text = value;
            }
        }


        public OptionElement()
        {
            CreatePanel();
        }

        private void CreatePanel()
        {
            LabelBlock = new TextBlock();
            ValueControl = new ContentControl();

            Rootgrid = new Grid
            {
                Margin = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width=GridLength.Auto
                    },
                    RowColumnDefinitionHelper.ColumnDefinitionStar()
                },
                Children =
                {
                    LabelBlock,
                    ValueControl
                }
            };

            Grid.SetColumn(LabelBlock, 0);
            Grid.SetColumn(ValueControl, 1);

            this.Content = Rootgrid;
        }

    }
}
