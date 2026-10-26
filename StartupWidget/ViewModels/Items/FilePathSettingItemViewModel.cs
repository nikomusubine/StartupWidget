using CommunityToolkit.Mvvm.Input;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StartupWidget.ViewModels.Items
{
    public partial class FilePathSettingItemViewModel: LimitedSettingItemViewModel, INotifyPropertyChanged
    {
public required OpenFileDialogService DialogService { get; init; }
        public FilePathSettingItemViewModel(string id, string name, Type dataType, object value, IEnumerable<object>? Limits) : base(id, name, dataType, value, Limits)
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [RelayCommand]
        private async void BrowseFile()
        {
            DialogService.Title = "Select File";
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
            DialogService.Filter = sb.ToString();
            DialogService.FileName = (string)Value;
            DialogService.RestoreDirectory = true;
            bool? result = await DialogService.ShowDialogAsync();
            if ((result != null) ? (bool)result : false)
            {
                Value= DialogService.FileName;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
