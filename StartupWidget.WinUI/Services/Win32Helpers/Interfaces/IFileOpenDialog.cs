using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;



namespace StartupWidget.WinUI.Services.Win32Helpers.Interfaces
{
    public enum FDAP
    {
        FDAP_BOTTOM = 0,
        FDAP_TOP = 1
    };
    public enum FOS : uint
    {
        FOS_OVERWRITEPROMPT = 0x00000002,
        FOS_STRICTFILETYPES = 0x00000004,
        FOS_NOCHANGEDIR = 0x00000008,
        FOS_PICKFOLDERS = 0x00000020,
        FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
        FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
        FOS_NOVALIDATE = 0x00000100,
        FOS_ALLOWMULTISELECT = 0x00000200,
        FOS_PATHMUSTEXIST = 0x00000800,
        FOS_FILEMUSTEXIST = 0x00001000,
        FOS_CREATEPROMPT = 0x00002000,
        FOS_SHAREAWARE = 0x00004000,
        FOS_NOREADONLYRETURN = 0x00008000,
        FOS_NOTESTFILECREATE = 0x00010000,
        FOS_HIDEMRUPLACES = 0x00020000,
        FOS_HIDEPINNEDPLACES = 0x00040000,
        FOS_NODEREFERENCELINKS = 0x00100000,
        FOS_DONTADDTORECENT = 0x02000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct COMDLG_FILTERSPEC
    {
        public LPCWSTR pszName;
        public LPCWSTR pszSpec;
    }

    [GeneratedComInterface, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IFileOpenDialog
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer
        // --------------------------------------------------------------------------------
        [PreserveSig]
        int Show( IntPtr parent);

        // Defined on IFileDialog - repeated here due to requirements of COM interop layer

        void SetFileTypes( uint cFileTypes,  COMDLG_FILTERSPEC* rgFilterSpec);


        void SetFileTypeIndex( uint iFileType);


        void GetFileTypeIndex(out uint piFileType);


        void Advise(IntPtr pfde, out uint pdwCookie);


        void Unadvise( uint dwCookie);


        void SetOptions( FOS fos);


        void GetOptions(out FOS pfos);


        void SetDefaultFolder( IShellItem psi);


        void SetFolder( IShellItem psi);


        void GetFolder( out IShellItem ppsi);


        void GetCurrentSelection( out IShellItem ppsi);


        void SetFileName( LPCWSTR pszName);

        //呼び出し元のアプリケーションはCoTaskMemFreeを使用して取得したバッファーを解放
        void GetFileName(out LPWSTR pszName);


        void SetTitle( LPCWSTR pszTitle);


        void SetOkButtonLabel( LPCWSTR pszText);


        void SetFileNameLabel( LPCWSTR pszLabel);


        void GetResult(out IShellItem ppsi);


        void AddPlace(IShellItem psi, FDAP fdap);


        void SetDefaultExtension(LPCWSTR pszDefaultExtension);


        void Close(int hr);


        void SetClientGuid( ref Guid guid);


        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr

        void SetFilter( IntPtr pFilter);

        // Defined by IFileOpenDialog
        // ---------------------------------------------------------------------------------

        void GetResults( out IShellItemArray ppenum);


        void GetSelectedItems( out IShellItemArray ppsai);
    }
}
