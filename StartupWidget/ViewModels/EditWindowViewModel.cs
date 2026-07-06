using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IWshRuntimeLibrary;
using StartupWidget.Messages;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.Services.Interfaces;
using StartupWidget.ViewModels.Items;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using Application = System.Windows.Application;

namespace StartupWidget.ViewModels
{
    public partial class EditWindowViewModel: ObservableObject, INotifyPropertyChanged
    {
        private SettingsService SettingsService;
        private ProcessViewModel ProcessViewModel;
        public required OpenFileDialogService FileDialogService;
        public required OpenFolderDialogService FolderDialogService;
        public required MessageBoxService MessageBoxService;

        private AddItemButtonViewModel _addButton;
        public AddItemButtonViewModel AddButton=> _addButton;

        public ObservableCollection<ProcessItemViewModel> ProcessItems { get; set; }
        
        private ProcessItemViewModel? _selectedItem;
        public ProcessItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;

                if (!IsCurrentItemValid())
                {
                    HasSelectTried = true;
                    var oldItem = _selectedItem;
                    _selectedItem = value;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _selectedItem = oldItem;
                        OnPropertyChanged(string.Empty);
                    }));
                    return;
                }
                if (value == _addButton)
                {
                    _selectedItem = value;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ProcessItemViewModel newItem = new(ProcessViewModel.GetIconImage);
                        int addButtonIndex = ProcessItems.IndexOf(_addButton);
                        if (addButtonIndex >= 0)
                        {
                            ProcessItems.Insert(addButtonIndex, newItem);
                        }
                        else
                        {
                            ProcessItems.Add(newItem);
                        }
                        SelectedItem = newItem;
                    }));
                    return;
                }
                else
                {
                    _selectedItem = value;
                }

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

        private bool _hasSelectTried = false;
        public bool HasSelectTried
        {
            get=>_hasSelectTried;
            set
            {
                _hasSelectTried = value;
                OnPropertyChanged(nameof(NameErrorVisible));
                OnPropertyChanged(nameof(FilePathErrorVisible));
            }
        }

        #region SelectedItemProperties
        public bool NameErrorVisible => _selectedItem!=null && HasSelectTried && string.IsNullOrEmpty(Name);
        public string Name
        {
            get { if (SelectedItem != null) { return SelectedItem.ExecutableProcess.Name; } else { return ""; }            }
            set { if(SelectedItem!=null) { SelectedItem.ExecutableProcess.Name = value; }  }
        }
        public bool FilePathErrorVisible => _selectedItem != null && HasSelectTried && (string.IsNullOrEmpty(FilePath) || !System.IO.File.Exists(FilePath));
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


        public EditWindowViewModel(SettingsService settings, ProcessViewModel processViewModel) : this(settings, processViewModel, (ProcessItemViewModel?)null)
        {

        }
        public EditWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, string ItemPath) : this(settings, processViewModel, CreateItemFromPath(ItemPath,processViewModel.GetIconImage))
        {


        }

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。'required' 修飾子を追加するか、Null 許容として宣言することを検討してください。
        public EditWindowViewModel(SettingsService settings, ProcessViewModel processViewModel, ProcessItemViewModel? EditingItem)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。'required' 修飾子を追加するか、Null 許容として宣言することを検討してください。
        {
            SettingsService = settings;
            ProcessViewModel = processViewModel;
            _addButton = new AddItemButtonViewModel(LanguageService.GetString("EditWindow/AddButton"));

            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { LoadSettings(); });
            LoadSettings();
            LoadItems(EditingItem);
            SelectedItem = EditingItem;
            ProcessItems!.CollectionChanged += ProcessItems_CollectionChanged;
        }

        [ObservableProperty]
        private float _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily;

        private static ProcessItemViewModel? CreateItemFromPath(string path, IExecutableIconImage executableIconImage)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            ExecutableProcess prcs;
            switch (Path.GetExtension(path))
            {
                case ".exe":
                    prcs = new ExecutableProcess(name, path);
                    return new ProcessItemViewModel(prcs, executableIconImage);
                case ".lnk":
                    WshShell wshShell = new WshShell();
                    IWshShortcut shortcut = wshShell.CreateShortcut(path);
                    path = shortcut.TargetPath;
                    string args = shortcut.Arguments;
                    string workingdir = shortcut.WorkingDirectory;
                    string iconpath = System.IO.File.Exists(shortcut.IconLocation) ? shortcut.IconLocation : string.Empty;
                    prcs = new ExecutableProcess(name, path, args, workingdir, iconpath, false);
                    return new ProcessItemViewModel(prcs, executableIconImage);
                default:
                    return null;
            }
        }
        private void LoadSettings()
        {
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
        }

        private void LoadItems(ProcessItemViewModel? selectedItem)
        {
            ProcessItems = new ObservableCollection<ProcessItemViewModel>();
            bool isItemExists = false;
            foreach (var item in ProcessViewModel.ProcessItems)
            {
                ProcessItemViewModel tmp = new ProcessItemViewModel(item);
                if (item == selectedItem)
                {
                    _selectedItem = tmp;
                    isItemExists = true;
                }
                ProcessItems.Add(tmp);
            }
            if (!isItemExists && selectedItem!=null)
            {
                ProcessItems.Add(selectedItem);
            }
            ProcessItems.Add(_addButton);
        }

        private bool IsCurrentItemValid()
        {
            if (SelectedItem == _addButton) return true;
            if (SelectedItem == null) return true;
            if (string.IsNullOrEmpty(SelectedItem.Name)) return false;
            if (string.IsNullOrWhiteSpace(FilePath)) return false;
            if (!System.IO.File.Exists(FilePath)) return false;

            return true;
        }

        private bool IsCurrentItemEmpty()
        {
            if(SelectedItem == null) return true;
            if (!string.IsNullOrEmpty(Name)) return false;
            if (!string.IsNullOrEmpty(FilePath)) return false;
            if (!string.IsNullOrEmpty(Arguments)) return false;
            if (!string.IsNullOrEmpty(WorkingDirectory)) return false;
            if (!string.IsNullOrEmpty(IconFilePath)) return false;
            return !RunAsAdmin;
        }

        private bool _isAdjusting = false;
        private void ProcessItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isAdjusting) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                int lastIndex = ProcessItems.Count - 1;
                int addButtonIndex = ProcessItems.IndexOf(_addButton);
                if (addButtonIndex >= 0 && addButtonIndex != lastIndex)
                {
                    _isAdjusting = true;
                    ProcessItems.Move(addButtonIndex, lastIndex);
                    _isAdjusting = false;
                    OnPropertyChanged(String.Empty);
                }
            });
        }

        [RelayCommand]
        private async Task BrowseExeFile()
        {
            
            FileDialogService.Title = LanguageService.GetString("Reusable/OpenFileDialogTitle");
            FileDialogService.FileName = FilePath;
            FileDialogService.Filter = LanguageService.GetString("EditWindow/Elements/FilePathDialogFilter")+"(*.exe)|*.exe";
            FileDialogService.RestoreDirectory = false;

            bool? result = await FileDialogService.ShowDialogAsync();
            if ((result != null) ? (bool)result : false)
            {
                FilePath = FileDialogService.FileName;
            }
        }
        [RelayCommand]
        private async Task BrowseWorkingDirectory()
        {
            FolderDialogService.InitialDirectory = WorkingDirectory;
            FolderDialogService.Title = LanguageService.GetString("Reusable/OpenFolderDialogTitle");
            
            bool? result = await FolderDialogService.ShowDialogAsync();
            if ((result != null) ? (bool)result : false)
            {
                string? path = FolderDialogService.FolderName;
                if (path == null) WorkingDirectory = "";
                else WorkingDirectory = path;
            }
        }
        [RelayCommand]
        private async Task BrowseIconFile()
        {
            FileDialogService.Title = LanguageService.GetString("Reusable/OpenFileDialogTitle");
            FileDialogService.FileName = IconFilePath;
            FileDialogService.Filter = LanguageService.GetString("EditWindow/Elements/IconFilePathDialogFilter") + "(*.bmp; *.png; *.jpg; *.jpeg; *.gif; *.tif; *.tiff; *.ico; *.wdp; *.jxr)| " 
                                                      + "*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff;*.ico;*.wdp;*.jxr";
            FileDialogService.RestoreDirectory = true;
            bool? result = await FileDialogService.ShowDialogAsync();
            if ((result != null) ? (bool)result : false)
            {
                IconFilePath = FileDialogService.FileName;
            }
        }

        [RelayCommand]
        private async Task RemoveApp()
        {
            if (SelectedItem == null) return;


            if (IsCurrentItemEmpty() ||
                await MessageBoxService.Show(
                    LanguageService.GetString("EditWindow/MessageBox/ConfirmDelete", Name),
                    LanguageService.GetString("EditWindow/MessageBox/ConfirmDeleteCaption")
                    , MessageBoxDialogButton.YesNo, MessageBoxDialogIcon.Question) == MessageBoxDialogResult.Yes)
            {
                ProcessItemViewModel tmp = _selectedItem!;
                _selectedItem = null;
                ProcessItems.Remove(tmp);
                OnPropertyChanged(string.Empty);
            }
        }
        [RelayCommand]
        private async Task OKButton()
        {
            if (!await Apply()) { return; }
            CancelButton();
        }
        private async Task<bool> Apply()
        {
            if (SelectedItem != null && !IsCurrentItemValid())
            {
                await MessageBoxService.Show(
                    LanguageService.GetString("EditWindow/MessageBox/InvalidItem"),
                    LanguageService.GetString("EditWindow/MessageBox/InvalidItemCaption"),
                    MessageBoxDialogButton.OK, MessageBoxDialogIcon.Stop);
                HasSelectTried = true;
                OnPropertyChanged(string.Empty);
                return false;
            }

            ObservableCollection<ProcessItemViewModel> tempItems = new ObservableCollection<ProcessItemViewModel>();
            foreach (var item in ProcessItems)
            {
                if (item is AddItemButtonViewModel) continue;
                ProcessItemViewModel tmp = new ProcessItemViewModel(item);
                tempItems.Add(tmp);
            }

            ProcessViewModel.ProcessItems = tempItems;
            ProcessViewModel.SaveApplications();
            return true;
        }
        [RelayCommand]
        private async Task ApplyButton()
        {
            await Apply();
        }
        [RelayCommand]
        private void CancelButton()
        {
            WeakReferenceMessenger.Default.Send(new CloseSettingsWindow());
        }
    }
}
