using Microsoft.UI.Xaml;
using StartupWidget.Services.Interfaces;
using StartupWidget.WinUI.Services.Win32Helpers;
using System.Threading.Tasks;
using WinRT.Interop;

/*
  No Compilation
 */

namespace StartupWidget.Services
{
    public class OpenFolderDialogService : IDialogService
    {
        public string FolderName = "";
        public string[]? FolderNames;
        public string Title = "";
        public string InitialDirectory = "";
        public bool MultiSelect = false;
        public Window OwnerWindow { get; set; }

        public FileOpenDialogHelper? fileOpenDialog;

        public OpenFolderDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<bool?> ShowDialogAsync()
        {
            fileOpenDialog = new FileOpenDialogHelper(WindowNative.GetWindowHandle(OwnerWindow))
            {
                Title = Title,
                Directory = InitialDirectory,
                PickFolders = true,
                AllowMultiSelect = MultiSelect,
                PathMustExist = true
            };

            bool? result = fileOpenDialog.ShowDialog();
            FolderName = fileOpenDialog.Result;
            FolderNames = fileOpenDialog.Results;

            return Task.FromResult<bool?>(result);
        }
    }
}
