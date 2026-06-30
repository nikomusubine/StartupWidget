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
        public SettingsService SettingsService;

        public MainWindowViewModel MainWindowViewModel;
        
        public WidgetWindowViewModel WidgetWindowViewModel;
        public ProcessViewModel ProcessViewModel;

        public WidgetWindow WidgetWindow;
        public LanguageService LanguageService;
        private SingleInstanceService SingleInstanceService;

        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstanceService = new SingleInstanceService();
            if (!SingleInstanceService.IsNewInstance)
            {
                Current.Shutdown();
                return;
            }


            base.OnStartup(e);
            
            
            SettingsService = new SettingsService();
            SettingsService.LoadSettings();
            LanguageService= new LanguageService();
            LanguageService.ChangeLanguage(SettingsService.GeneralSettings.Language);

            ProcessViewModel = new ProcessViewModel();
            MainWindowViewModel = new MainWindowViewModel(SettingsService,ProcessViewModel,LanguageService);
            
            WidgetWindowViewModel = new WidgetWindowViewModel(SettingsService, ProcessViewModel,LanguageService);


            WidgetWindow = new WidgetWindow { DataContext=WidgetWindowViewModel };

            var mainWindow = new MainWindow { DataContext = MainWindowViewModel };

            RegisterMessages();

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (SingleInstanceService.IsNewInstance)
            {
                SettingsService.SaveSettings();
                ProcessViewModel.SaveApplications();
            }
            SingleInstanceService.Dispose();
            base.OnExit(e);
        }

        AddWindow? addWindow;
        EditWindow? editWindow;
        SettingsWindow? settingsWindow;

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<QuitAppMessage>(this, (r, m) =>
            {
                Current.Shutdown();
            });
            WeakReferenceMessenger.Default.Register<OpenAddWindow>(this, (r, m) =>
            {
                addWindow = new AddWindow { DataContext = new AddWindowViewModel(SettingsService,ProcessViewModel) };
                addWindow.ShowDialog();
            });

            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this, (r, m) =>
            {
                WidgetWindow.Show();
            });

            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                WidgetWindow.Hide();
            });

            WeakReferenceMessenger.Default.Register<OpenEditWindow>(this, (r, m) =>
            {

                if (m.EditingItem != null)
                    editWindow = new EditWindow { DataContext = new EditWindowViewModel(SettingsService, ProcessViewModel, m.EditingItem) };
                else if (!string.IsNullOrEmpty(m.AddingItemPath))
                    editWindow = new EditWindow { DataContext = new EditWindowViewModel(SettingsService,ProcessViewModel, m.AddingItemPath) };
                else
                   editWindow = new EditWindow { DataContext = new EditWindowViewModel(SettingsService, ProcessViewModel) };


                editWindow.ShowDialog();
            });

            WeakReferenceMessenger.Default.Register<OpenSettingsWindow>(this, (r, m) =>
            {
                settingsWindow = new SettingsWindow { DataContext = new SettingsWindowViewModel(SettingsService,LanguageService) };
                settingsWindow.ShowDialog();

                StartupService.SetStartup(SettingsService.GeneralSettings.IsStartup);
            });
        }
    }

}
