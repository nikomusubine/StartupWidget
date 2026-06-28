using StartupWidget.Models.Types;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public class AddItemButtonViewModel : ProcessItemViewModel
    {
        public AddItemButtonViewModel(string Name) : base (new ExecutableProcess(Name, "", "", ""))
        {

        }

        public override ImageSource IconImage
        {
            get
            {
                return CreatePlusSignImage();
            }
        }

        private ImageSource CreatePlusSignImage()
        {
            GeometryGroup geometryGroup = new();
            geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(10, 4, 4, 16)));
            geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(4, 10, 16, 4)));

            SolidColorBrush brush = new (Color.FromArgb(0xFF,150, 150, 150));
            GeometryDrawing drawing = new GeometryDrawing(brush, null, geometryGroup);
            return new DrawingImage(drawing);
        }
    }
}