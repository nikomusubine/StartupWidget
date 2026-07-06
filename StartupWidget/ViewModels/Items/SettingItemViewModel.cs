using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace StartupWidget.ViewModels.Items
{
    public class SettingItemViewModel
    {
        public SettingItemViewModel(string id, string name, Type datatype, object value)
        {
            Name = name;
            DataType = datatype;
            Value = value;
            Id = id;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public Type DataType { get; set; }
        public virtual object Value { get; set; }
    }
}
