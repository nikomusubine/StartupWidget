using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public partial class WidgetWindowViewModel: ObservableObject
    {
        public WidgetWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, LanguageService languageService)
        {
            SettingsService = settings;
            LanguageService = languageService;
            ProcessViewModel = processViewModel;
            LoadSettings();
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) =>
            {
                LoadSettings();
            });
        }
        private LanguageService LanguageService { get; set; }
        [ObservableProperty]
        private ProcessItemViewModel? _selectedItem;
        public ObservableCollection<ProcessItemViewModel> ProcessItems => ProcessViewModel.ProcessItems;
        private SettingsService SettingsService { get; set; }
        private ProcessViewModel ProcessViewModel { get; set; }

        public ShortcutKeys ShortcutKeys
        {
            get => SettingsService.WidgetSettings.ShortcutKeys;
        }
        [ObservableProperty]
        private string _backgroundImagePath;

        [ObservableProperty]
        private SolidColorBrush _foregroundColor;

        [ObservableProperty]
        private SolidColorBrush _backgroundColor;
        [ObservableProperty]
        private float _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily;
        [ObservableProperty]
        private float _windowSize = 300;


        private void LoadSettings()
        {
            BackgroundImagePath = SettingsService.WidgetSettings.BackgroundImageFilePath;
            ForegroundColor = new SolidColorBrush(Color.FromArgb(SettingsService.WidgetSettings.ForegroundColor.A,
                                                                 SettingsService.WidgetSettings.ForegroundColor.R,
                                                                 SettingsService.WidgetSettings.ForegroundColor.G,
                                                                 SettingsService.WidgetSettings.ForegroundColor.B));

            byte Alpha = (byte)(0xFF - (SettingsService.WidgetSettings.Transparency / 100) * 0xFF);


            BackgroundColor = new SolidColorBrush(Color.FromArgb(Alpha,
                                                                 SettingsService.WidgetSettings.BackgroundColor.R,
                                                                 SettingsService.WidgetSettings.BackgroundColor.G,
                                                                 SettingsService.WidgetSettings.BackgroundColor.B));
            WindowSize= SettingsService.WidgetSettings.WindowSize;
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
        }


        [RelayCommand]
        private void StartProcess()
        {
            if (SelectedItem != null)
            {
                try
                {
                    ProcessViewModel.StartProcess(SelectedItem.ExecutableProcess,
                        Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
                }
                catch (Win32Exception ex)
                {
                    if(ex.NativeErrorCode == 1223)
                    {
                        // ユーザーがキャンセルした場合は何もしない Canceled by user. Do nothing
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }


        [RelayCommand]
        private void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
            WeakReferenceMessenger.Default.Send(new Messages.HideWidgetWindow());
        }
    }
}
