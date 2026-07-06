using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace StartupWidget.ViewModels.Items
{
    public class ProcessItemViewModel
    {
        public ProcessItemViewModel(IExecutableIconImage getExecutableIcon): this(new ExecutableProcess(), getExecutableIcon) { }
        public ProcessItemViewModel(ProcessItemViewModel old)
        {
            ExecutableProcess = new ExecutableProcess(old.ExecutableProcess);
            getExecutableIcon = old.getExecutableIcon;
            __IconImage = null;
        }

        public ProcessItemViewModel(ExecutableProcess executable, IExecutableIconImage getExecutableIcon)
        {
            ExecutableProcess = executable;
            this.getExecutableIcon = getExecutableIcon;
            __IconImage = null;
        }
        /// <summary>
        /// 追加ボタン用のコンストラクタ。IconImageプロパティを隠蔽すること。
        /// </summary>
        /// <param name="executable"></param>
        protected ProcessItemViewModel(ExecutableProcess executable)
        {
            ExecutableProcess = executable;
            getExecutableIcon = null!;
        }

        private IExecutableIconImage getExecutableIcon;
        private ImageSource? __IconImage;
        public ExecutableProcess ExecutableProcess;
        public virtual ImageSource IconImage
        {
            get
            {
                if (getExecutableIcon == null)
                {
                    throw new InvalidOperationException("""
getExecutableIcon is null. When using the protected constructor, you must override the IconImage property to avoid using getExecutableIcon. 
getExecutableIconがnullです。派生クラスのprotectedコンストラクタを使用した場合、IconImageプロパティを必ずoverrideしてください。
""");
                }
                if (__IconImage == null)
                {
                    __IconImage = getExecutableIcon.GetIconImage(ExecutableProcess);
                }
                return __IconImage;
            }
        }
        public ImageSource? ShieldIcon
        {
            get
            {
                if (ExecutableProcess.RunasAdmin)
                {
                    return ExecutableIconImage.GetShieldIcon();
                }
                else
                {
                    return null;
                }
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
