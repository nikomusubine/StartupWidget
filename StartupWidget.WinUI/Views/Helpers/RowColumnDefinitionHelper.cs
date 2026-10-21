using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Views.Helpers
{
    public static class RowColumnDefinitionHelper
    {
        public static RowDefinition RowDefinitionStar(double ratio)
        {

            RowDefinition rowDefinition = new RowDefinition
            {
                Height = GridLengthStar(ratio)
            };
            return rowDefinition;
        }

        public static ColumnDefinition ColumnDefinitionStar()
        {
            return ColumnDefinitionStar(1);
        }
        public static ColumnDefinition ColumnDefinitionStar(double ratio)
        {

            ColumnDefinition columnDefinition = new ColumnDefinition
            {
                Width = GridLengthStar(ratio)
            };
            return columnDefinition;
        }

        public static GridLength GridLengthStar(double ratio)
        {
            return new GridLength(ratio, GridUnitType.Star);
        }
    }
}
