using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StartupWidget.Messages;
using StartupWidget.Models.Settings;
using StartupWidget.Models.Types;
using StartupWidget.Services;
using StartupWidget.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace StartupWidget.ViewModels
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        public SettingsWindowViewModel(SettingsService settings, OpenFileDialogService fileDialog, ColorPickerDialogService colorPicker)
        {
            SettingsService = settings;
            GeneralSettingsDraft = SettingsService.GeneralSettings;
            MainWindowSettingsDraft = SettingsService.MainWindowSettings;
            WidgetSettingsDraft = SettingsService.WidgetSettings;
            FileDialogService = fileDialog;
            ColorPickerService = colorPicker;
            WeakReferenceMessenger.Default.Register<ApplySettings>(this, (r, m) => { LoadSettings(); });
            LoadSettings();
            LoadItems();
        }
        private OpenFileDialogService FileDialogService;
        private ColorPickerDialogService ColorPickerService;
        private SettingsService SettingsService { get; set; }

        private GeneralSettings GeneralSettingsDraft;
        
        [ObservableProperty]
        private ObservableCollection<SettingItemViewModel> _generalSettingsItems;
        private MainWindowSettings MainWindowSettingsDraft;
        [ObservableProperty]
        private ObservableCollection<SettingItemViewModel> _mainWindowSettingsItems;
        private WidgetSettings WidgetSettingsDraft;
        [ObservableProperty]
        private ObservableCollection<SettingItemViewModel> _widgetSettingsItems;
        [ObservableProperty]
        private double _fontSize;
        [ObservableProperty]
        private System.Windows.Media.FontFamily _fontFamily;


        private void LoadSettings()
        {
            FontSize = SettingsService.GeneralSettings.Font.Size;
            FontFamily = new System.Windows.Media.FontFamily(SettingsService.GeneralSettings.Font.Name);
        }


        private void LoadItems(ref ObservableCollection<SettingItemViewModel> settingItems, object SettingsModel)
        {
            settingItems = new ObservableCollection<SettingItemViewModel>();
            string LangPath = "SettingsWindow/";

            if (SettingsModel.GetType() == typeof(GeneralSettings)) LangPath += "GeneralSettings/";
            if (SettingsModel.GetType() == typeof(MainWindowSettings)) LangPath += "MainWindowSettings/";
            if (SettingsModel.GetType() == typeof(WidgetSettings)) LangPath += "WidgetSettings/";


            foreach (var property in SettingsModel.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<Attributes.SettingElementIgnoreAttribute>() != null) continue;

                var doubleAttr = property.GetCustomAttribute(typeof(Attributes.DoubleBoundsAttribute));
                var intAttr = property.GetCustomAttribute(typeof(Attributes.IntBoundsAttribute));
                var strAttr = property.GetCustomAttribute(typeof(Attributes.StringOptionAttribute));
                var langAttr = property.GetCustomAttribute(typeof(Attributes.LanguageElementAttribute));

                SettingItemViewModel settingItem;
                if (doubleAttr != null)
                {
                    List<double> l = new List<double>();
                    l.Add(((Attributes.DoubleBoundsAttribute)doubleAttr).Min);
                    l.Add(((Attributes.DoubleBoundsAttribute)doubleAttr).Max);
                    settingItem = new LimitedSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, property.GetValue(SettingsModel), l.Cast<object>().ToList());
                }
                else if (intAttr != null)
                {
                    List<int> l = new List<int>();
                    l.Add(((Attributes.IntBoundsAttribute)intAttr).Min);
                    l.Add(((Attributes.IntBoundsAttribute)intAttr).Max);
                    settingItem = new LimitedSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, property.GetValue(SettingsModel), l.Cast<object>().ToList());
                }
                else if (strAttr != null)
                {

                    if (property.Name.EndsWith("FilePath") && property.PropertyType == typeof(string))
                    {
                        settingItem = new FilePathSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, property.GetValue(SettingsModel), ((Attributes.StringOptionAttribute)strAttr).Options.Cast<object>()) { 
                        DialogService=this.FileDialogService
                        };
                        
                    }
                    else
                    {
                        settingItem = new LimitedSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, property.GetValue(SettingsModel), ((Attributes.StringOptionAttribute)strAttr).Options.Cast<object>());
                    }
                }else if (langAttr != null)
                {
                    settingItem = new LimitedSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, LanguageService.GetNativeName( (string)property.GetValue(SettingsModel)), LanguageService.GetAvailableLanguages());
                }
                else
                {
                    if (property.PropertyType == typeof(SerializableFont))
                    {
                        System.Drawing.Text.InstalledFontCollection fontCollection = new System.Drawing.Text.InstalledFontCollection();
                        settingItem = new FontSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, ((SerializableFont)property.GetValue(SettingsModel)).Name, fontCollection.Families.Select(x => x.Name).ToList(), ((SerializableFont)property.GetValue(SettingsModel)).Size);
                    }
                    else if (property.PropertyType == typeof(Models.Types.Color))
                    {
                        settingItem = new ColorSettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, (Color)property.GetValue(SettingsModel))
                        {
                            ColorDialog = ColorPickerService
                        };

                    }
                    else
                    {
                        settingItem = new SettingItemViewModel(property.Name, LanguageService.GetString(LangPath + property.Name), property.PropertyType, property.GetValue(SettingsModel));
                    }
                }
                settingItems.Add(settingItem);
            }
        }

        private void LoadItems()
        {
            LoadItems(ref _generalSettingsItems, GeneralSettingsDraft);
            LoadItems(ref _mainWindowSettingsItems, MainWindowSettingsDraft);
            LoadItems(ref _widgetSettingsItems, WidgetSettingsDraft);
        }

        private void _ApplyItems(ObservableCollection<SettingItemViewModel> settingItems, object SettingsModel)
        {
            foreach (var property in SettingsModel.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<Attributes.SettingElementIgnoreAttribute>() != null) continue;
                foreach (var item in settingItems)
                {
                    if (item.Id == property.Name)
                    {
                        if (property.PropertyType == typeof(SerializableFont))
                        {
                            property.SetValue(SettingsModel, new SerializableFont(item.Value.ToString(), ((FontSettingItemViewModel)item).Size));
                        }
                        else if (property.PropertyType == typeof(Color))
                        {
                            property.SetValue(SettingsModel, new Color((Color)item.Value));
                        }
                        else
                        {
                            if (property.GetCustomAttribute<Attributes.LanguageElementAttribute>() != null)
                            {
                                property.SetValue(SettingsModel, LanguageService.GetCultureName((string)item.Value));
                            }
                            else
                            {
                                property.SetValue(SettingsModel, item.Value);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private bool IsChanged()
        {
            bool changed = false;
            changed = _generalSettingsItems.Any(x => x.isChanged);
            if (changed) { return true; }
            changed = _mainWindowSettingsItems.Any(x => x.isChanged);
            if (changed) { return true; }
            changed = _widgetSettingsItems.Any(x => x.isChanged);
            return changed;
        }

        [RelayCommand]
        private void OKSettings()
        {
            ApplySettings();
            CancelSettings();
        }
        [RelayCommand]
        private void ApplySettings()
        {
            _ApplyItems(_generalSettingsItems, GeneralSettingsDraft);
            _ApplyItems(_mainWindowSettingsItems, MainWindowSettingsDraft);
            _ApplyItems(_widgetSettingsItems, WidgetSettingsDraft);
            WeakReferenceMessenger.Default.Send(new ApplySettings());
            SettingsService.UpdateSettings(GeneralSettingsDraft, MainWindowSettingsDraft, WidgetSettingsDraft);
        }
        [RelayCommand]
        private void CancelSettings()
        {
            WeakReferenceMessenger.Default.Send(new CloseSettingsWindow());
        }
        [RelayCommand]
        private void EscapeKey()
        {
            if (!IsChanged())
            {
                CancelSettings();
            }
        }
    }
}
