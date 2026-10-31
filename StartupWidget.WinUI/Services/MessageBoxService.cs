using StartupWidget.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

/*
  No Compilation
ContentDialogを使用する。DialogIconは不要になる
 */

namespace StartupWidget.Services
{
    public class MessageBoxService
    {
        Window OwnerWindow;

        public MessageBoxService(Window ownerWindow)
        {
            OwnerWindow = ownerWindow;
        }

        public Task<MessageBoxDialogResult> Show(string messageBoxText, string caption, MessageBoxDialogButton button, MessageBoxDialogIcon icon)
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

            return Task.FromResult<MessageBoxDialogResult>(ConvertResult(MessageBox.Show(OwnerWindow, messageBoxText, caption, wpfButton, wpfImage)));
        }

        private static MessageBoxDialogResult ConvertResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.OK:
                    return MessageBoxDialogResult.OK;
                case MessageBoxResult.Cancel:
                    return MessageBoxDialogResult.Cancel;
                case MessageBoxResult.Abort:
                    return MessageBoxDialogResult.Abort;
                case MessageBoxResult.Retry:
                    return MessageBoxDialogResult.Retry;
                case MessageBoxResult.Ignore:
                    return MessageBoxDialogResult.Ignore;
                case MessageBoxResult.Yes:
                    return MessageBoxDialogResult.Yes;
                case MessageBoxResult.No:
                    return MessageBoxDialogResult.No;
                case MessageBoxResult.TryAgain:
                    return MessageBoxDialogResult.TryAgain;
                case MessageBoxResult.Continue:
                    return MessageBoxDialogResult.Continue;
                case MessageBoxResult.None:
                default:
                    return MessageBoxDialogResult.None;
            }
        }
    }

    public enum MessageBoxDialogResult
    {
        None = 0,
        OK = 1, Cancel =2,
        Abort=3,
        Retry = 4,
        Ignore = 5,
        Yes=6, No =7,
        TryAgain = 10,
        Continue=11,
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
