using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Services;
using StartupWidget.ViewModels;
using StartupWidget.Views;
using System.IO;
using System.Windows;

namespace StartupWidget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public SettingsService? SettingsService;

        public MainWindowViewModel? MainWindowViewModel;
        
        public WidgetWindowViewModel? WidgetWindowViewModel;
        public ProcessViewModel? ProcessViewModel;

        public WidgetWindow? WidgetWindow;
        public MainWindow AppMainWindow = null!;
        private SingleInstanceService? SingleInstanceService;
        public NamedPipeIPCServer? NamedPipeServer;
        public TaskTrayIconViewModel? TaskTrayViewModel;
        const string defaultFallbackLanguage = "ja-JP";

        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstanceService = new SingleInstanceService();
#if !DEBUG
            if (!SingleInstanceService.IsNewInstance)
            {
                try
                {
                    using NamedPipeIPCClient ipcclient = new NamedPipeIPCClient();
                    ipcclient.SendMessage(e.Args);
                }
                catch (Exception)
                {

                }
                Current.Shutdown();
                return;
            }
#endif

            base.OnStartup(e);
            
            
            SettingsService = new SettingsService();
            SettingsService.LoadSettings();
            try
            {
                LanguageService.ChangeLanguage(SettingsService.GeneralSettings.Language);
            }
            catch (FileNotFoundException)
            {
                SettingsService.GeneralSettings.Language = defaultFallbackLanguage;
                LanguageService.ChangeLanguage(SettingsService.GeneralSettings.Language);
            }
            NamedPipeServer = new NamedPipeIPCServer();
            NamedPipeServer.OnMessageReceived += OnNamedPipeMessageReceived;
            NamedPipeServer.StartListen();

            ProcessViewModel = new ProcessViewModel();
            MainWindowViewModel = new MainWindowViewModel(SettingsService,ProcessViewModel);
            
            WidgetWindowViewModel = new WidgetWindowViewModel(SettingsService, ProcessViewModel);
            TaskTrayViewModel = new TaskTrayIconViewModel(SettingsService);

            WidgetWindow = new WidgetWindow { DataContext=WidgetWindowViewModel };
            WidgetWindowViewModel.HWnd= new System.Windows.Interop.WindowInteropHelper(WidgetWindow).Handle;

            AppMainWindow = new MainWindow { DataContext = MainWindowViewModel };

            RegisterMessages();

            AppMainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (SingleInstanceService != null)
            {
                SingleInstanceService.Dispose();
            }
            base.OnExit(e);
        }

        private void OnNamedPipeMessageReceived(string[]? message)
        {
            Current.Dispatcher.BeginInvoke(() =>
            {
                if (message == null || message.Length < 1)
                {
                    WeakReferenceMessenger.Default.Send(new ShowMainWindow());
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new OpenEditWindow(AddingItemPath: message[0]));
                }
            });
        }


        EditWindow? editWindow;
        SettingsWindow? settingsWindow;

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<QuitAppMessage>(this, (r, m) =>
            {
                if (SingleInstanceService!.IsNewInstance)
                {
                    SettingsService!.SaveSettings();
                    ProcessViewModel!.SaveApplications();
                    NamedPipeServer!.Dispose();
                    TaskTrayViewModel!.Dispose();
                }
                Current.Shutdown();
            });

            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this, (r, m) =>
            {
                SettingsService.WidgetSettings.IsWidgetShow = true;
                WidgetWindow.Show();
            });

            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                SettingsService.WidgetSettings.IsWidgetShow = false;
                WidgetWindow.Hide();
            });

            WeakReferenceMessenger.Default.Register<OpenEditWindow>(this, (r, m) =>
            {

                if (m.EditingItem != null)
                {
                    editWindow = new EditWindow();
                    editWindow.DataContext = new EditWindowViewModel(SettingsService, ProcessViewModel, m.EditingItem)
                    {
                        FolderDialogService = new OpenFolderDialogService(editWindow),
                        FileDialogService = new OpenFileDialogService(editWindow),
                        MessageBoxService = new MessageBoxService(editWindow)
                    };
                }
                else if (!string.IsNullOrEmpty(m.AddingItemPath))
                {
                    editWindow = new EditWindow();
                    editWindow.DataContext = new EditWindowViewModel(SettingsService, ProcessViewModel, m.AddingItemPath)
                    {
                        FolderDialogService = new OpenFolderDialogService(editWindow),
                        FileDialogService = new OpenFileDialogService(editWindow),
                        MessageBoxService = new MessageBoxService(editWindow)
                    };
                }
                else
                {
                    editWindow = new EditWindow();
                    editWindow.DataContext = new EditWindowViewModel(SettingsService, ProcessViewModel)
                    {
                        FolderDialogService = new OpenFolderDialogService(editWindow),
                        FileDialogService = new OpenFileDialogService(editWindow),
                        MessageBoxService = new MessageBoxService(editWindow)
                    };
                }
                editWindow.Owner = AppMainWindow;
                editWindow.ShowDialog();
            });

            WeakReferenceMessenger.Default.Register<OpenSettingsWindow>(this, (r, m) =>
            {
                settingsWindow = new SettingsWindow ();
                settingsWindow.DataContext = new SettingsWindowViewModel(SettingsService, new OpenFileDialogService(settingsWindow),new ColorPickerDialogService(settingsWindow));
                settingsWindow.Owner = AppMainWindow;
                settingsWindow.ShowDialog();

                if (!StartupService.SetStartup(SettingsService.GeneralSettings.IsStartup))
                {
                    SettingsService.GeneralSettings.IsStartup = false;
                    MessageBoxService messageBoxService = new MessageBoxService(AppMainWindow);
                    messageBoxService.Show(LanguageService.GetString("SettingsWindow/MessageBox/FailedStartUp"),
                        LanguageService.GetString("Reusable/ErrorMessageBoxTitle"), MessageBoxDialogButton.OK, MessageBoxDialogIcon.Stop);
                }
            });
        }
    }

}
