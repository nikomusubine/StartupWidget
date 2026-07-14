using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services.Win32Helpers;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StartupWidget.Services
{
    public class TaskTrayService : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        private IntPtr hWnd;
        private IntPtr hMenu;
        private List<ContextMenuItem> taskTrayItems;
        private IntPtr hInstance;
        private WndProcDelegate wndProcDelegate;
        private IntPtr wndProcPtr
        {
            get
            {
                return Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
            }
        }
        private string className => $"{Constants.AppName}.NotifyIcon";
        private int iconResourceid;
        private GetHIconService iconService;
        private const int iconid = 0;
        #region Windowメッセージ
        private const int WM_COMMAND = 0x0111;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONUP = 0x0205;

        private const int WM_APP = 0x8000;
        private const int iconCallbackMessage = WM_APP + 1;
        #endregion

        public TaskTrayService(int iconresourceid)
        {
            taskTrayItems = new List<ContextMenuItem>();
            wndProcDelegate = new WndProcDelegate(WndProc);
            Module mod = Assembly.GetExecutingAssembly().GetModules()[0];
            hInstance = Marshal.GetHINSTANCE(mod);
            iconResourceid = iconresourceid;
            iconService = new GetHIconService(hInstance, iconresourceid);


            InitializeMessageWindow();
            InitializeTaskTrayIcon();
            hMenu = ContextMenuHelper.CreateMenu();
        }

        private void InitializeMessageWindow()
        {
            ushort atom = CreateWindowHelper.RegisterMessageWindowClass(className, wndProcPtr, hInstance);
            if (atom == 0)
            {
                const int ERROR_CLASS_ALREADY_EXISTS = 1410;
                int errorCode = Marshal.GetLastPInvokeError();
                if (errorCode == ERROR_CLASS_ALREADY_EXISTS)
                {
                    throw new InvalidOperationException("既に一つ以上のタスクトレイアイコンが作成されています。");
                }
                else
                {
                    throw new Win32Exception(errorCode);
                }
            }
            else
            {

                hWnd = CreateWindowHelper.CreateMessageOnlyWindow(className, hInstance);
                if (hWnd == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastPInvokeError();
                    throw new Win32Exception(errorCode);
                }

            }
        }

        private void InitializeTaskTrayIcon()
        {
            if (!TaskTrayIconHelper.TryAddIcon(hWnd, iconid, iconCallbackMessage, iconService.GetHIcon(), "StartupWidget"))
            {
                int errorCode = Marshal.GetLastPInvokeError();
                throw new Win32Exception(errorCode);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == iconCallbackMessage)
            {
                if (wParam.ToInt32() == iconid)
                {
                    int mouseMessage = lParam.ToInt32();
                    if (mouseMessage == WM_LBUTTONDBLCLK)
                    {
                        WeakReferenceMessenger.Default.Send(new ShowMainWindow());
                    }
                    if (mouseMessage == WM_RBUTTONUP)
                    {
                        Win32Helpers.Point cursorPos;
                        ContextMenuHelper.GetCursorPos(out cursorPos);

                        ContextMenuHelper.ShowPopupMenu(hwnd, hMenu, cursorPos.x, cursorPos.y);
                    }
                    return IntPtr.Zero;
                }
            }
            else if (msg == WM_COMMAND)
            {
                int id = wParam.ToInt32() & 0xFFFF;
                taskTrayItems[id].MenuClick();
                return IntPtr.Zero;
            }
            return CreateWindowHelper.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void CheckableItemValueChanged(object sender, EventArgs e)
        {
            if (sender is ContextMenuCheckableItem checkable)
            {
                int index = taskTrayItems.IndexOf(checkable);
                ContextMenuHelper.ChangeMenuCheckState(hMenu, (uint)index, checkable.IsChecked);
            }
        }


        public void AppendMenu(ContextMenuItemBase trayItem)
        {
            if (trayItem is ContextMenuItem headerMenu)
            {
                taskTrayItems.Add(headerMenu);
                ContextMenuHelper.AddMenuItem(hMenu, (uint)taskTrayItems.Count - 1, headerMenu.Header);
            }
            if (trayItem is ContextMenuCheckableItem checkable)
            {
                checkable.CheckableValueChanged += CheckableItemValueChanged;
                ContextMenuHelper.ChangeMenuCheckState(hMenu, (uint)taskTrayItems.Count - 1, checkable.IsChecked);
            }
            if (trayItem is ContextMenuSeparator separator)
            {
                ContextMenuHelper.AddMenuSeparator(hMenu);
            }
        }


        public void Dispose()
        {
            TaskTrayIconHelper.TryRemoveIcon(hWnd, iconid);

            if (hMenu != IntPtr.Zero)
            {
                ContextMenuHelper.TryDestroyMenu(hMenu);
                hMenu = IntPtr.Zero;
            }

            if (hWnd != IntPtr.Zero)
            {
                CreateWindowHelper.DestroyWindow(hWnd);
                hWnd = IntPtr.Zero;
            }
        }
    }
}
