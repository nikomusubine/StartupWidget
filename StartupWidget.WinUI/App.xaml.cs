using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using StartupWidget.Messages;
using StartupWidget.Services;
using StartupWidget.ViewModels;
using StartupWidget.WinUI.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StartupWidget.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static DispatcherQueue DispatcherQueue { get; private set; } = null!;
        public SettingsService SettingsService=null!;

        public MainWindowViewModel MainWindowViewModel=null!;

        public WidgetWindowViewModel WidgetWindowViewModel=null!;
        public ProcessViewModel ProcessViewModel=null!;

        public WidgetWindow WidgetWindow = null!;
        public MainWindow AppMainWindow = null!;
        private EditWindow? editWindow = null;
        private SettingsWindow? settingsWindow = null;
        private SingleInstanceService SingleInstanceService = null!;
        public NamedPipeIPCServer NamedPipeServer = null!;
        public TaskTrayIconViewModel TaskTrayViewModel = null!;
        const string defaultFallbackLanguage = "ja-JP";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            CheckSingleInstance(args);
            base.OnLaunched(args);
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();

            LoadSettings();
            StartNamedPipe();
            InitViewModelInstances();
            RegisterMessages();
            WidgetWindow = new WidgetWindow { DataContext = WidgetWindowViewModel! };
            AppMainWindow = new MainWindow();
            AppMainWindow.DataContext = MainWindowViewModel;
            AppMainWindow.Activate();
        }
        private void CheckSingleInstance(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
        {
            SingleInstanceService = new SingleInstanceService();
            if (!SingleInstanceService.IsNewInstance)
            {
                try
                {
                    using NamedPipeIPCClient ipcclient = new NamedPipeIPCClient();
                    ipcclient.SendMessage(Parse_RegEx(e.Arguments));
                }
                catch (Exception)
                {

                }
                Current.Exit();
                return;
            }
        }

        private void LoadSettings()
        {
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
        }

        private void InitViewModelInstances()
        {
            ProcessViewModel = new ProcessViewModel();
            MainWindowViewModel = new MainWindowViewModel(SettingsService, ProcessViewModel);

            WidgetWindowViewModel = new WidgetWindowViewModel(SettingsService, ProcessViewModel);
            TaskTrayViewModel = new TaskTrayIconViewModel(SettingsService);
        }

        private void StartNamedPipe()
        {
            NamedPipeServer = new NamedPipeIPCServer();
            NamedPipeServer.OnMessageReceived += OnNamedPipeMessageReceived;
            using var _ = NamedPipeServer.StartListen();
        }
        private void OnNamedPipeMessageReceived(string[]? message)
        {
            DispatcherQueue.TryEnqueue(() =>
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

        static string[] Parse_RegEx(string text)
        {
            return Regex.Matches(text, @"("".*?""|[^ ""]+)+").OfType<Match>()
                        .Select(m => m.Groups[0].Value)
                        .ToArray();
        }
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
                Current.Exit();
            });

            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this, (r, m) =>
            {
                SettingsService.WidgetSettings.IsWidgetShow = true;
                WidgetWindow.Activate();
            });

            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                SettingsService.WidgetSettings.IsWidgetShow = false;
                WidgetWindow.AppWindow.Hide();
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
                editWindow.Own = AppMainWindow;
                editWindow.ShowDialog();
            });

            WeakReferenceMessenger.Default.Register<OpenSettingsWindow>(this, (r, m) =>
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.DataContext = new SettingsWindowViewModel(SettingsService, new OpenFileDialogService(settingsWindow), new ColorPickerDialogService(settingsWindow));
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

            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) =>
            {
                LanguageService.ChangeLanguage(SettingsService.GeneralSettings.Language);
            });
        }
    }
}
