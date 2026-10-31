using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Services.Win32Helpers
{
    public partial class WindowMessageHelper
    {
        

        [LibraryImport("comctl32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        public static partial IntPtr DefSubclassProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr SUBCLASSPROC(
            IntPtr hWnd,
            int uMsg,
            IntPtr wParam,
            IntPtr lParam,
            UIntPtr uIdSubclass,
            IntPtr dwRefData
        );

        /// <summary>
        /// ウィンドウのサブクラスコールバック関数を登録します。
        /// </summary>
        [LibraryImport("comctl32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowSubclass(
            IntPtr hWnd,
            SUBCLASSPROC pfnSubclass,
            UIntPtr uIdSubclass,
            IntPtr dwRefData
        );

        [LibraryImport("comctl32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool RemoveWindowSubclass(
            IntPtr hWnd, SUBCLASSPROC pfnSubClass, UIntPtr uIdSubclass);
    }
}