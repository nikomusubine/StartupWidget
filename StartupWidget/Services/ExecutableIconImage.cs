using StartupWidget.Models.Types;
using StartupWidget.Services.Interfaces;
using StartupWidget.Services.Win32Helpers;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;

namespace StartupWidget.Services
{
    public class ExecutableIconImage : IExecutableIconImage
    {
        #region Win32API
        public const uint SIID_SHIELD = 0x00000004D;
        public const uint SHGSI_ICON = 0x000000100;
        public const uint SHGSI_LARGEICON = 0x000000000;
        public const uint SHGSI_SMALLICON = 0x000000001;
        public const uint SHGFI_SYSICONINDEX = 0x00004000;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x00000010;
        public const int SHIL_EXTRALARGE = 0x2;
        public const int SHIL_JUMBO = 0x4;

        /// <summary>
        /// UAC盾アイコンを取得する
        /// </summary>
        /// <param name="smallSize">小さいアイコン(16x16)を取得する時はtrue。
        /// 大きいいアイコン(32x32)を取得する時はfalse。</param>
        /// <returns>UAC盾アイコンのIconオブジェクト。
        /// Windows Vista未満のOSの時はnullを返す。</returns>
        public static ImageSource? GetShieldIcon()
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
            int res = IconHelper.SHGetStockIconInfo(SIID_SHIELD,
                SHGSI_ICON | (false ? SHGSI_SMALLICON : SHGSI_LARGEICON),
                ref sii);

            //失敗した時は例外をスローする
            if (res != 0)
            {
                Marshal.ThrowExceptionForHR(res);
            }

            //ImageSourceオブジェクトを作成する
            try
            {
                BitmapSource source = Imaging.CreateBitmapSourceFromHIcon(sii.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {

            }
        }

        public static ImageSource? GetFileIcon(string filePath, bool isLarge = true)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            uint flags = SHGSI_ICON | (isLarge ? SHGSI_LARGEICON : SHGSI_SMALLICON);
            IntPtr result = IconHelper.SHGetFileInfo(filePath, 0, ref shfi, (uint)Marshal.SizeOf(shfi), flags);
            if (result == IntPtr.Zero || shfi.hIcon == IntPtr.Zero)
            { return null; }

            try
            {
                BitmapSource source = Imaging.CreateBitmapSourceFromHIcon(
                    shfi.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                return source;
            }
            finally
            {
                IconHelper.DestroyIcon(shfi.hIcon);
            }
        }

        public static ImageSource? GetJumboIcon(string filePath)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr getInfoResult = IconHelper.SHGetFileInfo(filePath, 0, ref shfi, (uint)Marshal.SizeOf(shfi), SHGFI_SYSICONINDEX | SHGFI_USEFILEATTRIBUTES);
            int iconIndex = shfi.iIcon;

            Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            IntPtr pImageList;

            int hResult = IconHelper.SHGetImageList(SHIL_JUMBO, ref iidImageList, out pImageList);

            if (hResult == 0 && pImageList != IntPtr.Zero)
            {
                IImageList imageList = (IImageList)Marshal.GetObjectForIUnknown(pImageList);
                Marshal.Release(pImageList);

                IntPtr hIcon = IntPtr.Zero;
                int getIconResult= imageList.GetIcon(iconIndex, 1, out hIcon);


                if (hIcon != IntPtr.Zero)
                {
                    try
                    {
                        BitmapSource bmpSource = Imaging.CreateBitmapSourceFromHIcon(
                            hIcon,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        if (CheckTransparentSpace(bmpSource,64))
                        {
                            return GetFileIcon(filePath);
                        }
                        bmpSource.Freeze();
                        return bmpSource;
                    }
                    finally
                    {
                        IconHelper.DestroyIcon(hIcon);
                    }
                }
            }
            return null;
        }

        private static bool CheckTransparentSpace(BitmapSource source, int offset)
        {
            int width = source.PixelWidth, height = source.PixelHeight;

            if (width <= offset || height <= offset) return false;

            int targetx = width - offset;
            int targety = height - offset;

            byte[] row = new byte[width * 4];
            source.CopyPixels(new Int32Rect(0, targety, width, 1), row, width * 4, 0);

            for (int i = 3; i < row.Length; i += 4)
            {
                if (row[i] > 0) return false;
            }

            byte[] column = new byte[height * 4];
            source.CopyPixels(new Int32Rect(targetx, 0, 1, height), column, 4, 0);

            for (int i = 3; i < row.Length; i += 4)
            {
                if (column[i] > 0) return false;
            }
            return true;
        }

        #endregion
        public ExecutableIconImage()
        {
            Iconcache = new Dictionary<string, ImageSource>();
        }
        private Dictionary<string, ImageSource> Iconcache;
        private ImageSource? ShieldIconCache;
        public ImageSource GetIconImage(ExecutableProcess process)
        {
            if (string.IsNullOrWhiteSpace(process.FilePath) || !File.Exists(process.FilePath))
            {
                return GetEmptyImageSource();
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
                ImageSource imageSource = GetJumboIcon(process.FilePath)
                    ?? GetEmptyImageSource();
                Iconcache.Add(process.FilePath, imageSource);
                return imageSource;
            }
            else //IconPath指定
            {
                if (string.Equals(Path.GetExtension(process.IconFilePath),".exe",StringComparison.OrdinalIgnoreCase))
                {
                    ImageSource imageSource = GetJumboIcon(process.IconFilePath)??GetEmptyImageSource();
                    Iconcache.Add(process.IconFilePath, imageSource);
                    return imageSource;
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(process.IconFilePath);
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
        }

        public ImageSource GetShieldIconImage()
        {
            if(ShieldIconCache == null)
            {
                ShieldIconCache = GetShieldIcon();
            }
            return ShieldIconCache?? GetEmptyImageSource();
        }

        private static ImageSource GetEmptyImageSource()
        {
            return new WriteableBitmap(1, 1, 1, 1, System.Windows.Media.PixelFormats.Bgra32, null);// 空のImageSourceを生成
        }
    }
}
