using Microsoft.Win32;
using StartupWidget.Services.Interfaces;
using System.Windows;

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

        private OpenFileDialog? dialog;

        public OpenFileDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<bool?> ShowDialogAsync()
        {
            dialog = new OpenFileDialog
            {
                Title = Title,
                InitialDirectory = InitialDirectory,
                Filter = Filter,
                FileName = FileName,
                FilterIndex = FilterIndex,
                RestoreDirectory = RestoreDirectory,
                CheckFileExists = CheckFileExists,
                CheckPathExists = CheckPathExists
            };

bool? result = dialog.ShowDialog(OwnerWindow);
            FileName = dialog.FileName;
            return Task.FromResult<bool?>(result);
        }

    }
}
