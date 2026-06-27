using StartupWidget.Models.Types;
using StartupWidget.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace StartupWidget.ViewModels.Items
{
    public class ProcessItemViewModel
    {
        public ProcessItemViewModel(ExecutableProcess executable, IExecutableIconImage getExecutableIcon)
        {
            ExecutableProcess = executable;
            this.getExecutableIcon = getExecutableIcon;
            __IconImage = null;
        }
        private IExecutableIconImage getExecutableIcon;
        private ImageSource? __IconImage;
        public ExecutableProcess ExecutableProcess;
        public ImageSource IconImage
        {
            get
            {
                if (__IconImage == null)
                {
                    __IconImage= getExecutableIcon.GetIconImage(ExecutableProcess);
                }
                return __IconImage;
            }
        }
        public string Name
        {
            get
            {
                return ExecutableProcess.Name;
            }
        }
    }
}
