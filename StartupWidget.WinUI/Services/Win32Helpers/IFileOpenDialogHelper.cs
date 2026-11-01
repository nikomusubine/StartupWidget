using StartupWidget.WinUI.Services.Win32Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using static StartupWidget.Services.Win32Helpers.ShellLinkHelper;

namespace StartupWidget.WinUI.Services.Win32Helpers
{
    public static partial class IFileOpenDialogHelper
    {
        [LibraryImport("ole32.dll")]
        public static partial int CreateInstance(
            in Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, in Guid riid,
            [MarshalUsing(typeof(ComInterfaceMarshaller<IFileOpenDialog>))] out IFileOpenDialog ppv);

    }
}