using Microsoft.Xaml.Behaviors;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StartupWidget.Behaviours
{
    public class ShortcutKeySetBehaviour: Behavior<System.Windows.Controls.TextBox>
    {
        public static readonly DependencyProperty ShortcutKeysProperty = DependencyProperty.Register
            (
            "ShortcutKeys",
            typeof(ShortcutKeys),
            typeof(ShortcutKeySetBehaviour),
            new FrameworkPropertyMetadata(new ShortcutKeys(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public ShortcutKeySetBehaviour()
        {
            tmpKeys = new ShortcutKeys();
        }
        public ShortcutKeys ShortcutKeys
        {
            get => (ShortcutKeys)GetValue(ShortcutKeysProperty);
            set => SetValue(ShortcutKeysProperty, value);
        }

        private ShortcutKeys tmpKeys;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.KeyUp += OnKeyUp;
            AssociatedObject.Text = ShortcutKeys.ToString();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.KeyUp -= OnKeyUp;
        }

        private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                ShortcutKeys = tmpKeys;
                ThrowFocus();
                return;
            }
            if(e.Key == Key.Tab)
            {
                ShortcutKeys = tmpKeys;
                return;
            }

            switch (e.Key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    ShortcutKeys.Modifires |= KeyModifiers.Control; break;
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.System:
                    ShortcutKeys.Modifires |= KeyModifiers.Alt; break;
                case Key.LeftShift:
                case Key.RightShift:
                    ShortcutKeys.Modifires |= KeyModifiers.Shift; break;
                case Key.LWin:
                case Key.RWin:
                    ShortcutKeys.Modifires |= KeyModifiers.Win; break;
                default:
                    ShortcutKeys.Keys = e.Key;
                    break;
            }
            AssociatedObject.Text = ShortcutKeys.ToString();
        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            tmpKeys = ShortcutKeys;
            ThrowFocus();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            AssociatedObject.Text = LanguageService.GetString("SettingsWindow/WidgetSettings/ShortcutKeysInputRequest");
            tmpKeys = ShortcutKeys;
            ShortcutKeys = new ShortcutKeys();
        }
        private void LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            AssociatedObject.Text = ShortcutKeys.ToString();
        }

        private void ThrowFocus()
        {
            DependencyObject ancestor = AssociatedObject.Parent;
            while (ancestor != null)
            {
                // フォーカスできるか
                if (ancestor is UIElement element && element.Focusable)
                {
                    element.Focus(); // フォーカスを当てる
                    break;
                }
                ancestor = VisualTreeHelper.GetParent(ancestor);
            }

        }
    }
}
