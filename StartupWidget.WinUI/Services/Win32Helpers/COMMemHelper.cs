using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Services.Win32Helpers
{
    public static unsafe class COMMemHelper
    {
        public static void FreeCoTaskStr(ref LPWSTR ppsz)
        {
            if (ppsz != null)
            {
                Marshal.FreeCoTaskMem((IntPtr)(ppsz));
                ppsz = null;
            }
        }
        public static void FreeCoTaskMem(ref IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(ptr);
                ptr = IntPtr.Zero;
            }
        }
    }
}
