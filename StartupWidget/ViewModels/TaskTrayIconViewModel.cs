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

        private ContextMenuItem ShowMainWindow;
        private ContextMenuCheckableItem ShowWidget;
        private ContextMenuItem OpenSettings;
        private ContextMenuItem OpenEdit;
        private ContextMenuItem Quit;

        private const string LangPathBase = "TaskTrayIcon/";

        public TaskTrayIconViewModel(SettingsService settingsService)
        {
            SettingsService = settingsService;
            TaskTrayService = new TaskTrayService(32512);
            ShowMainWindow = new ContextMenuItem(LanguageService.GetString(LangPathBase+"ShowMainWindow").Replace('_','&'));
            ShowWidget = new ContextMenuCheckableItem(LanguageService.GetString(LangPathBase + "ShowWidget").Replace('_', '&'));
            OpenSettings = new ContextMenuItem(LanguageService.GetString(LangPathBase + "OpenSettings").Replace('_', '&'));
            OpenEdit = new ContextMenuItem(LanguageService.GetString(LangPathBase + "OpenEdit").Replace('_', '&'));
            Quit = new ContextMenuItem(LanguageService.GetString(LangPathBase + "Quit").Replace('_', '&'));

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
            TaskTrayService.AppendMenu(new ContextMenuSeparator());
            TaskTrayService.AppendMenu(OpenSettings);
            TaskTrayService.AppendMenu(OpenEdit);
            TaskTrayService.AppendMenu(new ContextMenuSeparator());
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
            if (sender is ContextMenuCheckableItem item)
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
