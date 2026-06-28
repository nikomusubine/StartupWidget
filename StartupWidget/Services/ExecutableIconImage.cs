using StartupWidget.Models.Types;
using StartupWidget.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace StartupWidget.Services
{
    public class ExecutableIconImage : IExecutableIconImage
    {
        #region Win32API
        private const int MAX_PATH = 260;
        private const uint SIID_SHIELD = 0x00000004D;
        private const uint SHGSI_ICON = 0x000000100;
        private const uint SHGSI_LARGEICON = 0x000000000;
        private const uint SHGSI_SMALLICON = 0x000000001;

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPath;
        }

        [DllImport("shell32.dll", SetLastError = false)]
        private static extern int SHGetStockIconInfo(uint siid,
            uint uFlags, ref SHSTOCKICONINFO sii);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// UAC盾アイコンを取得する
        /// </summary>
        /// <param name="smallSize">小さいアイコン(16x16)を取得する時はtrue。
        /// 大きいいアイコン(32x32)を取得する時はfalse。</param>
        /// <returns>UAC盾アイコンのIconオブジェクト。
        /// Windows Vista未満のOSの時はnullを返す。</returns>
        public static Icon? GetShieldIcon()
        {
            //Windows Vista以上か確認する
            if (Environment.OSVersion.Platform != PlatformID.Win32NT ||
                Environment.OSVersion.Version.Major < 6)
            {
                return null;
            }

            SHSTOCKICONINFO sii = new SHSTOCKICONINFO();
            sii.cbSize = (uint)Marshal.SizeOf(typeof(SHSTOCKICONINFO));
            //盾アイコンを取得する
            int res = SHGetStockIconInfo(SIID_SHIELD,
                SHGSI_ICON | (false ? SHGSI_SMALLICON : SHGSI_LARGEICON),
                ref sii);

            //失敗した時は例外をスローする
            if (res != 0)
            {
                Marshal.ThrowExceptionForHR(res);
            }

            //Iconオブジェクトを作成する
            return Icon.FromHandle(sii.hIcon);
        }
        #endregion
        public ExecutableIconImage()
        {
            Iconcache = new Dictionary<string, ImageSource>();
        }
        private Dictionary<string, ImageSource> Iconcache;
        private ImageSource ConvertFromBitmap(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        public ImageSource GetIconImage(ExecutableProcess process)
        {
            if (string.IsNullOrWhiteSpace(process.FilePath) || !File.Exists(process.FilePath))
            {
                Bitmap bmp = new Bitmap(16, 16);
                return ConvertFromBitmap(bmp);
            }

            if (!string.IsNullOrEmpty(process.IconFilePath)) //カスタムアイコンの場合
            {
                if (Iconcache.ContainsKey(process.IconFilePath))
                {
                    return Iconcache[process.IconFilePath];
                }
            }
            else if (Iconcache.ContainsKey(process.FilePath))
            {
                return Iconcache[process.FilePath];
            }

            //IconPath指定なし
            if (process.IconFilePath == "" || process.IconFilePath == null)
            {
                Icon? ico = Icon.ExtractAssociatedIcon(process.FilePath);
                if (ico == null)
                {
                    throw new FileNotFoundException(nameof(process.IconFilePath));
                }

                Bitmap bitmap = ico.ToBitmap();
                if (process.RunasAdmin)
                {
                    //盾アイコン取得
                    Icon? Shield = GetShieldIcon();
                    if (Shield != null)
                    {
                        //合成
                        Graphics graphics = Graphics.FromImage(bitmap);
                        float Devision = 2;
                        Image shield = new Bitmap(Shield.ToBitmap(), new System.Drawing.Size((int)(bitmap.Width / Devision), (int)(bitmap.Height / Devision)));
                        graphics.DrawImage(shield, bitmap.Width - (bitmap.Width / Devision), bitmap.Height - (bitmap.Height / Devision), shield.Width, shield.Height);
                        graphics.Dispose();
                    }
                }
                ImageSource imageSource = ConvertFromBitmap(bitmap);
                Iconcache.Add(process.FilePath, imageSource);
                return imageSource;
            }
            else //IconPath指定
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(process.IconFilePath);

                ImageSource imageSource = ConvertFromBitmap(bmp);
                Iconcache.Add(process.IconFilePath, imageSource);
                return imageSource;
            }
        }
    }
}
