using StartupWidget.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace StartupWidget.Services
{
    public class MessageBoxService
    {
        Window OwnerWindow;

        public MessageBoxService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public MessageBoxResult Show(string messageBoxText,string caption,MessageBoxDialogButton button,MessageBoxDialogIcon icon)
        {
            MessageBoxButton wpfButton;
            MessageBoxImage wpfImage;

            switch (button)
            {
                case MessageBoxDialogButton.OKCancel:
                    wpfButton = MessageBoxButton.OKCancel;
                    break;
                case MessageBoxDialogButton.YesNo:
                    wpfButton = MessageBoxButton.YesNo;
                    break;
                case MessageBoxDialogButton.YesNoCancel:
                    wpfButton = MessageBoxButton.YesNoCancel;
                    break;
                case MessageBoxDialogButton.OK:
                default:
                    wpfButton = MessageBoxButton.OK;
                    break;
            }

            switch (icon)
            {
                case MessageBoxDialogIcon.Information:
                    wpfImage = MessageBoxImage.Information;
                    break;
                case MessageBoxDialogIcon.Stop:
                    wpfImage = MessageBoxImage.Stop;
                    break;
                case MessageBoxDialogIcon.Question:
                    wpfImage = MessageBoxImage.Question;
                    break;
                case MessageBoxDialogIcon.Exclamation:
                    wpfImage = MessageBoxImage.Exclamation;
                    break;
                case MessageBoxDialogIcon.None:
                default:
                    wpfImage = MessageBoxImage.None;
                    break;
            }
            
            return MessageBox.Show(messageBoxText, caption, wpfButton, wpfImage);
        }
    }

    public enum MessageBoxDialogResult
    {
        None,
        OK, Cancel,
        Yes, No
    }

    public enum MessageBoxDialogButton
    {
        OK, OKCancel,
        YesNo, YesNoCancel
    }

    public enum MessageBoxDialogIcon
    {
        None, Information, Exclamation, Stop, Question
    }
}
