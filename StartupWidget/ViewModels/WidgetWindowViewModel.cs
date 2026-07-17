using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.ViewModels.Bases;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public partial class WidgetWindowViewModel : ProcessListViewWindowBase
    {
        public WidgetWindowViewModel(SettingsService settings, ProcessViewModel processViewModel):base(settings)
        {
            ProcessViewModel = processViewModel;
            ShieldIcon = ExecutableIconImage.GetShieldIcon();

            LoadSettings();
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) =>
            {
                LoadSettings();
            });
            IsAppEmpty = ProcessItems.Count == 0;
            ProcessItems.CollectionChanged+=IsAppEmptyChanged;
        }

        public IntPtr HWnd
        {
            set
            {
                ContextMenu = new ContextMenuService(value);
            }
        }

        public ContextMenuService ContextMenu = null!;
        public ImageSource? ShieldIcon { get; set; }
        [ObservableProperty]
        private ProcessItemViewModel? _selectedItem;
        public ObservableCollection<ProcessItemViewModel> ProcessItems => ProcessViewModel.ProcessItems;
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
        private double _magnetThreshold = 12;

        public ShortcutKeys ShortcutKeys
        {
            get => SettingsService.WidgetSettings.ShortcutKeys;
        }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValidBackgroundImagePath))]
        private string _backgroundImagePath = null!;

        public Uri? ValidBackgroundImagePath => string.IsNullOrWhiteSpace(BackgroundImagePath) ? null : new Uri(BackgroundImagePath);

        [ObservableProperty]
        private SolidColorBrush _foregroundColor = null!;

        [ObservableProperty]
        private SolidColorBrush _backgroundColor = null!;
        [ObservableProperty]
        private double _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily = null!;
        [ObservableProperty]
        private double _windowSize = 300;
        [ObservableProperty]
        private double _opacity;
        [ObservableProperty]
        private bool _isAppEmpty;
        private void IsAppEmptyChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IsAppEmpty = ProcessItems.Count == 0;
        }

        public double PositionX
        {
            get => SettingsService.WidgetSettings.WidgetPosition.x;
            set
            {
                Position p = SettingsService.WidgetSettings.WidgetPosition;
                p.x = value;
                SettingsService.WidgetSettings.WidgetPosition = p;
            }
        }
        public double PositionY
        {
            get => SettingsService.WidgetSettings.WidgetPosition.y;
            set
            {
                Position p = SettingsService.WidgetSettings.WidgetPosition;
                p.y = value;
                SettingsService.WidgetSettings.WidgetPosition = p;
            }
        }
        private List<ContextMenuItemBase> ContextMenuBase = null!;

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
            MagnetThreshold = SettingsService.WidgetSettings.MagnetThreshold;
            PrepareContextMenu();
            OnPropertyChanged(string.Empty);
        }



        private void _StartProcess(bool isAdmin)
        {
            if (SelectedItem != null)
            {
                try
                {
                    ProcessViewModel.StartProcess(SelectedItem.ExecutableProcess, isAdmin);
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
        private void CloseWidget()
        {
            WeakReferenceMessenger.Default.Send(new HideWidgetWindow());
        }

        [RelayCommand]
        private void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
            WeakReferenceMessenger.Default.Send(new Messages.HideWidgetWindow());
        }
        [RelayCommand]
        private void RightClick()
        {
            if (SelectedItem == null)
            {
                ContextMenu.ShowContextMenu(ContextMenuBase.GetRange(4, ContextMenuBase.Count - 4));
            }
            else
            {
                ContextMenu.ShowContextMenu(ContextMenuBase);
            }
        }
    }
}