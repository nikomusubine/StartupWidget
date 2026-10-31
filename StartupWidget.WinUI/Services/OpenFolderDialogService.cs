using Microsoft.Win32;
using StartupWidget.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

/*
  No Compilation
 */

namespace StartupWidget.Services
{
    public class OpenFolderDialogService : IDialogService
    {
        public string FolderName = "";
        public string Title = "";
        public string InitialDirectory = "";
        public bool MultiSelect = false;
        public Window OwnerWindow { get; set; }

        private OpenFolderDialog? dialog;

        public OpenFolderDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<bool?> ShowDialogAsync()
        {
            dialog = new OpenFolderDialog
            {
                FolderName = FolderName,
                Title = Title,
                InitialDirectory = InitialDirectory,
                Multiselect = MultiSelect

            };

            bool? result = dialog.ShowDialog(OwnerWindow);
            FolderName = dialog.FolderName;
            return Task.FromResult<bool?>(result);
        }
    }
}
