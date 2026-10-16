using System;
using System.Collections.Generic;
using System.Text;
using static StartupWidget.Services.Win32Helpers.ShellLinkHelper;

namespace StartupWidget.Services
{
    public class FileLnkService
    {
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
