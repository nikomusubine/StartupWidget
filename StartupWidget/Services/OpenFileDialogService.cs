using Microsoft.Win32;
using System.Windows;

namespace StartupWidget.Services
{
    public class OpenFileDialogService
    {
        public string FileName="";
        public string InitialDirectory="";
        public string Filter="";
        public int FilterIndex = 0;
        public string Title = "";
        public bool RestoreDirectory = false;
        public bool CheckFileExists = true;
        public bool CheckPathExists = true;
        public Window OwnerWindow;

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

            bool? result = dialog.ShowDialog();
            FileName = dialog.FileName;
            return Task.FromResult<bool?>(result);
        }

    }
}
