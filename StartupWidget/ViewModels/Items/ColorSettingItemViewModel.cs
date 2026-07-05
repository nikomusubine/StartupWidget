using CommunityToolkit.Mvvm.Input;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StartupWidget.ViewModels.Items
{
    public partial class ColorSettingItemViewModel: SettingItemViewModel,INotifyPropertyChanged
    {
        public required ColorPickerDialogService ColorDialog;
        public event PropertyChangedEventHandler? PropertyChanged;

        public override object Value
        {
            get => base.Value;
            set
            {
                _colorName = ((Color)value).ToHtml();
                OnPropertyChanged(nameof(ColorName));
                OnPropertyChanged();
                base.Value = value;
            }
        }

        private string _colorName;
        public string ColorName
        {
            get
            {
                return _colorName;
            }
            set
            {
                _colorName = value;
                Color color;
                if (Color.TryFromHtml(value, out color))
                {
                    Value = color;
                }
                else
                {
                    Value = Color.FromHtml(value);
                }
                OnPropertyChanged();
            }
        }
        public ColorSettingItemViewModel(string id, string name, Type datatype, object value) : base(id, name, datatype, value)
        {
            Value = value;
        }

        [RelayCommand]
        private async void ColorSelect()
        {            
            ColorDialog.Color = (Color)Value;
            bool? result = await ColorDialog.ShowDialogAsync();
            if (result != null && (bool)result)
            {
                ColorName = ColorDialog.Color.ToHtml();
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
