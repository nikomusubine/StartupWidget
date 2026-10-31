using System;
using static StartupWidget.WinUI.Services.Win32Helpers.WindowMessageHelper;
using static StartupWidget.Constants;

namespace StartupWidget.WinUI.Services
{
    public class WndProcMessageService:IDisposable
    {
        public event EventHandler<WndProcEventArgs>? OnWindowEvent;

        private SUBCLASSPROC _subClassProc;
        private IntPtr _hWnd;

        public WndProcMessageService(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                throw new ArgumentException("hWndは無効です。", nameof(hWnd));
            }
            _hWnd = hWnd;
            _subClassProc = new SUBCLASSPROC(CustomSubClassProc);
            SetWindowSubclass(_hWnd, _subClassProc, AppUniqueNumber, IntPtr.Zero);
        }

        private IntPtr CustomSubClassProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, UIntPtr idSubclass, IntPtr refData)
        {
            bool handled = false;
            if (_hWnd == hWnd)
            {
                if (OnWindowEvent != null)
                {
                    WndProcEventArgs args = new WndProcEventArgs(hWnd, msg, wParam, lParam);
                    foreach (EventHandler<WndProcEventArgs> handler in OnWindowEvent.GetInvocationList())
                    {
                        handler(this, args);
                        if (args.Handled)
                        {
                            return args.Result;
                        }
                    }
                }
            }
            return DefSubclassProc(hWnd, msg, wParam, lParam);
        }
        public void Dispose()
        {
            RemoveWindowSubclass(_hWnd, _subClassProc, AppUniqueNumber);
        }
    }

    public class WndProcEventArgs : EventArgs
    {
        public IntPtr HWnd { get; }
        public int Msg { get; }
        public IntPtr WParam { get; }
        public IntPtr LParam { get; }
        public bool Handled { get; set; }
        public IntPtr Result { get; set; } = IntPtr.Zero;

        public WndProcEventArgs(IntPtr hWnd,int msg,IntPtr wParam,IntPtr lParam)
        {
            HWnd = hWnd;
            Msg = msg;
            WParam = wParam;
            LParam = lParam;
        }

    }
}
