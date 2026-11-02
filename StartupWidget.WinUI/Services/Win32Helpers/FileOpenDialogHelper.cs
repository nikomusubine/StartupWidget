using StartupWidget.WinUI.Services.Win32Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using static StartupWidget.Services.Win32Helpers.ShellLinkHelper;
using StartupWidget.Services.Win32Helpers;
using System.IO;

namespace StartupWidget.WinUI.Services.Win32Helpers
{
    public partial class FileOpenDialogHelper
    {
        static readonly Guid CLSID_FileOpenDialog = new Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
        static readonly Guid IID_IFileOpenDialog = new Guid("d57c7288-d4ad-4768-be02-9d969532d960");
        static readonly Guid IID_IShellItem = new("4382693f-9286-47a2-af76-080c191b93a2");
        IFileOpenDialog FileOpenDialog;
        private IntPtr parent;
        public FileOpenDialogHelper(IntPtr owner)
        {
            CoCreateInstance(CLSID_FileOpenDialog, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, IID_IFileOpenDialog, out FileOpenDialog);
            FileOpenDialog.SetOptions(fos);
            parent = owner;                        
        }
        private unsafe string GetDisplayNameFromShellItem(IShellItem ppsi)
        {
            if (ppsi == null)
            {
                return string.Empty;
            }
            int hr = ppsi.GetDisplayName(0, out LPWSTR displayName);
            if (hr != 0)
            {
                return string.Empty;
            }
            string result = new string(displayName);
            COMMemHelper.CoTaskStrFree(ref displayName);
            return result;
        }

