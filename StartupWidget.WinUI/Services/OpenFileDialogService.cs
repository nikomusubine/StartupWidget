using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Microsoft.Windows.Storage.Pickers;
using StartupWidget.Services.Interfaces;
using StartupWidget.WinUI.Services.Win32Helpers;
using System.Threading.Tasks;
using System.Windows;
using WinRT.Interop;

/*
  No Compilation
 */

namespace StartupWidget.Services
{
    public class OpenFileDialogService : IDialogService
    {
        public string FileName = "";
        public string InitialDirectory = "";
        public string Filter = "";
        public int FilterIndex = 0;
        public string Title = "";
        public bool RestoreDirectory = false;
        public bool CheckFileExists = true;
        public bool CheckPathExists = true;
        public Window OwnerWindow { get; set; }

        public FileOpenDialogHelper? fileOpenDialog;

        public OpenFileDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<bool?> ShowDialogAsync()
        {
            fileOpenDialog = new FileOpenDialogHelper(WindowNative.GetWindowHandle(OwnerWindow))
            {
                Title = Title,
                Directory = InitialDirectory,
                FileType = Filter,
                //FileName = FileName,
                FileTypeIndex = (uint)FilterIndex,
                //RestoreDirectory = RestoreDirectory,
                AllowMultiSelect = false,
                FileMustExist = CheckFileExists,
                PathMustExist = CheckPathExists
            };
            //IFileDialogをPInvokeしよう

            bool? result = fileOpenDialog.ShowDialog();
            FileName = fileOpenDialog.Result;
            return Task.FromResult<bool?>(result);
        }

    }
}
