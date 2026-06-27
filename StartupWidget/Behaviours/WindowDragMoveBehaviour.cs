using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace StartupWidget.Behaviours
{
    public class WindowDragMoveBehaviour: Behavior<Window>
    {
        private System.Windows.Point _mouseClickPoint;
        private bool _moving = false;
        protected override void OnAttached()
        {
            base.OnAttached();
            // Windowの左クリックイベントを監視
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                _mouseClickPoint=e.GetPosition((IInputElement)sender);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _moving = false;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_moving)
            {
                double MinimumHorizonalDragDistance = SystemParameters.MinimumHorizontalDragDistance;
                double MinimumVerticalDragDistance = SystemParameters.MinimumVerticalDragDistance;
                if (Math.Abs(e.GetPosition((IInputElement)sender).X - _mouseClickPoint.X) > MinimumHorizonalDragDistance ||
                    Math.Abs(e.GetPosition((IInputElement)sender).Y - _mouseClickPoint.Y) > MinimumVerticalDragDistance)
                {
                    _moving = true;
                    AssociatedObject.DragMove();
                }
            }
        }
    }
}
