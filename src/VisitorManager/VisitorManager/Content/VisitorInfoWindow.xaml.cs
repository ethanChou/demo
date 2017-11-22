
using System.Windows;

using VisitorManager.ViewModel;

namespace VisitorManager.Content
{
    /// <summary>
    /// VisitorInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VisitorInfoWindow : Window, IContextWindow
    {
        public VisitorInfoWindow()
        {
            InitializeComponent();
            this.Close.Click += (o, e) => { this.Close(); };
            this.Confirm.Click += (o, e) => { this.Close(); };
        }
    }
}
