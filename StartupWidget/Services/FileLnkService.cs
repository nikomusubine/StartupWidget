using StartupWidget.Services.Win32Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace StartupWidget.Services
{
    public class FileLnkService: IDisposable
    {
        ShellLinkHelper.IShellLinkW shellLink;
        ShellLinkHelper.IPersistFile persistFile;

        public FileLnkService(string LnkPath)
        {
            int hr = ShellLinkHelper.CoCreateInstance(ShellLinkHelper.CLSID_ShellLink,
                IntPtr.Zero,
                ShellLinkHelper.CLSCTX.CLSCTX_INPROC_SERVER,
                ShellLinkHelper.IID_IShellLinkW,
                out shellLink);

            if (hr != 0 || shellLink == null)
            {
                throw new Win32Exception(hr);
            }

            if (shellLink is ShellLinkHelper.IPersistFile pfl)
            {
                persistFile = pfl;
                unsafe
                {
                    fixed(char * pPath = LnkPath)
                    {
                        persistFile.Load(pPath, ShellLinkHelper.STGM_READ);
                    }
                }
            }
            else
            {
                throw new COMException("IShellLinkからIPersistFileインターフェースの取得に失敗",unchecked((int)0x80004002));
            }
        }

        public void Dispose() // 本当に必要？
        {
            if(persistFile is IDisposable dp)
            {
                dp.Dispose();
            }
            if(shellLink is IDisposable ds)
            {
                ds.Dispose();
            }
        }

        public static string ResolveShortcut(string filename)
        {
            ShellLink link = new ShellLink();

            Span<char> fname = new Span<char>(filename.ToArray());
            unsafe
            {
                fixed (char* fnameptr = fname)
                {
                    ((IPersistFile)link).Load(fnameptr, STGM_READ);
                    // TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.  
                    // ((IShellLinkW)link).Resolve(hwnd, 0) 
                    fixed char* pszFile = new char[256];
                    WIN32_FIND_DATAW data = new WIN32_FIND_DATAW();
                    ((IShellLinkW)link).GetPath(pszFile, 2556, out data, 0);
                    return sb.ToString();
                }
            }
        }
    }
}
