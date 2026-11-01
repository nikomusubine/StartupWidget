using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace StartupWidget.WinUI.Services.Win32Helpers.Interfaces
{

    [StructLayout(LayoutKind.Sequential)]
    public struct PROPERTYKEY
    {
        public Guid fmtid;
        public UInt32 pid;
    }

    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 1,
        SIATTRIBFLAGS_OR = 2,
        SIATTRIBFLAGS_APPCOMPAT = 3,
        SIATTRIBFLAGS_MASK = 3,
        SIATTRIBFLAGS_ALLITEMS = 16384
    }

        [GeneratedComInterface, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IShellItemArray
    {
        // Not supported: IBindCtx

        void BindToHandler(ref IntPtr pbc, ref Guid rbhid,
                      ref Guid riid, out IntPtr ppvOut);


        void GetPropertyStore( int Flags,  ref Guid riid, out IntPtr ppv);


        void GetPropertyDescriptionList( ref PROPERTYKEY keyType,  ref Guid riid, out IntPtr ppv);


        void GetAttributes( SIATTRIBFLAGS dwAttribFlags,  uint sfgaoMask, out uint psfgaoAttribs);


        void GetCount(out uint pdwNumItems);


        void GetItemAt( uint dwIndex,out IShellItem ppsi);

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)

        void EnumItems( out IntPtr ppenumShellItems);
    }
}
