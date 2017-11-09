using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VisitorManager.ViewModel
{
    public interface IDispatcher
    {
        Dispatcher Dispatcher { get; }
    }


    public interface IWindowBase : IDispatcher
    {
        bool? DialogResult { get; set; }

        Window Owner { get; set; }

        void Hide();

        void Show();

        bool? ShowDialog();
    }

    public interface IContextWindow : IWindowBase
    {
        object DataContext { get; set; }
    }

    public interface INotifyWindow : IContextWindow, INotifyPropertyChanged
    {

    }

    public interface ICaptureWindow : IContextWindow
    {
        string CaptureImagePath { get; }
    }
}
