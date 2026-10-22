using StartupWidget.Services.Win32Helpers;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace StartupWidget.Services
{
    public class FileLnkService
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
                throw new COMException("IShellLinkからIPersistFileインターフェースの取得に失敗しました。",unchecked((int)0x80004002));
            }
        }
        
        public unsafe string GetPath()
        {
            char* pBuffer = stackalloc char[ShellLinkHelper.MAX_PATH];

            ShellLinkHelper.WIN32_FIND_DATAW fndData = default;
            shellLink.GetPath(pBuffer, ShellLinkHelper.MAX_PATH, out fndData, ShellLinkHelper.SLGP_FLAGS.SLGP_RAWPATH);
            return new string(pBuffer);
        }

        public unsafe string GetWorkingDirectory()
        {
            char* pBuffer = stackalloc char[ShellLinkHelper.MAX_PATH];
            shellLink.GetWorkingDirectory(pBuffer, ShellLinkHelper.MAX_PATH);
            return new string(pBuffer);
        }

        public unsafe string GetArguments()
        {
            char* pBuffer = stackalloc char[ShellLinkHelper.MAX_PATH];
            shellLink.GetArguments(pBuffer, ShellLinkHelper.MAX_PATH);
            return new string(pBuffer);
        }

        public unsafe string GetIconLocation()
        {
            char* pBuffer = stackalloc char[ShellLinkHelper.MAX_PATH];
            int piIcon;
            shellLink.GetIconLocation(pBuffer, ShellLinkHelper.MAX_PATH, out piIcon);
            return new string(pBuffer);
        }

    }
}
