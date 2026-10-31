using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Microsoft.Windows.Storage.Pickers;
using StartupWidget.Services.Interfaces;
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

        private FileOpenPicker? picker;

        public OpenFileDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(ownerWindow));
        }

        public Task<bool?> ShowDialogAsync()
        {
            picker = new FileOpenPicker(OwnerWindow.AppWindow.Id)
            {
                Title = Title,
                SuggestedStartFolder = InitialDirectory,
                FileTypeFilter = Filter,
                FileName = FileName,
                FilterIndex = FilterIndex,
                RestoreDirectory = RestoreDirectory,
                CheckFileExists = CheckFileExists,
                CheckPathExists = CheckPathExists
            };
            //IFileDialogをPInvokeしよう






bool? result = picker.ShowDialog(OwnerWindow);
            FileName = picker.FileName;
            return Task.FromResult<bool?>(result);
        }

    }
}
