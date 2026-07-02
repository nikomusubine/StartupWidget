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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public partial class WidgetWindowViewModel : ObservableObject
    {
        public WidgetWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, LanguageService languageService)
        {
            SettingsService = settings;
            LanguageService = languageService;
            ProcessViewModel = processViewModel;

            IntPtr hBitmap = ExecutableIconImage.GetShieldIcon().ToBitmap().GetHbitmap();
            ShieldIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            LoadSettings();
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) =>
            {
                LoadSettings();
            });
        }
        public ImageSource ShieldIcon { get; set; }
        private LanguageService LanguageService { get; set; }
        [ObservableProperty]
        private ProcessItemViewModel? _selectedItem;
        public ObservableCollection<ProcessItemViewModel> ProcessItems => ProcessViewModel.ProcessItems;
        private SettingsService SettingsService { get; set; }
        private ProcessViewModel ProcessViewModel { get; set; }

        private bool _isFixed = false;
        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                _isFixed = value;
                SettingsService.WidgetSettings.IsFixed = value;
                OnPropertyChanged(nameof(IsFixed));
            }
        }
        private bool _isMagnet = true;
        public bool IsMagnet
        {
            get => _isMagnet;
            set
            {
                _isMagnet = value;
                SettingsService.WidgetSettings.IsMagnet = value;
                OnPropertyChanged(nameof(IsMagnet));
            }
        }
        [ObservableProperty]
        private float _magnetThreashold = 12;

        public ShortcutKeys ShortcutKeys
        {
            get => SettingsService.WidgetSettings.ShortcutKeys;
        }
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
        [ObservableProperty]
        private float _windowSize = 300;
        [ObservableProperty]
        private float _opacity;

        public float PositionX
        {
            get => SettingsService.WidgetSettings.WidgetPosition.x;
            set
            {
                Position p = SettingsService.WidgetSettings.WidgetPosition;
                p.x = value;
                SettingsService.WidgetSettings.WidgetPosition = p;
            }
        }
        public float PositionY
        {
            get => SettingsService.WidgetSettings.WidgetPosition.y;
            set
            {
                Position p = SettingsService.WidgetSettings.WidgetPosition;
                p.y = value;
                SettingsService.WidgetSettings.WidgetPosition = p;
            }
        }


        private void LoadSettings()
        {
            Opacity = (100 - SettingsService.WidgetSettings.Transparency) / 100;
            BackgroundImagePath = SettingsService.WidgetSettings.BackgroundImageFilePath;
            ForegroundColor = new SolidColorBrush(Color.FromArgb(SettingsService.WidgetSettings.ForegroundColor.A,
                                                                 SettingsService.WidgetSettings.ForegroundColor.R,
                                                                 SettingsService.WidgetSettings.ForegroundColor.G,
                                                                 SettingsService.WidgetSettings.ForegroundColor.B));

            byte Alpha = (byte)(string.IsNullOrEmpty(BackgroundImagePath) ? (0xFF - (SettingsService.WidgetSettings.Transparency / 100) * 0xFF) : 0);

            BackgroundColor = new SolidColorBrush(Color.FromArgb(Alpha,
                                                                 SettingsService.WidgetSettings.BackgroundColor.R,
                                                                 SettingsService.WidgetSettings.BackgroundColor.G,
                                                                 SettingsService.WidgetSettings.BackgroundColor.B));
            WindowSize = SettingsService.WidgetSettings.WindowSize;
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
            IsFixed = SettingsService.WidgetSettings.IsFixed;
            IsMagnet = SettingsService.WidgetSettings.IsMagnet;
            MagnetThreashold = SettingsService.WidgetSettings.MagnetThreshold;
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
        private void FileDrop(string path)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                WeakReferenceMessenger.Default.Send(new OpenEditWindow(AddingItemPath: path));
            }));
        }
        [RelayCommand]
        private void OpenEditWindow()
        {
            if (SelectedItem != null) WeakReferenceMessenger.Default.Send(new OpenEditWindow(SelectedItem));
            else WeakReferenceMessenger.Default.Send(new OpenEditWindow());
        }
        [RelayCommand]
        private void ShowSettings()
        {
            WeakReferenceMessenger.Default.Send(new OpenSettingsWindow());
        }

        [RelayCommand]
        private void ShowMainWindow()
        {
            WeakReferenceMessenger.Default.Send(new ShowMainWindow());
        }


        [RelayCommand]
        private void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
            WeakReferenceMessenger.Default.Send(new Messages.HideWidgetWindow());
        }
    }
}