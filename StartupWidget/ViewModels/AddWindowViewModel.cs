using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace StartupWidget.ViewModels
{
    public partial class AddWindowViewModel: ObservableObject
    {
        public AddWindowViewModel(SettingsService settings, ProcessViewModel processViewModel)
        {
            SettingsService = settings;
            ProcessViewModel = processViewModel;
            _AddingItem = new ExecutableProcess();

        }

        [ObservableProperty]
        private ExecutableProcess? _AddingItem;

        private SettingsService SettingsService { get; set; }
        private ProcessViewModel ProcessViewModel { get; set; }

        [RelayCommand]
        private void OKButtonClick()
        {
            if (_AddingItem.Name == "")
            {
                MessageBox.Show("名前を入力してください");
                return;
            }
            if(!File.Exists(_AddingItem?.FilePath))
            {
                MessageBox.Show("実行ファイルのパスが誤っています");
                return;
            }
            if(_AddingItem.IconFilePath !="" && !File.Exists(_AddingItem?.IconFilePath))
            {
                MessageBox.Show("アイコンのパスが誤っています");
                return;
            }
            ProcessViewModel.AddProcess(_AddingItem);
            WeakReferenceMessenger.Default.Send(new Messages.CloseAddWindow());
        }
        [RelayCommand]
        private void CancelButtonClick()
        {
            WeakReferenceMessenger.Default.Send(new Messages.CloseAddWindow());
        }
    }
}
