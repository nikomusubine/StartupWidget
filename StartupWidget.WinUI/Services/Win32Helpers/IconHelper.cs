using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StartupWidget.Services.Win32Helpers
{
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct SHSTOCKICONINFO
    {
        public const int MAX_PATH = 260;

        public uint cbSize;
        public IntPtr hIcon;
        public int iSysIconIndex;
        public int iIcon;
        public fixed char szPath[MAX_PATH];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct SHFILEINFO{
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        public fixed char szDisplayName[260];
        public fixed char szTypeName[80];
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;
        public int yBitmap;
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }
    [GeneratedComInterface()]
    [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IImageList
    {
        [PreserveSig]
        int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
        [PreserveSig]
        int ReplaceIcon(int i, IntPtr hicon, ref int pi);
        [PreserveSig]
        int SetOverlayImage(int iImage, int iOverlay);
        [PreserveSig]
        int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
        [PreserveSig]
        int AddMasked(IntPtr hbmImage, int crMsk, ref int pi);
        [PreserveSig]
        int Draw(ref IMAGELISTDRAWPARAMS pimldp);
        [PreserveSig]
        int Remove(int i);
        [PreserveSig]
        int GetIcon(int i, int flags, out IntPtr picon);
    }

    public static partial class IconHelper
    {
        public const uint IMAGE_ICON = 1;
        public const uint LR_DEFAULTSIZE = 0x0040;//デフォルトサイズ
        public const uint LR_SHARED = 0x8000; //共有キャッシュを使用

        [LibraryImport("shell32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static partial int SHGetStockIconInfo(uint siid,
            uint uFlags, ref SHSTOCKICONINFO sii);
        
        [LibraryImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DeleteObject(IntPtr hObject);
        [LibraryImport("shell32.dll",EntryPoint ="SHGetFileInfoW",StringMarshalling =StringMarshalling.Utf16)]
        public static partial IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        [LibraryImport("shell32.dll", EntryPoint = "#727")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static partial int SHGetImageList(int iImageList, ref Guid riid, out IntPtr ppv);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyIcon(IntPtr hIcon);

        [LibraryImport("user32.dll", EntryPoint = "LoadImageW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadImage(
            IntPtr hInstance,
            string name,
            uint type,
            int cx, //0でデフォルト
            int cy,
            uint fuLoad);
        [LibraryImport("user32.dll", EntryPoint = "LoadImageW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadImage(
            IntPtr hInstance,
            IntPtr name,
            uint type,
            int cx, //0でデフォルト
            int cy,
            uint fuLoad);

        public static IntPtr LoadIconId(IntPtr hinstnce,int resourceid)
        {
            IntPtr presource = (IntPtr)(resourceid & 0xFFFF);
            return LoadImage(hinstnce, presource, IMAGE_ICON, 0, 0, LR_DEFAULTSIZE | LR_SHARED);
        }
    }
}
