using StartupWidget.Models.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace StartupWidget.Services.Interfaces
{
    public interface IExecutableIconImage
    {
        ImageSource GetIconImage(ExecutableProcess process);
        ImageSource GetShieldIconImage();
    }
}
