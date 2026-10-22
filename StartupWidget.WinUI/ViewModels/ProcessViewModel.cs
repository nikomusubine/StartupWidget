using CommunityToolkit.Mvvm.ComponentModel;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace StartupWidget.ViewModels
{
    public class ProcessViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ProcessViewModel()
        {
            executableProcessService = new ExecutableProcessService();
            executableProcessService.LoadApplications();
            _processItems = new ObservableCollection<ProcessItemViewModel>();
            getIconImage = new ExecutableIconImage();
            __ReflectProcessItems();

        }

        private void __ReflectProcessItems()
        {
            _processItems.Clear();
            foreach(var i in executableProcessService.ExecutableProcesses)
            {
                _processItems.Add(new ProcessItemViewModel(i, getIconImage));
            }
        }

        public void AddProcess(ExecutableProcess process)
        {
            executableProcessService.AddProcess(process);
            __ReflectProcessItems();
        }
        public void SaveApplications()
        {
            executableProcessService.SaveApplications();
        }
        public void StartProcess(ExecutableProcess process)
        {
            this.StartProcess(process, false);
        }
        public void StartProcess(ExecutableProcess process, bool RunAsAdmin)
        {
            try
            {
                executableProcessService.ProcessStart(process, RunAsAdmin);
            }
            catch (Win32Exception exc)
            {
                if (exc.NativeErrorCode == 740)
                {
                    process.RunasAdmin = true;
                    StartProcess(process, true);
                    __ReflectProcessItems();
                }
                else
                {
                    throw;
                }
            }
        }
        private ExecutableIconImage getIconImage;
        public ExecutableIconImage GetIconImage { get => getIconImage; }
        private ExecutableProcessService executableProcessService;
        private ObservableCollection<ProcessItemViewModel> _processItems;
        public ObservableCollection<ProcessItemViewModel> ProcessItems
        {
            get => _processItems;
            set
            {
                executableProcessService.ExecutableProcesses.Clear();
                foreach (var i in value)
                {
                    executableProcessService.ExecutableProcesses.Add(i.ExecutableProcess);
                }
                __ReflectProcessItems();
                OnPropertyChanged();
            }
        }
    }
}
