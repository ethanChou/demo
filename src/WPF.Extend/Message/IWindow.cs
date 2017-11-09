using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace WPF.Extend
{
    internal interface IMsgWindow
    {
        bool? DialogResult { get; set; }

        MessageBoxResult Result { get; set; }

        Dispatcher Dispatcher { get; }

        bool IsChecked { get; set; }

        void Close();
    }
}
