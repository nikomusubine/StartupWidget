using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StartupWidget.Services.Win32Helpers
{
    internal static partial class ContextMenuHelper
    {
        #region Win32構造体と定数
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
        #region Win32API関数
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
