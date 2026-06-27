using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StartupWidget.ViewModels.Items
{
    public partial class FilePathSettingItemViewModel: LimitedSettingItemViewModel, INotifyPropertyChanged
    {
        public FilePathSettingItemViewModel(string id, string name, Type dataType, object value, IEnumerable<object>? Limits) : base(id, name, dataType, value, Limits)
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [RelayCommand]
        private void BrowseFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select File";
            StringBuilder sb = new StringBuilder();
            if (Options != null)
            {
                foreach (var l in Options)
                {
                    sb.Append(l);
                    sb.Append('|');
                }
                sb.Length -= 1;
            }
            dialog.Filter = sb.ToString();
            dialog.FileName = (string)Value;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Value= dialog.FileName;
                OnPropertyChanged();
            }
            dialog.Dispose();
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
