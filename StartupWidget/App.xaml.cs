using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Services;
using StartupWidget.Services.Interfaces;
using StartupWidget.ViewModels;
using StartupWidget.ViewModels.Items;
using StartupWidget.Views;
using System.Configuration;
using System.Data;
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
        public LanguageService? LanguageService;
        private SingleInstanceService? SingleInstanceService;
        public NamedPipeIPCServer? NamedPipeServer;
        public TaskTrayIconViewModel? TaskTrayViewModel;
        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstanceService = new SingleInstanceService();
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


            base.OnStartup(e);
            
            
            SettingsService = new SettingsService();
            SettingsService.LoadSettings();
            LanguageService= new LanguageService();
            LanguageService.ChangeLanguage(SettingsService.GeneralSettings.Language);
            
            NamedPipeServer = new NamedPipeIPCServer();
            NamedPipeServer.OnMessageReceived += OnNamedPipeMessageReceived;
            NamedPipeServer.StartListen();

            ProcessViewModel = new ProcessViewModel();
            MainWindowViewModel = new MainWindowViewModel(SettingsService,ProcessViewModel,LanguageService);
            
            WidgetWindowViewModel = new WidgetWindowViewModel(SettingsService, ProcessViewModel,LanguageService);
            TaskTrayViewModel = new TaskTrayIconViewModel(SettingsService);

            WidgetWindow = new WidgetWindow { DataContext=WidgetWindowViewModel };

            AppMainWindow = new MainWindow { DataContext = MainWindowViewModel };

            RegisterMessages();

            AppMainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (SingleInstanceService!.IsNewInstance)
            {
                SettingsService!.SaveSettings();
                ProcessViewModel!.SaveApplications();
                NamedPipeServer!.Dispose();
                TaskTrayViewModel!.Dispose();
            }
            SingleInstanceService.Dispose();
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
                settingsWindow.DataContext = new SettingsWindowViewModel(SettingsService, LanguageService,new OpenFileDialogService(settingsWindow),new ColorPickerDialogService(settingsWindow));
                settingsWindow.Owner = AppMainWindow;
                settingsWindow.ShowDialog();

                StartupService.SetStartup(SettingsService.GeneralSettings.IsStartup);
            });
        }
    }

}