        public bool IsDialogShown = false;
        public bool ShowDialog()
        {
            if (IsDialogShown)
            {
                throw new InvalidOperationException("ShowDialogは二度以上呼び出せません。");
            }
            FileOpenDialog.SetOptions(fos);
            int hr = FileOpenDialog.Show(parent);
            IsDialogShown = true;
            if (hr == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Title
        {
            set
            {
                unsafe
                {
                    fixed(LPCWSTR str = value)
                    {
                        FileOpenDialog.SetTitle(str);
                    }
                }
            }
        }

        public string Directory
        {
            get
            {
                unsafe
                {
                    IShellItem shellItem;
                    FileOpenDialog.GetFolder(out shellItem);
                    return GetDisplayNameFromShellItem(shellItem);
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && System.IO.Directory.Exists(value))
                {
                    int hr = SHCreateItemFromParsingName(value, IntPtr.Zero, IID_IShellItem, out IShellItem folderItem);
                    if (hr == 0 && folderItem != null)
                    {
                        FileOpenDialog.SetFolder(folderItem);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException("入力されたフォルダは存在しません。");
                }
            }
        }

        public bool IsFileTypePuted = false;
        public string FileType
        {
            set
            {
                if (IsFileTypePuted)
                {
                    throw new InvalidOperationException("FileTypeは設定済みです。");
                }

                string[] typearray = value.Split('|');
                if (typearray.Length % 2 != 0)
                {
                    throw new ArgumentException("ファイルタイプが不正です。", nameof(value));
                }

                COMDLG_FILTERSPEC[] FilterSpecs = new COMDLG_FILTERSPEC[typearray.Length / 2];

                for (int i = 0; i < typearray.Length; i += 2)
                {
                    COMDLG_FILTERSPEC filter = new COMDLG_FILTERSPEC();

                    unsafe
                    {
                        fixed (char* name = typearray[i])
                        {
                            filter.pszName = name;
                        }
                        fixed (char* spec = typearray[i + 1])
                        {
                            filter.pszSpec = spec;
                        }
                    }

                    FilterSpecs[i / 2] = filter;
                }

                unsafe
                {
                    fixed (COMDLG_FILTERSPEC* rgFilterSpec=FilterSpecs)
                    {
                        FileOpenDialog.SetFileTypes((uint)(typearray.Length / 2), rgFilterSpec);
                        IsFileTypePuted = true;
                    }
                }
            }
        }

        //1始まり
        public uint FileTypeIndex 
        {
            get
            {
                uint value;
                FileOpenDialog.GetFileTypeIndex(out value);
                return value;
            }
            set
            {
                FileOpenDialog.SetFileTypeIndex(value);
            }
        }

        private FOS fos = 
            FOS.FOS_FORCEFILESYSTEM|
            FOS.FOS_FILEMUSTEXIST|
            FOS.FOS_PATHMUSTEXIST|
            FOS.FOS_NOCHANGEDIR;
        private void ChangeFlag(FOS targetFlag, bool value)
        {
            if (value)
            {
                fos |= targetFlag;
            }
            else
            {
                fos &= ~(targetFlag);
            }
        }
        private bool GetFlag(FOS targetFlag)
        {
            return (fos & targetFlag) != 0 ? true : false;
        }

        public bool PickFolders
        {
            get
            {
                return GetFlag(FOS.FOS_PICKFOLDERS);
            }
            set
            {
                ChangeFlag(FOS.FOS_PICKFOLDERS, value);
            }
        }
        /// <summary>
        /// システムに物理的に存在する項目の選択を強制する。
        /// trueで有効、falseで無効。デフォルトはtrue
        /// </summary>
        public bool ForceFileSystem
        {
            get
            {
                return GetFlag(FOS.FOS_FORCEFILESYSTEM);
            }
            set
            {
                ChangeFlag(FOS.FOS_FORCEFILESYSTEM, value);
            }
        }

        public bool AllowMultiSelect
        {
            get
            {
                return GetFlag(FOS.FOS_ALLOWMULTISELECT);
            }
            set
            {
                ChangeFlag(FOS.FOS_ALLOWMULTISELECT, value);
            }
        }

        public bool FileMustExist
        {
            get
            {
                return GetFlag(FOS.FOS_FILEMUSTEXIST);
            }
            set
            {
                ChangeFlag(FOS.FOS_FILEMUSTEXIST, value);
            }
        }

        public bool PathMustExist
        {
            get
            {
                return GetFlag(FOS.FOS_PATHMUSTEXIST);
            }
            set
            {
                ChangeFlag(FOS.FOS_PATHMUSTEXIST, value);
            }
        }

        /// <summary>
        /// 指定された拡張子に一致するファイルのみを表示選択できるようにする。
        /// </summary>
        public bool StrictFileTypes
        {
            get
            {
                return GetFlag(FOS.FOS_STRICTFILETYPES);
            }
            set
            {
                ChangeFlag(FOS.FOS_STRICTFILETYPES, value);
            }
        }

        public bool NoChangeDir
        {
            get
            {
                return GetFlag(FOS.FOS_NOCHANGEDIR);
            }
            set
            {
                ChangeFlag(FOS.FOS_NOCHANGEDIR, value);
            }
        }



        public string Result
        {
            get
            {
                unsafe
                {
                    FileOpenDialog.GetResult(out IShellItem ppsi);
                    return GetDisplayNameFromShellItem(ppsi);
                }
            }
        }

        public string[] Results
        {
            get
            {
                unsafe
                {
                    FileOpenDialog.GetResults(out IShellItemArray ppenum);
                    ppenum.GetCount(out uint count);
                    string[] result = new string[count];
                    for(uint i = 0; i < count; i++)
                    {
                        ppenum.GetItemAt(i, out IShellItem ppsi);
                        result[i] = GetDisplayNameFromShellItem(ppsi);
                    }
                    return result;
                }
            }
        }


        [LibraryImport("ole32.dll")]
        public static partial int CoCreateInstance(
            in Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, in Guid riid,
            [MarshalUsing(typeof(ComInterfaceMarshaller<IFileOpenDialog>))] out IFileOpenDialog ppv);

        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int SHCreateItemFromParsingName(
            string pszPath,
            IntPtr pbc,
            in Guid riid,
            [MarshalUsing(typeof(ComInterfaceMarshaller<IShellItem>))] out IShellItem ppv);
    }
}