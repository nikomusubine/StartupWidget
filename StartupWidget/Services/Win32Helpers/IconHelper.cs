using System.Runtime.InteropServices;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class IconHelper
    {
        public const uint IMAGE_ICON = 1;
        public const uint LR_DEFAULTSIZE = 0x0040;//デフォルトサイズ
        public const uint LR_SHARED = 0x8000; //共有キャッシュを使用

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
