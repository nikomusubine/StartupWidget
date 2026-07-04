using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services.Win32Helpers;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StartupWidget.Services
{
    public class TaskTrayService: IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        private IntPtr hWnd;
        private IntPtr hMenu;
        private List<TaskTrayItem> taskTrayItems;
        private IntPtr hInstance;
        private WndProcDelegate wndProcDelegate;
        private IntPtr wndProcPtr
        {
            get
            {
                return Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
            }
        }
        private string className => $"{Constants.AppName}.NotiryIcon";
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
            taskTrayItems = new List<TaskTrayItem>();
            wndProcDelegate = new WndProcDelegate(WndProc);
            Module mod = Assembly.GetExecutingAssembly().GetModules()[0];
            hInstance = Marshal.GetHINSTANCE(mod);
            iconResourceid = iconresourceid;
            iconService = new GetHIconService(hInstance, iconresourceid);
            

            InitializeMessageWindow();
            InitializeTaskTrayIcon();
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
            else {

                hWnd = CreateWindowHelper.CreateMessageOnlyWindow(className,hInstance);
                if (hWnd == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastPInvokeError();

                    MessageBox.Show($"Code: {errorCode}");
                }

            } 
        }

        private void InitializeTaskTrayIcon()
        {
            TaskTrayIconHelper.TryAddIcon(hWnd, iconid, iconCallbackMessage, iconService.GetHIcon(), "StartupWidget");
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
                        TaskTrayIconHelper.GetCursorPos(out cursorPos);

                        TaskTrayIconHelper.ShowPopupMenu(hwnd, hMenu, cursorPos.x, cursorPos.y);
                    }
                    return IntPtr.Zero;
                }
            }
            else if (msg == WM_COMMAND)
            {
                int id = msg & 0xFFFF;
                taskTrayItems[id].MenuClick();
                return IntPtr.Zero;
            }
            return CreateWindowHelper.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void CheckableItemValueChanged(object sender, EventArgs e)
        {
            if (sender is TaskTrayCheckableItem checkable)
            {
                int index = taskTrayItems.IndexOf(checkable);
                TaskTrayIconHelper.ChangeMenuCheckState(hMenu, (uint)index, checkable.IsChecked);
            }
        }


        public void AppendMenu(TaskTrayMenuItemBase trayItem)
        {
            if (trayItem is TaskTrayCheckableItem checkable)
            {
                checkable.CheckableValueChanged += CheckableItemValueChanged;
            }
            if (trayItem is TaskTrayItem headerMenu)
            {
                taskTrayItems.Add(headerMenu);
                TaskTrayIconHelper.AddMenuItem(hMenu, (uint)taskTrayItems.Count - 1, headerMenu.Header);
            }
            if(trayItem is TaskTraySeparator separator)
            {
                TaskTrayIconHelper.AddMenuSeparator(hMenu);
            }
        }


        public void Dispose()
        {
            TaskTrayIconHelper.TryRemoveIcon(hWnd, iconid);
            CreateWindowHelper.DestroyWindow(hWnd);
        }
    }
}
