using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VisitorManager
{
    public interface IDispatcher
    {
        Dispatcher Dispatcher { get; set; }
    }

    public interface IWindowBase
    {
        bool? DialogResult { get; set; }

        Dispatcher Dispatcher { get; }

        Window Owner { get; set; }

        void Hide();

        void Show();

        bool? ShowDialog();
    }

    public interface ICaptureWindow : IWindowBase
    {
        string CaptureImagePath { get; }
    }
}
