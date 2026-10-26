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
            _value = value;
            Id = id;
            isChanged = false;
            initialized = true;
        }
        private bool initialized = false;
        public string Id { get; set; }
        public string Name { get; set; }
        public Type DataType { get; set; }
        private object _value;
        public virtual object Value { get { return _value; } set{
                _value = value;
                if (initialized)
                {
                    isChanged = true;
                }
            } }

        public bool isChanged = false;
    }
}
