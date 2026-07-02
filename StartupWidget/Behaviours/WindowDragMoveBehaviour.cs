using Microsoft.Xaml.Behaviors;
using StartupWidget.Behaviours.Win32Helpers;
using System;
using System.Windows;
using System.Windows.Input;

namespace StartupWidget.Behaviours
{
    public class WindowDragMoveBehaviour: Behavior<Window>
    {
        private System.Windows.Point _mouseClickPoint;
        private bool _moving = false;
        WindowMove? WindowMove;

        public static readonly DependencyProperty IsFixedProperty = DependencyProperty.Register(
                    nameof(IsFixed),
                    typeof(bool),
                    typeof(WindowDragMoveBehaviour),
                    new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsFixedChanged)
                );

        public bool IsFixed
        {
            get => (bool)GetValue(IsFixedProperty);
            set => SetValue(IsFixedProperty, value);
        }

        private static void OnIsFixedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowDragMoveBehaviour behavior && behavior.WindowMove != null)
            {
                behavior.WindowMove.IsMoveEnabled = !(bool)e.NewValue;
            }
        }
        public static readonly DependencyProperty IsMagnetEnabledProperty = DependencyProperty.Register
        (
            nameof(IsMagnetEnabled),
            typeof(bool),
            typeof(WindowDragMoveBehaviour),
            new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsMagnetEnabledChanged)
        );
        public bool IsMagnetEnabled {
            get=>(bool)GetValue(IsMagnetEnabledProperty);
            set => SetValue(IsMagnetEnabledProperty, value);
        }
        private static void OnIsMagnetEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowDragMoveBehaviour behavior && behavior.WindowMove != null)
            {
                behavior.WindowMove.IsMagnetEnabled = (bool)e.NewValue;
            }
        }


        public static readonly DependencyProperty MagnetThresholdProperty = DependencyProperty.Register
        (
            nameof(MagnetThreshold),
            typeof(double),
            typeof(WindowDragMoveBehaviour),
            new FrameworkPropertyMetadata(12.0,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMagnetThresholdChanged)
        );
        public double MagnetThreshold
        {
            get => (double)GetValue(MagnetThresholdProperty);
            set => SetValue(MagnetThresholdProperty, value);
        }
        private static void OnMagnetThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowDragMoveBehaviour behavior && behavior.WindowMove != null)
            {
                behavior.WindowMove.MagnetThreshold = (double)e.NewValue;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            WindowMove = new WindowMove();
            WindowMove.MagnetThreshold = MagnetThreshold;
            WindowMove.IsMagnetEnabled = IsMagnetEnabled;
            WindowMove.IsMoveEnabled = !IsFixed;
    
            // Windowの左クリックイベントを監視
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.SourceInitialized += OnSourceInitialized;
            WindowMove.OnMouseUp += OnMouseButtonUp;
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            WindowMove!.SetHwnd(new System.Windows.Interop.WindowInteropHelper(AssociatedObject).Handle);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
            WindowMove!.OnMouseUp -= OnMouseButtonUp;
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
        private void OnMouseButtonUp(object sender, EventArgs e)
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
                    WindowMove!.DragMove();
                }
            }
        }
    }
}
