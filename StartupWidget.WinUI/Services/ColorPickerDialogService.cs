using StartupWidget.Models.Types;
using StartupWidget.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace StartupWidget.Services
{
    public class ColorPickerDialogService:IDialogService
    {
        public Window OwnerWindow { get; set; }
        public Color Color;

        public ColorPickerDialogService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<bool?> ShowDialogAsync()
        {
            bool result=false;
            ColorDialog dialog = new ColorDialog(); //WinFormsのColorDialogは、WPFで使用不可。WinUI3移行時にWinUI3のColorPickerに置き換え。
            dialog.Color = System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
            
            DialogResult dResult = dialog.ShowDialog();

            if (dResult == DialogResult.OK)
            {
                result = true;
                Color = Color.FromArgb(dialog.Color.ToArgb());
            }
            dialog.Dispose();
            return Task.FromResult<bool?>(result);
        }
    }
}
