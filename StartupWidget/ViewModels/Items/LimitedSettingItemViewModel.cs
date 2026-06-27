using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.ViewModels.Items
{
    public class LimitedSettingItemViewModel: SettingItemViewModel
    {
        public object? MaxValue { get; set; }
        public object? MinValue { get; set; }
        public IEnumerable<string>? Options { get; set; } = null;

        public double SliderMaximum => Convert.ToDouble(MaxValue);

        public double SliderValue
        {
            get => Convert.ToDouble(Value);
            set
            {
                if (DataType == typeof(int))
                    Value = (object)Convert.ToInt32(value);
                else if (DataType == typeof(float))
                    Value = Convert.ToSingle(value);
                else
                    Value = value;
                
            }
        }
        public double SliderMinimum => Convert.ToDouble(MinValue);

        public LimitedSettingItemViewModel(string id, string name, Type dataType, object value, IEnumerable<object>? Limits) : base(id, name, dataType, value)
        {
            Name = name;
            Value = value;

            if(dataType == typeof(int))
            {
                MinValue = Limits is not null ? (int)Limits.ElementAtOrDefault(0) : int.MinValue;
                MaxValue = Limits is not null ? (int)Limits.ElementAtOrDefault(1) : int.MaxValue;
                if((int)MaxValue == int.MaxValue)
                {
                    MaxValue = 1000;
                }
            }
            if(dataType == typeof(float))
            {
                MinValue = Limits is not null ? (float)Limits.ElementAtOrDefault(0) : float.MinValue;
                MaxValue = Limits is not null ? (float)Limits.ElementAtOrDefault(1) : float.MaxValue;
                if ((float)MaxValue == float.MaxValue)
                {
                    MaxValue = 1000;
                }
            }
            if(dataType == typeof(string))
            {
                Options = Limits is not null ? (IEnumerable<string>)Limits : null;
            }
        }
    }
}
