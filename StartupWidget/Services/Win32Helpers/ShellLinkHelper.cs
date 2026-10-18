using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace StartupWidget.Services.Win32Helpers
{
    public static partial class ShellLinkHelper
    {
        public static readonly Guid CLSID_ShellLink = new("00021401-0000-0000-C000-000000000046");
        public static readonly Guid IID_IShellLinkW = new("000214F9-0000-0000-C000-000000000046");
        [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static unsafe partial int SHGetFolderPathW(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, char* lpszPath);

        [Flags()]
        public enum SLGP_FLAGS
        {
            /// <summary>Retrieves the standard short (8.3 format) file name</summary>
            SLGP_SHORTPATH = 0x1,
            /// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
            SLGP_UNCPRIORITY = 0x2,
            /// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
            SLGP_RAWPATH = 0x4
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
        public unsafe struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            public fixed char cFileName[260];
            public fixed char cAlternateFileName[14];
        }
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MULTI_QI
        {
            [MarshalAs(UnmanagedType.LPStruct)] public Guid pIID;
            [MarshalAs(UnmanagedType.Interface)] public object pItf;
            public int hr;
        }

        [Flags()]
        public enum SLR_FLAGS
        {
            /// <summary>
            /// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
            /// the high-order word of fFlags can be set to a time-out value that specifies the
            /// maximum amount of time to be spent resolving the link. The function returns if the
            /// link cannot be resolved within the time-out duration. If the high-order word is set
            /// to zero, the time-out duration will be set to the default value of 3,000 milliseconds
            /// (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
            /// duration, in milliseconds.
            /// </summary>
            SLR_NO_UI = 0x1,
            /// <summary>Obsolete and no longer used</summary>
            SLR_ANY_MATCH = 0x2,
            /// <summary>If the link object has changed, update its path and list of identifiers.
            /// If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
            /// whether or not the link object has changed.</summary>
            SLR_UPDATE = 0x4,
            /// <summary>Do not update the link information</summary>
            SLR_NOUPDATE = 0x8,
            /// <summary>Do not execute the search heuristics</summary>
            SLR_NOSEARCH = 0x10,
            /// <summary>Do not use distributed link tracking</summary>
            SLR_NOTRACK = 0x20,
            /// <summary>Disable distributed link tracking. By default, distributed link tracking tracks
            /// removable media across multiple devices based on the volume name. It also uses the
            /// Universal Naming Convention (UNC) path to track remote file systems whose drive letter
            /// has changed. Setting SLR_NOLINKINFO disables both types of tracking.</summary>
            SLR_NOLINKINFO = 0x40,
            /// <summary>Call the Microsoft Windows Installer</summary>
            SLR_INVOKE_MSI = 0x80
        }

        public enum CLSCTX
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_ACTIVATE_X86_SERVER = 0x40000,
            CLSCTX_ACTIVATE_32_BIT_SERVER,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_ENABLE_CLOAKING = 0x100000,
            CLSCTX_APPCONTAINER = 0x400000,
            CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,
            CLSCTX_RESERVED6 = 0x1000000,
            CLSCTX_ACTIVATE_ARM32_SERVER = 0x2000000,
            CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION,
            CLSCTX_SERVER_MUST_BE_EQUAL_OR_GREATER_PRIVILEGE,
            CLSCTX_DO_NOT_ELEVATE_SERVER,
            CLSCTX_PS_DLL = -2147483648 //0x80000000
        };

        /// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
        [GeneratedComInterface(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        public unsafe partial interface IShellLinkW
        {
            /// <summary>Retrieves the path and file name of a Shell link object</summary>
            public void GetPath(char* pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
            /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
            public void GetIDList(out IntPtr ppidl);
            /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
            void SetIDList(IntPtr pidl);
            /// <summary>Retrieves the description string for a Shell link object</summary>
            void GetDescription(char* pszName, int cchMaxName);
            /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
            void SetDescription(char* pszName);
            /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
            void GetWorkingDirectory(char* pszDir, int cchMaxPath);
            /// <summary>Sets the name of the working directory for a Shell link object</summary>
            void SetWorkingDirectory(char* pszDir);
            /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
            void GetArguments(char* pszArgs, int cchMaxPath);
            /// <summary>Sets the command-line arguments for a Shell link object</summary>
            void SetArguments(char* pszArgs);
            /// <summary>Retrieves the hot key for a Shell link object</summary>
            void GetHotkey(out short pwHotkey);
            /// <summary>Sets a hot key for a Shell link object</summary>
            void SetHotkey(short wHotkey);
            /// <summary>Retrieves the show command for a Shell link object</summary>
            void GetShowCmd(out int piShowCmd);
            /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
            void SetShowCmd(int iShowCmd);
            /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
            void GetIconLocation(char* pszIconPath,
                int cchIconPath, out int piIcon);
            /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            /// <summary>Sets the relative path to the Shell link object</summary>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
            void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
            /// <summary>Sets the path and file name of a Shell link object</summary>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

        }

        [GeneratedComInterface, Guid("0000010c-0000-0000-c000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public partial interface IPersist
        {
            [PreserveSig]
            void GetClassID(out Guid pClassID);
        }


        [GeneratedComInterface, Guid("0000010b-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe partial interface IPersistFile : IPersist
        {
            //new void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();

            [PreserveSig]
            void Load(char* pszFileName, uint dwMode);

            [PreserveSig]
            void Save(char* pszFileName,
                [MarshalAs(UnmanagedType.Bool)] bool fRemember);

            [PreserveSig]
            int SaveCompleted(char* pszFileName);

            [PreserveSig]
            int GetCurFile(out char* ppszFileName);
        }

        public const uint STGM_READ = 0;
        public const int MAX_PATH = 260;

        [LibraryImport("ole32.dll")]
        public static partial int CoCreateInstance(
            in Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            in Guid riid,
            [MarshalUsing(typeof(ComInterfaceMarshaller<IShellLinkW>))] out IShellLinkW ppv);
    }
}
