using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class HBitmapHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct BitmapInfoHeader
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct BitmapInfo
        {
            public BitmapInfoHeader bmiHeader;
            public int bmiColors;
        }

        [LibraryImport("gdi32.dll", SetLastError = true)]
        public static partial IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [LibraryImport("gdi32.dll", SetLastError = true)]
        public static partial IntPtr CreateDIBSection(IntPtr hdc, ref BitmapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [LibraryImport("gdi32.dll", SetLastError = true)]
        public static partial int SetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, byte[] lpvBits, ref BitmapInfo lpbmi, uint fuColorUse);
        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial IntPtr GetDC(IntPtr hWnd);
        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [LibraryImport("gdi32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DeleteObject(IntPtr hObject);
        [LibraryImport("user32.dll")]
        private static partial int GetSystemMetrics(int nIndex);
        private const int SM_CXSMICON = 49;
        private const int SM_CYSMICON = 50;


        public static IntPtr ConvertToHBitmap(BitmapSource source)
        {
            if (source == null) return IntPtr.Zero;
            int width = source.PixelWidth;
            int height = source.PixelHeight;

            int targetWidth = GetSystemMetrics(SM_CXSMICON);
            int targetHeight = GetSystemMetrics(SM_CYSMICON);

            double scaleX = (double)targetWidth / width;
            double scaleY = (double)targetHeight / height;

            BitmapSource scaledsource = new TransformedBitmap(source, new ScaleTransform(scaleX, scaleY));

            if (scaledsource.Format != PixelFormats.Pbgra32)
            {
                scaledsource = new FormatConvertedBitmap(scaledsource, PixelFormats.Pbgra32, null, 0);
            }
            width = scaledsource.PixelWidth;
            height = scaledsource.PixelHeight;

            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            scaledsource.CopyPixels(Int32Rect.Empty, pixels, stride, 0);


            BitmapInfo bmi = new();
            bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BitmapInfoHeader));
            bmi.bmiHeader.biWidth = width;
            bmi.bmiHeader.biHeight = -height;
            bmi.bmiHeader.biPlanes = 1;
            bmi.bmiHeader.biBitCount = 32;
            bmi.bmiHeader.biCompression = 0;

            IntPtr screenDC = GetDC(IntPtr.Zero);
            
            IntPtr hBitmap = CreateDIBSection(screenDC,ref bmi,0,out IntPtr _, IntPtr.Zero,0);

            SetDIBits(screenDC, hBitmap, 0, (uint)height, pixels, ref bmi, 0);

            ReleaseDC(IntPtr.Zero, screenDC);

            return hBitmap;
        }
    }
}
