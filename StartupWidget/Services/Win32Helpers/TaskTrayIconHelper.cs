using System.Runtime.InteropServices;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class TaskTrayIconHelper
    {
        #region Win32API構造体と定数
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct NOTIFYICONDATA
        {
            public int cbSize; //構造体のサイズ(byte)
            public IntPtr hWnd; //コールバック先hWnd
            public uint uID; //アプリで使用するアイコン識別ID
            public uint uFlags; //どのメンバが有効か？例: NIF_ICONを指定すればhIconを見てアイコンを設定、NIF_TIPを指定でszTipを見てTooltipを指定
            public uint uCallbackMessage; //通知メッセージのID
            public IntPtr hIcon; //アイコンのハンドル
            
            public fixed char szTip[128]; //ツールチップのテキスト
            public uint dwState;
            public uint dwStateMask;
            public fixed char szInfo[256];
            public uint uTimeoutOrVersion;
            
            public fixed char szInfoTitle[64];
            public uint dwInfoFlags;
        }
        public const uint NIM_ADD =    0x00000000;
        public const uint NIM_MODIFY = 0x00000001;
        public const uint NIM_DELETE = 0x00000002;

        public const uint NIF_MESSAGE = 0x00000001;
        public const uint NIF_ICON =    0x00000002;
        public const uint NIF_TIP =     0x00000004;

        public const uint WM_USER = 0x00000400;

        public const uint MF_STRING = 0x00000000;
        public const uint MF_SEPARATOR = 0x00000800;
        public const uint MF_BYCOMMAND = 0x00000000;
        public const uint MF_CHECKED = 0x00000008;
        public const uint MF_UNCHECKED = 0x00000000;
        #endregion

        #region Win32API 関数
        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [LibraryImport("user32.dll")]
        private static partial IntPtr CreatePopupMenu();

        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16,EntryPoint ="AppendMenuA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string? lpNewItem);

        [LibraryImport("user32.dll", EntryPoint = "InsertMenuItemA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool InsertMenuItem(IntPtr hMenu, uint Item,
            [MarshalAs(UnmanagedType.Bool)] bool fByPosition, IntPtr lpmi);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DestroyMenu(IntPtr hMenu);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static partial int CheckMenuItem(IntPtr hMenu, uint uIDCheckItem, uint uCheck);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetForegroundWindow(IntPtr hWnd);

        [LibraryImport("user32.dll", EntryPoint = "TrackPopupMenuEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool TrackPopupMenu(IntPtr hmenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr lptpm);

        #endregion
        /// <summary>
        /// アイコン追加
        /// </summary>
        /// <param name="Wndhandle"></param>
        /// <param name="iconId"></param>
        /// <param name="callbackMessage"></param>
        /// <param name="iconHandle"></param>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public static unsafe bool TryAddIcon(IntPtr Wndhandle, uint iconId, uint callbackMessage, IntPtr iconHandle, string tooltip)
        {
            var data = new NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = Wndhandle,
                uID = iconId,
                uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
                uCallbackMessage = callbackMessage,
                hIcon = iconHandle,
            };

            char* pTip = data.szTip;
            // fixed char 領域に文字列をコピーする
            // 最大127文字までコピー (最後の1枠はNull終端文字 '\0' のため)
            int len = Math.Min(tooltip.Length, 127);
            for (int i = 0; i < len; i++)
            {
                pTip[i] = tooltip[i];
            }
            pTip[len] = '\0'; // 明示的にNull終端を付与

            return Shell_NotifyIcon(NIM_ADD, ref data);
        }

        /// <summary>
        /// タスクトレイからアイコン削除
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="iconId"></param>
        /// <returns></returns>
        public static bool TryRemoveIcon(IntPtr windowHandle,uint iconId)
        {
            var data = new NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = windowHandle,
                uID = iconId
            };
            return Shell_NotifyIcon(NIM_DELETE, ref data);
        }
        /// <summary>
        /// ポップアップメニュー生成
        /// </summary>
        /// <returns></returns>
        public static IntPtr CreateMenu() => CreatePopupMenu();

        public static bool AddMenuItem(IntPtr menuHandle,uint commandId,string label)
        {
            return AppendMenu(menuHandle, MF_STRING, commandId, label);
        }

        public static bool AddMenuSeparator(IntPtr menuHandle)
        {
            return AppendMenu(menuHandle, MF_SEPARATOR, 0, null);
        }

        public static void ChangeMenuCheckState(IntPtr hmenu,uint menuid,bool Checked)
        {
            uint uCheck = Checked ? MF_CHECKED : MF_UNCHECKED;
            CheckMenuItem(hmenu, menuid, uCheck);
        }

        public static void ShowPopupMenu(IntPtr hWnd, IntPtr hmenu,int x,int y)
        {
            SetForegroundWindow(hWnd);
            TrackPopupMenu(hmenu, 0, x, y, hWnd, IntPtr.Zero);
        }

        public static bool TryDestroyMenu(IntPtr menuHandle)
        {
            return DestroyMenu(menuHandle);
        }

    }
}
