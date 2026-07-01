using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace StartupWidget.Behaviours.Win32Helpers
{
    public partial class WindowMove
    {   

        /*
            メモ: DIP、ピクセル変換が必要
         */
        bool _isSnapEnabled = false;
        public bool IsSnapEnabled
        {
            get => _isSnapEnabled;
            set => _isSnapEnabled = value;
        }
        bool _isMoveEnabled = false;
        public bool IsMoveEnabled
        {
            get => _isMoveEnabled;
            set=> _isMoveEnabled = value;
        }
        int _snapThreshold = 12;
        public int SnapThreshold
        {
            get => _snapThreshold;
            set=> _snapThreshold = value;
        }

        private IntPtr _hWnd;
        private HwndSource? _hwndSource;

        public WindowMove(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _hwndSource = HwndSource.FromHwnd(_hWnd);
            if (_hwndSource != null)
            {
                _hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left,top,right,bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MONITORINFO
        {
            public int cbSize; //構造体のサイズ
            public RECT rcMonitor; //モニターの表示領域。プライマリでないと、一部の値が負になる。
            public RECT rcWork; //モニターの作業領域。上と同様
            public int dwFlags;　//ディスプレイモニターの属性を表すフラグ
        }
        [LibraryImport("user32.dll",SetLastError =false)]
        private static partial IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [LibraryImport("user32.dll")]
        [return:MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetWindowRect(IntPtr hwnd, out RECT lpRect);//Window位置取得
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ReleaseCapture();

        private int ExaminingDifference(int baseValue, int evaluate)
        {
            if ( Math.Abs(baseValue-evaluate) <= _snapThreshold)
            {
                return baseValue;
            }
            return evaluate;
        }

        public void DragMove()
        {

        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_MOVING = 0x0216;
            const int WM_EXITSIZEMOVE = 0x0232;
            if (msg == WM_MOVING)
            {
                if (!_isMoveEnabled)
                {
                    RECT current;
                    GetWindowRect(hwnd, out current);
                    unsafe
                    {
                        RECT* p = (RECT*)lParam.ToPointer();
                        p->left = current.left;
                        p->top = current.top;
                        p->right = current.right;
                        p->bottom = current.bottom;
                    }
                }
                else
                {
                    if (_isSnapEnabled)
                    {
                        IntPtr hMonitor = MonitorFromWindow(hwnd, 0);
                        MONITORINFO monitorinfo = new MONITORINFO();
                        GetMonitorInfo(hMonitor, ref monitorinfo);

                        unsafe
                        {
                            RECT* p = (RECT*)lParam.ToPointer();
                            p->left = ExaminingDifference(monitorinfo.rcWork.left, p->left);
                            p->top = ExaminingDifference(monitorinfo.rcWork.top, p->top);
                            p->right = ExaminingDifference(monitorinfo.rcWork.right, p->right);
                            p->bottom = ExaminingDifference(monitorinfo.rcWork.bottom, p->bottom);
                        }
                    }
                }
                return IntPtr.Zero;
            }
            else if (msg == WM_EXITSIZEMOVE)
            {
                // Handle exit size/move if needed
            }
            return IntPtr.Zero;
        }
    }
}
