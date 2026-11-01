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
    public partial class FileOpenDialogHelper
    {
        static readonly Guid CLSID_FileOpenDialog = new Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
        static readonly Guid IID_IFileOpenDialog = new Guid("d57c7288-d4ad-4768-be02-9d969532d960");
        static readonly Guid IID_IShellItem = new("4382693f-9286-47a2-af76-080c191b93a2");
        IFileOpenDialog FileOpenDialog;
        public FileOpenDialogHelper()
        {
            CoCreateInstance(CLSID_FileOpenDialog, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, IID_IFileOpenDialog, out FileOpenDialog);
        }

        public void ShowDialog(IntPtr parent)
        {
            FileOpenDialog.Show(parent);
        }

        public string FIleType
        {
            set
            {
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