using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Control = System.Windows.Controls.Control;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace StartupWidget.Behaviours
{
    public class FileDropBehaviour:Behavior<Control>
    {
        public static readonly DependencyProperty FileDropCommandProperty =
            DependencyProperty.RegisterAttached(
                "FileDropCommand",
                typeof(ICommand),
                typeof(FileDropBehaviour),
                new PropertyMetadata(null, OnFileDropCommandChanged));
        public static ICommand GetFileDropCommand(DependencyObject obj) => (ICommand)obj.GetValue(FileDropCommandProperty);
        public static void SetFileDropCommand(DependencyObject obj, ICommand value) => obj.SetValue(FileDropCommandProperty, value);

        private static void OnFileDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ( d is UIElement element)
            {
                element.DragOver -= Element_DragOver;
                element.Drop -= Element_Drop;
                if(e.NewValue is ICommand)
                {
                    element.AllowDrop= true;
                    element.DragOver += Element_DragOver;
                    element.Drop += Element_Drop; 
                }
            }
        }

        private static void Element_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private static void Element_Drop(object sender,DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(files!=null && files.Length > 0)
                {
                    var command = GetFileDropCommand((DependencyObject)sender);
                    if(command != null && command.CanExecute(files[0]))
                    {
                        command.Execute(files[0]);
                    }
                }
            }
        }
    }
}
