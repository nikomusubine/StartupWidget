using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StartupWidget.ViewModels.Items
{
    public partial class ColorSettingItemViewModel: SettingItemViewModel,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string ColorName
        {
            get
            {
                return ColorTranslator.ToHtml((Color)Value);
            }
            set
            {
                Value = ColorTranslator.FromHtml(value);
            }
        }
        public ColorSettingItemViewModel(string id, string name, Type datatype, object value) : base(id, name, datatype, value)
        {

        }

        [RelayCommand]
        private void ColorSelect()
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = (Color)Value;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ColorName = ColorTranslator.ToHtml(dialog.Color);
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
