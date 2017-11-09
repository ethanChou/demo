using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WPF.Extend
{
    /// <summary>
    /// CMessageBoxWindow.xaml 的交互逻辑
    /// </summary>
    partial class MsgBoxCore : WindowBase, IMsgWindow
    {
        internal MsgBoxCore()
        {
            InitializeComponent();
        }

        public MessageBoxResult Result { get; set; }

        public bool IsChecked { get; set; }
    }
}
