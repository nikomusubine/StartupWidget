using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StartupWidget.Controls
{    
    public class FloatNumericBox: System.Windows.Controls.TextBox
    {
        private float Value
        {
            get
            {
                if (float.TryParse(Text, out float Value))
                {
                    return Value;
                }
                return 0.0f;
            }
            set
            {
                Text = value.ToString();
            }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(float), typeof(FloatNumericBox), new PropertyMetadata(float.MinValue));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(float), typeof(FloatNumericBox), new PropertyMetadata(float.MaxValue));

        public bool IsLimited { get; set; } = false;
        public float MaxValue { get=>(float)GetValue(MaxValueProperty);
            set { SetValue(MaxValueProperty, value); } }
        public float MinValue { get => (float)GetValue(MinValueProperty);
            set { SetValue(MinValueProperty, value); } }
        public FloatNumericBox()
        {
            InputMethod.SetIsInputMethodEnabled(this, false);
            System.Windows.DataObject.AddPastingHandler(this, OnPaste);
            this.LostFocus += LostFocusHandler;
            this.TextChanged += TextChangedEventHandler;
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                DependencyObject ancestor = this.Parent;
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

        /// <summary>
        /// 入力を反映前に検証する
        /// </summary>

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            var afterText = GetAfterText(e.Text);

            if (!IsFloat(afterText))
            {
                e.Handled = true;
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                var afterText = GetAfterText(text);
                if(!IsFloat(afterText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void LostFocusHandler(object sender, RoutedEventArgs e)
        {
            if (IsLimited)
            {
                Value = Math.Clamp(Value, MinValue, MaxValue);
            }
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            CheckOutofRange();
        }

        private bool IsFloat(string text)
        {
            if("-" == text)
            {
                return true;
            }

            if("0." == text)
            {
                return true;
            }
            if ("-0." == text)
            {
                return true;
            }

            string[] splited = text.Split('.');
            string AfterPeriod = "";
            if (splited.Length > 1)
            {
                AfterPeriod = splited[1];
            }

            if (float.TryParse(text, out float Value))
            {
                return string.Equals(text, Value.ToString()) || string.Equals(text, Value.ToString()+"."+AfterPeriod);
            }

            return false;
        }

        private bool CheckOutofRange()
        {
            if (IsLimited)
            {
                if (Value < MinValue || Value > MaxValue)
                {
                    this.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    this.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
            return false;
        }

        private string GetAfterText(string text)
        {
            return Text.Substring(0, SelectionStart) + text + Text.Substring(SelectionStart + SelectionLength);
        }
    }
}
