using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;

namespace WPF.Extend
{
    public sealed class MsgBox
    {

        public static MessageBoxResult Show(bool topMost, bool isShowCheckbox, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return Show(null, topMost, isShowCheckbox, messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(bool isShowCheckbox, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return Show(null, false, isShowCheckbox, messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(bool isShowCheckbox, string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(null, false, isShowCheckbox, messageBoxText, caption, button, MessageBoxImage.Information, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return Show(false, false, messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon)
        {
            return Show(messageBoxText, caption, button, icon, MessageBoxResult.None);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(messageBoxText, caption, button, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(messageBoxText, Strings.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return Show(owner, false, false, messageBoxText, caption, button, icon, defaultResult);
        }

        private static MessageBoxResult Show(Window owner, bool topMost, bool isShowCheckbox, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            MsgBoxCore msg = new MsgBoxCore();

            if (owner != null)
            {
                msg.Owner = owner;
            }
            else
            {
                if (Application.Current.MainWindow != null)
                    msg.Owner = Application.Current.MainWindow;
            }

            MsgBoxCoreViewModel vm = new MsgBoxCoreViewModel(msg);
            vm.Init(topMost, isShowCheckbox, messageBoxText, caption, button, icon, defaultResult);
            msg.DataContext = vm;
            if (msg.Dispatcher.CheckAccess())
            {
                msg.ShowDialog();
            }
            else
            {
                msg.Dispatcher.Invoke(new Action(() =>
                {
                    msg.ShowDialog();
                }));
            }
            return msg.Result;
        }
    }
}
