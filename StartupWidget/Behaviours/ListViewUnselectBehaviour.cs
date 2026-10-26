using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StartupWidget.Behaviours
{
    public class ListViewUnselectBehaviour : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseUp += MouseLeftButtonUpHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject?.MouseUp -= MouseLeftButtonUpHandler;
        }

        private void MouseLeftButtonUpHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && !(obj is ListViewItem)) 
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (obj == null)
            {
                AssociatedObject.UnselectAll();
            }
        }
    }
}
