using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class TaskTrayIconHelper
    {
        #region Win32API構造体と定数
        [InlineArray(128)]
        public struct CharBuffer128
        {
            private char _element;

            public static CharBuffer128 Build(string str)
            {
                CharBuffer128 buffer = new();
                ReadOnlySpan<char> span = str;
                span.CopyTo(buffer);
                return buffer;
            }
        }
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
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct MENUITEMINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public char* dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;
        }
        public const uint NIM_ADD = 0x00000000;
        public const uint NIM_MODIFY = 0x00000001;
        public const uint NIM_DELETE = 0x00000002;

        public const uint NIF_MESSAGE = 0x00000001;
        public const uint NIF_ICON = 0x00000002;
        public const uint NIF_TIP = 0x00000004;

        public const uint WM_USER = 0x00000400;

        public const uint MF_STRING = 0x00000000;
        public const uint MF_SEPARATOR = 0x00000800;
        public const uint MF_BYCOMMAND = 0x00000000;
        public const uint MF_CHECKED = 0x00000008;
        public const uint MF_UNCHECKED = 0x00000000;

        public const uint MIIM_STRING = 0x00000040;
        public const uint MIIM_ID = 0x00000002;
        public const uint MIIM_BITMAP = 0x00000080;

        public const uint TPM_NONOTIFY = 0x0080;
        public const uint TPM_RETURNCMD = 0x0100;
        #endregion

        #region Win32API 関数
        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16, EntryPoint = "Shell_NotifyIconW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [LibraryImport("user32.dll")]
        private static partial IntPtr CreatePopupMenu();

        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16, EntryPoint = "AppendMenuW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string? lpNewItem);

        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16, EntryPoint = "InsertMenuItemW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool InsertMenuItem(IntPtr hMenu, uint Item,
            [MarshalAs(UnmanagedType.Bool)] bool fByPosition, ref MENUITEMINFO lpmi);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DestroyMenu(IntPtr hMenu);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static partial int CheckMenuItem(IntPtr hMenu, uint uIDCheckItem, uint uCheck);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetForegroundWindow(IntPtr hWnd);

        [LibraryImport("user32.dll", EntryPoint = "TrackPopupMenuEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static partial int TrackPopupMenu(IntPtr hmenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr lptpm);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetCursorPos(out Point lpPoint);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial int GetMenuItemCount(IntPtr hMenu);

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
        public static bool TryRemoveIcon(IntPtr windowHandle, uint iconId)
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

        public static bool AddMenuItem(IntPtr menuHandle, uint commandId, string label)
        {
            return AppendMenu(menuHandle, MF_STRING, commandId, label);
        }

        public static bool AddMenuSeparator(IntPtr menuHandle)
        {
            return AppendMenu(menuHandle, MF_SEPARATOR, 0, null);
        }

        public static unsafe bool AddMenuItemImage(IntPtr menuHandle, uint commandId, string label, IntPtr hbitmap)
        {
            var mii = new MENUITEMINFO();
            mii.cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO));
            mii.fMask = MIIM_STRING | MIIM_ID | MIIM_BITMAP;
            mii.fType = MF_STRING;
            if (hbitmap == IntPtr.Zero)
            {
                return false;
            }
            mii.hbmpItem = hbitmap;
            mii.wID = commandId;
            mii.cch = (uint)label.Length;


            fixed (char* plabel = label)
            {
                mii.dwTypeData = plabel;
                return InsertMenuItem(menuHandle, commandId, false, ref mii);
            }
        }

        public static void ChangeMenuCheckState(IntPtr hmenu, uint menuid, bool Checked)
        {
            uint uCheck = Checked ? MF_CHECKED : MF_UNCHECKED;
            CheckMenuItem(hmenu, menuid, uCheck);
        }

        public static void ShowPopupMenu(IntPtr hWnd, IntPtr hmenu, int x, int y)
        {
            SetForegroundWindow(hWnd);
            TrackPopupMenu(hmenu, 0, x, y, hWnd, IntPtr.Zero);
        }

        public static int ShowPopupMenuWithIndex(IntPtr hWnd, IntPtr hmenu, int x, int y)
        {
            SetForegroundWindow(hWnd);
            return TrackPopupMenu(hmenu, TPM_NONOTIFY | TPM_RETURNCMD, x, y, hWnd, IntPtr.Zero);
        }

        public static bool TryDestroyMenu(IntPtr menuHandle)
        {
            return DestroyMenu(menuHandle);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }
}
