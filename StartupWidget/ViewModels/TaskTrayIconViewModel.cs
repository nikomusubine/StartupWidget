using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using System.Runtime.InteropServices.ObjectiveC;

namespace StartupWidget.ViewModels
{
    public class TaskTrayIconViewModel: IDisposable
    {
        private SettingsService SettingsService;
        private TaskTrayService TaskTrayService;

        private TaskTrayItem ShowMainWindow;
        private TaskTrayCheckableItem ShowWidget;
        private TaskTrayItem OpenSettings;
        private TaskTrayItem OpenEdit;
        private TaskTrayItem Quit;

        private const string LangPathBase = "TaskTrayIcon/";

        public TaskTrayIconViewModel(SettingsService settingsService)
        {
            SettingsService = settingsService;
            TaskTrayService = new TaskTrayService(32512);
            ShowMainWindow = new TaskTrayItem(LanguageService.GetString(LangPathBase+"ShowMainWindow").Replace('_','&'));
            ShowWidget = new TaskTrayCheckableItem(LanguageService.GetString(LangPathBase + "ShowWidget").Replace('_', '&'));
            OpenSettings = new TaskTrayItem(LanguageService.GetString(LangPathBase + "OpenSettings").Replace('_', '&'));
            OpenEdit = new TaskTrayItem(LanguageService.GetString(LangPathBase + "OpenEdit").Replace('_', '&'));
            Quit = new TaskTrayItem(LanguageService.GetString(LangPathBase + "Quit").Replace('_', '&'));

            InitializeMenu();
            InitializeMessenger();
        }

        void InitializeMenu()
        {
            ShowWidget.IsChecked = SettingsService.WidgetSettings.IsWidgetShow;
            ShowMainWindow.MenuClicked += ShowMainWindowEventHandler;
            ShowWidget.MenuClicked += ShowWidgetEventHandler;
            OpenSettings.MenuClicked += OpenSettingsEventHandler;
            OpenEdit.MenuClicked += OpenEditEventHandler;
            Quit.MenuClicked += QuitEventHandler;

            TaskTrayService.AppendMenu(ShowMainWindow);
            TaskTrayService.AppendMenu(ShowWidget);
            TaskTrayService.AppendMenu(new TaskTraySeparator());
            TaskTrayService.AppendMenu(OpenSettings);
            TaskTrayService.AppendMenu(OpenEdit);
            TaskTrayService.AppendMenu(new TaskTraySeparator());
            TaskTrayService.AppendMenu(Quit);
        }
        
        void InitializeMessenger()
        {
            WeakReferenceMessenger.Default.Register<ShowWidgetWindow>(this,(r, m) =>
            {
                ShowWidget.IsChecked = true;
            });
            WeakReferenceMessenger.Default.Register<HideWidgetWindow>(this, (r, m) =>
            {
                ShowWidget.IsChecked = false;
            });
        }

        private void ShowMainWindowEventHandler(object sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new ShowMainWindow());
        }
        private void ShowWidgetEventHandler(object sender, EventArgs e)
        {
            if (sender is TaskTrayCheckableItem item)
            {
                if (item.IsChecked)
                {
                    WeakReferenceMessenger.Default.Send(new ShowWidgetWindow());
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new HideWidgetWindow());
                }
            }
        }
        private void OpenSettingsEventHandler(object sender ,EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new OpenSettingsWindow());
        }
        private void OpenEditEventHandler(object sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new OpenEditWindow());
        }
        private void QuitEventHandler(object sender,EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new QuitAppMessage());
        }

        public void Dispose()
        {
            TaskTrayService.Dispose();
        }
    }
}
