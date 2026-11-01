using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Services.Win32Helpers.Interfaces
{

    [GeneratedComInterface]
    [Guid("4382693f-9286-47a2-af76-080c191b93a2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IShellItem
    {   
        void BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut);
        void GetParent(out IShellItem ppsi);
        //要解放
        [PreserveSig] int GetDisplayName(uint sigdnName, out LPWSTR ppszName);
    }

}
