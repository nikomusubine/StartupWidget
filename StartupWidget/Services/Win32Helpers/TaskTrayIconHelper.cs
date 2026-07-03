using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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
        public const uint NIM_ADD = 0x00000000;
        public const uint NIM_MODIFY = 0x0001;
        public const uint NIM_DELETE = 0x0002;

        public const uint NIF_MESSAGE = 0X0001;
        public const uint NIF_ICON = 0x0002;
        public const uint NIF_TIP = 0x0004;

        public const uint WM_USER = 0x0400;

        public const uint MF_STRING = 0x0000;
        public const uint MF_SEPARATOR = 0x0800;
        #endregion

        #region Win32API 関数
        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [LibraryImport("user32.dll")]
        private static partial IntPtr CreatePopupMenu();

        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string? lpNewItem);
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DestroyMenu(IntPtr hMenu);

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
                uFlags= NIF_MESSAGE | NIF_ICON | NIF_TIP,
                uCallbackMessage=callbackMessage,
                hIcon=iconHandle,
            };
            // fixed char 領域に文字列をコピーする
            fixed (char* pTip = data.szTip)
            {
                // 最大127文字までコピー (最後の1枠はNull終端文字 '\0' のため)
                int len = Math.Min(tooltip.Length, 127);
                for (int i = 0; i < len; i++)
                {
                    pTip[i] = tooltip[i];
                }
                pTip[len] = '\0'; // 明示的にNull終端を付与
            }
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

        public static bool TryDestroyMenu(IntPtr menuHandle)
        {
            return DestroyMenu(menuHandle);
        }
    }
}
