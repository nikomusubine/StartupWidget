using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using StartupWidget.WinUI.Views.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace StartupWidget.WinUI.Views.TemplateBuilders
{
    internal class FilePathTextBox : Grid
    {
        private readonly TextBox _textBox;
        private readonly Button _browseButton;

        public string FilePath
        {
            get
            {
                return _textBox.Text;
            }
            set
            {
                _textBox.Text = value ?? string.Empty;
            }
        }

        public event EventHandler<string>? FilePathChanged;

        public List<string> FilterExtensions { get; }

        public FilePathTextBox()
        {
            FilterExtensions = new List<string>();
            _textBox = new TextBox();
            _browseButton = new Button();

            SetupTextBox();
            SetupButton();


            ColumnDefinitions.Add(RowColumnDefinitionHelper.ColumnDefinitionStar(1));
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            this.Children.Add(_textBox);
            this.Children.Add(_browseButton);
        }

        void SetupTextBox()
        {
            _textBox.TextChanged += (s, e) => FilePathChanged?.Invoke(this, _textBox.Text);
            SetColumn(_textBox, 0);
        }
        void SetupButton()
        {
            _browseButton.Content = "...";
            _browseButton.Click += OnBrowseButtonClick;
            SetColumn(_browseButton, 1);
        }


        private async void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element && element.XamlRoot != null)
            {
                WindowId windowid = element.XamlRoot.ContentIslandEnvironment.AppWindowId;

                FileOpenPicker picker = new FileOpenPicker(windowid);
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    _textBox.Text=file.Path;
                }
            }
        }
    }
}
