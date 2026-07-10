using StartupWidget.Models.Types;
using StartupWidget.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace StartupWidget.ViewModels
{
    public partial class WidgetWindowViewModel
    {
        private const string LangRCMenuPath = "Widget/RightClickMenu/";
        ContextMenuCheckableItem MenuItemIsFixed = null!;
        ContextMenuCheckableItem MenuItemIsMagnet = null!;
        private void PrepareContextMenu()
        {
            ContextMenuBase = new List<ContextMenuItemBase>();
            ContextMenuItem Run = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "Run").Replace('_', '&'));
            Run.MenuClicked += ContextMenuRun;
            ContextMenuBase.Add(Run);

            ContextMenuItem RunAsAdmin = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "RunAsAdmin").Replace('_', '&'));
            RunAsAdmin.MenuClicked += ContextMenuRunAsAdmin;
            RunAsAdmin.Icon = ShieldIcon;
            ContextMenuBase.Add(RunAsAdmin);

            ContextMenuItem Edit = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "Edit").Replace('_', '&'));
            Edit.MenuClicked += ContextMenuEdit;
            ContextMenuBase.Add(Edit);

            ContextMenuBase.Add(new ContextMenuSeparator());

            MenuItemIsFixed = new ContextMenuCheckableItem(LanguageService.GetString(LangRCMenuPath + "IsFixed").Replace('_', '&'));
            MenuItemIsFixed.IsChecked = IsFixed;
            MenuItemIsFixed.MenuClicked += ContextMenuIsFixed;
            ContextMenuBase.Add(MenuItemIsFixed);

            MenuItemIsMagnet = new ContextMenuCheckableItem(LanguageService.GetString(LangRCMenuPath + "IsMagnet").Replace('_', '&'));
            MenuItemIsMagnet.IsChecked = IsMagnet;
            MenuItemIsMagnet.MenuClicked += ContextMenuIsMagnet;
            ContextMenuBase.Add(MenuItemIsMagnet);

            ContextMenuBase.Add(new ContextMenuSeparator());

            ContextMenuItem ShowMainWindow = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "ShowMainWindow").Replace('_', '&'));
            ShowMainWindow.MenuClicked += ContextMenuRunAsAdmin;
            ContextMenuBase.Add(ShowMainWindow);

            ContextMenuItem ShowSettings = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "ShowSettings").Replace('_', '&'));
            ShowSettings.MenuClicked += ContextMenuEdit;
            ContextMenuBase.Add(ShowSettings);

            ContextMenuBase.Add(new ContextMenuSeparator());

            ContextMenuItem CloseWidget = new ContextMenuItem(LanguageService.GetString(LangRCMenuPath + "CloseWidget").Replace('_', '&'));
            CloseWidget.MenuClicked += ContextMenuCloseWidget;
            ContextMenuBase.Add(CloseWidget);
        }

        private void ContextMenuRun(object sender, EventArgs e)
        {
            StartProcess();
        }
        private void ContextMenuRunAsAdmin(object sender, EventArgs e)
        {
            StartProcessAsAdmin();
        }
        private void ContextMenuEdit(object sender, EventArgs e)
        {
            OpenEditWindow();
        }
        private void ContextMenuIsFixed(object sender, EventArgs e)
        {
            IsFixed = !IsFixed;
            MenuItemIsFixed.IsChecked = IsFixed;
        }
        private void ContextMenuIsMagnet(object sender, EventArgs e)
        {
            IsMagnet = !IsMagnet;
            MenuItemIsMagnet.IsChecked = IsMagnet;
        }
        private void ContextMenuShowMainWindow(object sender, EventArgs e)
        {
            ShowMainWindow();
        }
        private void ContextMenuShowSettings(object sender, EventArgs e)
        {
            ShowSettings();
        }
        private void ContextMenuCloseWidget(object sender, EventArgs e)
        {
            CloseWidget();
        }
    }
}
