using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Services;
using StartupWidget.ViewModels.Bases;
using StartupWidget.ViewModels.Items;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace StartupWidget.ViewModels
{
    public partial class MainWindowViewModel : ProcessListViewWindowBase, INotifyPropertyChanged
    {
        public MainWindowViewModel(SettingsService settings, ProcessViewModel processViewModel) : base(settings)
        {
            ProcessViewModel = processViewModel;

            ShieldIcon = ExecutableIconImage.GetShieldIcon();

            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                IsWidgetShow = false;
            });
            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this, (r, m) =>
            {
                IsWidgetShow = true;
            });
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { LoadSettings(); });
            ProcessItems.CollectionChanged += IsAppEmptyChanged;
            IsAppEmpty = ProcessItems.Count == 0;
            LoadSettings();
        }
        public ObservableCollection<ProcessItemViewModel> ProcessItems => ProcessViewModel.ProcessItems;
        [ObservableProperty]
        private partial ProcessViewModel ProcessViewModel { get; set; }
        [ObservableProperty]
        private ProcessItemViewModel? _selectedItem;
        [ObservableProperty]
        private bool _isWidgetShow = false;

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
        public ImageSource? ShieldIcon { get; set; }
        [ObservableProperty]
        private bool _isAppEmpty;
        private void IsAppEmptyChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IsAppEmpty = ProcessItems.Count == 0;
        }
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
        private void ShowSettings()
        {

        }



        [RelayCommand]
        private void ShowWidget()
        {
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
            if (SelectedItem != null) WeakReferenceMessenger.Default.Send(new OpenEditWindow(SelectedItem));
            else WeakReferenceMessenger.Default.Send(new OpenEditWindow());
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
        private void OpenSettings()
        {
            WeakReferenceMessenger.Default.Send(new OpenSettingsWindow());
        }

        [RelayCommand]
        private void QuitApp()
        {
            WeakReferenceMessenger.Default.Send(new Messages.QuitAppMessage());
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