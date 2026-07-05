using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace StartupWidget.Services.Interfaces
{
    public interface IDialogService
    {
        Window OwnerWindow { get; set; }

        public Task<bool?> ShowDialogAsync();
    }
}
