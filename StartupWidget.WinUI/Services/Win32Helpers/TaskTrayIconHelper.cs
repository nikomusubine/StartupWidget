using System.Runtime.CompilerServices;
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

        public const uint NIM_ADD = 0x00000000;
        public const uint NIM_MODIFY = 0x00000001;
        public const uint NIM_DELETE = 0x00000002;

        public const uint NIF_MESSAGE = 0x00000001;
        public const uint NIF_ICON = 0x00000002;
        public const uint NIF_TIP = 0x00000004;

        public const uint WM_USER = 0x00000400;
        #endregion

        #region Win32API 関数
        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16, EntryPoint = "Shell_NotifyIconW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);


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
    }
}
