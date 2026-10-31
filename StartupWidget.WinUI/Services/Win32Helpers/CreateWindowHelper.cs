using System;
using System.Runtime.InteropServices;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class CreateWindowHelper
    {
        [LibraryImport("user32.dll",EntryPoint ="RegisterClassExW", SetLastError =true)]
        [return: MarshalAs(UnmanagedType.U2)]
        private static partial ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
        [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16, EntryPoint = "CreateWindowExW")]
        private static partial IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int X, int Y, int nWidth, int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
            );
        [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
        public static partial IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyWindow(IntPtr hWnd);
        

        static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

        public static IntPtr CreateMessageOnlyWindow(string className, IntPtr hInstance)
        {
            return CreateWindowEx(
                0, className, string.Empty, 0,
                0, 0, 0, 0,
                HWND_MESSAGE,
                IntPtr.Zero, hInstance, IntPtr.Zero);
        }

        public static ushort RegisterMessageWindowClass(string className,IntPtr lpfnWndProc,IntPtr hInstance)
        {
            unsafe
            {
                fixed (char* pclassName = className)
                {
                    WNDCLASSEX nw = WNDCLASSEX.Build();
                    nw.lpfnWndProc = lpfnWndProc;
                    nw.hInstance = hInstance;
                    nw.lpszClassName = (IntPtr)pclassName;

                    return RegisterClassEx(ref nw);
                }
            }

        }

    }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
    public unsafe struct WNDCLASSEX
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;// 必須。Build()を使用すれば入る
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public IntPtr lpfnWndProc;//not WndProc。WndProcのデリゲートのMarshal.GetFunctionPointerForDelegate
        public int cvClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;//必須。アプリケーション本体のハンドル
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public IntPtr lpszMenuName;
        public IntPtr lpszClassName;//必須。クラス名
        public IntPtr hIconSm;


        
        public static WNDCLASSEX Build()
        {
            var nw = new WNDCLASSEX();
            nw.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            return nw;
        }
    }
}
