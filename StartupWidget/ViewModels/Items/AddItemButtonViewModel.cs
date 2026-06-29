using StartupWidget.Models.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels.Items
{
    public class AddItemButtonViewModel : ProcessItemViewModel
    {
        public AddItemButtonViewModel(string Name) : base (new ExecutableProcess(Name, ""))
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
            geometryGroup.FillRule = FillRule.Nonzero;

            SolidColorBrush brush = new (Color.FromArgb(0xFF,150, 150, 150));
            System.Windows.Media.Pen pen = new(brush,1);


            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(5.3, 1), new System.Windows.Point(15.5, 1))); //上辺
            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(15, 0.5), new System.Windows.Point(15, 15.5))); //右辺
            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(0.5, 15), new System.Windows.Point(15, 15))); //底辺
            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(1, 15), new System.Windows.Point(1, 5.2))); //左辺
            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(5.5, 1), new System.Windows.Point(1, 5.5))); //斜辺

            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(8, 3), new System.Windows.Point(8, 13))); //縦線
            geometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(3, 8), new System.Windows.Point(13, 8))); //横線

            GeometryDrawing drawing = new GeometryDrawing(brush, pen, geometryGroup);

            return new DrawingImage(drawing);
        }
    }
}