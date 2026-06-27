using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Services;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using static System.Windows.Forms.Design.AxImporter;
using MessageBox = System.Windows.MessageBox;

namespace StartupWidget.ViewModels
{
    public partial class EditWindowViewModel: ObservableObject, INotifyPropertyChanged
    {
        private SettingsService SettingsService;
        private ProcessViewModel ProcessViewModel;
        public ObservableCollection<ProcessItemViewModel> ProcessItems { get; set; }
        
        private ProcessItemViewModel? _selectedItem;
        public ProcessItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(string.Empty);
            }
        }

        public bool IsSelected
        {
            get
            {
                return _selectedItem != null;
            }
        }

        #region SelectedItemProperties
        public string Name
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.Name; } else { return ""; }            }
            set { if(SelectedItem!=null) { SelectedItem.ExecutableProcess.Name = value; }  }
        }
        public string FilePath
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.FilePath; } else { return ""; } }
            set { if (SelectedItem != null) { SelectedItem.ExecutableProcess.FilePath = value;
                    OnPropertyChanged();
                } }
        }
        public string Arguments
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.Arguments; } else { return ""; } }
            set { if (SelectedItem != null) { SelectedItem.ExecutableProcess.Arguments = value; } }
        }
        public string WorkingDirectory
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.WorkingDirectory; } else { return ""; } }
            set { if (SelectedItem != null) { SelectedItem.ExecutableProcess.WorkingDirectory = value;
                    OnPropertyChanged();
                } }
        }
        public string IconFilePath
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.IconFilePath; } else { return ""; } }
            set { if (SelectedItem != null) { SelectedItem.ExecutableProcess.IconFilePath = value;
                    OnPropertyChanged();
                } }
        }
        public bool RunAsAdmin
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.RunasAdmin; } else { return false; } }
            set { if (SelectedItem != null) { SelectedItem.ExecutableProcess.RunasAdmin = value; } }
        }
        #endregion


        public EditWindowViewModel(SettingsService settings, ProcessViewModel processViewModel)
        {
            SettingsService = settings;
            ProcessViewModel = processViewModel;
            ProcessItems = new ObservableCollection<ProcessItemViewModel>(ProcessViewModel.ProcessItems);

            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { LoadSettings(); });
            LoadSettings();
        }

        public EditWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, ProcessItemViewModel EditingItem) : this(settings, processViewModel)
        {
            SelectedItem = EditingItem;
        }


        [ObservableProperty]
        private float _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily;


        private void LoadSettings()
        {
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
        }


        [RelayCommand]
        private void BrowseExeFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = LanguageService.GetString("Reusable/OpenFileDialogTitle");
            dialog.FileName = FilePath;
            dialog.Filter = LanguageService.GetString("EditWindow/Elements/FilePathDialogFilter")+"(*.exe)|*.exe";
            dialog.RestoreDirectory = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = dialog.FileName;
            }
            dialog.Dispose();
        }
        [RelayCommand]
        private void BrowseWorkingDirectory()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = dialog.Title = LanguageService.GetString("Reusable/OpenFolderDialogTitle");
            ;
            dialog.Filter = LanguageService.GetString("EditWindow/Elements/WorkingDirectoryDialogFilter") + "|.";
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string? path = Path.GetDirectoryName(dialog.FileName);
                if (path == null) FilePath = "";
                else WorkingDirectory = path;
            }
            dialog.Dispose();
        }
        [RelayCommand]
        private void BrowseIconFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = LanguageService.GetString("Reusable/OpenFileDialogTitle");
            dialog.FileName = IconFilePath;
            dialog.Filter = LanguageService.GetString("EditWindow/Elements/IconFilePathDialogFilter") + "(*.bmp; *.png; *.jpg; *.jpeg; *.gif; *.tif; *.tiff; *.ico; *.wdp; *.jxr)| " 
                                                      + "*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr";
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                IconFilePath = dialog.FileName;
            }
            dialog.Dispose();
        }

        [RelayCommand]
        private void RemoveApp()
        {
            if (SelectedItem == null) return;
            if (MessageBox.Show(LanguageService.GetString("EditWindow/MessageBox/ConfirmDelete", Name),
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProcessItems.Remove(SelectedItem);
            }
        }
        [RelayCommand]
        private void OKButton()
        {
            ApplyButton();
            CancelButton();
        }
        [RelayCommand]
        private void ApplyButton()
        {
            ProcessViewModel.ProcessItems= ProcessItems;
            ProcessItems = new ObservableCollection<ProcessItemViewModel>(ProcessViewModel.ProcessItems);
            ProcessViewModel.SaveApplications();
        }
        [RelayCommand]
        private void CancelButton()
        {
            WeakReferenceMessenger.Default.Send(new CloseSettingsWindow());
        }
    }
}
