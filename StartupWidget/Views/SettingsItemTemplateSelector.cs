using StartupWidget.Models.Types;
using StartupWidget.ViewModels.Items;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace StartupWidget.Views
{
    public class SettingsItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BoolTemplate {  get; set; }
        public DataTemplate? StringTemplate { get; set; }
        public DataTemplate? IntTemplate { get; set; }
        public DataTemplate? FloatTemplate { get; set; }
        public DataTemplate? LimitedIntTemplate { get; set; }
        public DataTemplate? LimitedFloatTemplate { get; set; }
        public DataTemplate? OptionalStringTemplate { get; set; }
        public DataTemplate? FontTemplate { get; set; }
        public DataTemplate? FilePathTemplate { get; set;  }
        public DataTemplate? ColorTemplate { get; set; }
        public DataTemplate? ShortcutKeyTemplate { get; set;  }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is SettingItemViewModel settingItem)
            {
                if (item is FilePathSettingItemViewModel) return FilePathTemplate;
                if (item is LimitedSettingItemViewModel limitedSettingItem)
                {
                    if(limitedSettingItem.DataType == typeof(int)) return LimitedIntTemplate;
                    if(limitedSettingItem.DataType == typeof(float)) return LimitedFloatTemplate;
                    if (limitedSettingItem.DataType == typeof(string)) return OptionalStringTemplate;
                }
                if (settingItem.DataType == typeof(bool)) return BoolTemplate;
                if (settingItem.DataType == typeof(string)) return StringTemplate;
                if (settingItem.DataType == typeof(int)) return IntTemplate;
                if (settingItem.DataType == typeof(float)) return FloatTemplate;
                if (settingItem.DataType == typeof(SerializableFont)) return FontTemplate;
                if (settingItem.DataType == typeof(Color)) return ColorTemplate;
                if (settingItem.DataType == typeof(ShortcutKeys)) return ShortcutKeyTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
