using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Settings;
using StartupWidget.Services;
using StartupWidget.Services.Interfaces;
using StartupWidget.ViewModels.Items;
using StartupWidget.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, INotifyPropertyChanged
    {
        public MainWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, LanguageService languageService)
        {
            SettingsService = settings;
            LanguageService = languageService;
            ProcessViewModel = processViewModel;



            IntPtr hBitmap = ExecutableIconImage.GetShieldIcon().ToBitmap().GetHbitmap();


            ShieldIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());


            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                IsWidgetShow = false;
            });
            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this, (r, m) =>
            {
                IsWidgetShow = true;
            });
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { LoadSettings(); });
            LoadSettings();
        }
        private LanguageService LanguageService;
        public SettingsService SettingsService { get; set; }
        public ObservableCollection<ProcessItemViewModel> ProcessItems => ProcessViewModel.ProcessItems;
        [ObservableProperty]
        private partial ProcessViewModel ProcessViewModel { get; set; }
        [ObservableProperty]
        private ProcessItemViewModel? _selectedItem;
        [ObservableProperty]
        private bool _isWidgetShow = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValidBackgroundImagePath))]
        private string _backgroundImagePath;

        public Uri? ValidBackgroundImagePath => string.IsNullOrWhiteSpace(BackgroundImagePath) ? null : new Uri(BackgroundImagePath);

        [ObservableProperty]
        private SolidColorBrush _foregroundColor;

        [ObservableProperty]
        private SolidColorBrush _backgroundColor;
        [ObservableProperty]
        private float _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily;
        public ImageSource ShieldIcon { get; set; }
        private void LoadSettings()
        {
            BackgroundImagePath = SettingsService.MainWindowSettings.BackgroundImageFilePath;
            ForegroundColor = new SolidColorBrush(Color.FromArgb(SettingsService.MainWindowSettings.ForegroundColor.A,
                                                                 SettingsService.MainWindowSettings.ForegroundColor.R,
                                                                 SettingsService.MainWindowSettings.ForegroundColor.G,
                                                                 SettingsService.MainWindowSettings.ForegroundColor.B));
            BackgroundColor = new SolidColorBrush(Color.FromArgb(SettingsService.MainWindowSettings.BackgroundColor.A,
                                                                 SettingsService.MainWindowSettings.BackgroundColor.R,
                                                                 SettingsService.MainWindowSettings.BackgroundColor.G,
                                                                 SettingsService.MainWindowSettings.BackgroundColor.B));
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
        }

        private void _StartProcess(bool isAdmin)
        {
            if (_selectedItem != null)
            {
                try
                {
                    ProcessViewModel.StartProcess(_selectedItem.ExecutableProcess, isAdmin);
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == 1223)
                    {
                        // User canceled the operation, do nothing

                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        [RelayCommand]
        private void StartProcessAsAdmin()
        {
            _StartProcess(true);
        }

        [RelayCommand]
        private void StartProcess()
        {
            _StartProcess(false);
        }

        [RelayCommand]
        private void ShowSettings()
        {

        }

        [RelayCommand]
        private void ShowWidget()
        {
            SettingsService.WidgetSettings.IsWidgetShow = IsWidgetShow;
            if (IsWidgetShow)
            {
                WeakReferenceMessenger.Default.Send(new ShowWidgetWindow());
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new HideWidgetWindow());
            }
        }

        [RelayCommand]
        private void OpenEditWindow()
        {
            if (SelectedItem != null)  WeakReferenceMessenger.Default.Send(new OpenEditWindow(OpenEditWindowOption.WithItem,SelectedItem));
            else WeakReferenceMessenger.Default.Send(new OpenEditWindow());
        }

        [RelayCommand]
        private void OpenSettings()
        {
            WeakReferenceMessenger.Default.Send(new OpenSettingsWindow());
        }
        
        [RelayCommand]
        private void QuitApp()
        {
            WeakReferenceMessenger.Default.Send(new Messages.QuitAppMessage());
            SettingsService.SaveSettings();
        }

        [RelayCommand]
        private void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
            WeakReferenceMessenger.Default.Send(new Messages.HideMainWindow());
        }

        [RelayCommand]
        private void Loaded()
        {
            IsWidgetShow = SettingsService.WidgetSettings.IsWidgetShow;
            ShowWidget();
        }

        public enum WindowType
        {
            Widget, Settings,
        }
    }
}