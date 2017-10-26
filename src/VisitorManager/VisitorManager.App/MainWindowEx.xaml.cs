using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VisitorManager.Common;

namespace VisitorManager.App
{
    /// <summary>
    /// MainWindowEx.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowEx : DemoWindow
    {
        public MainWindowEx()
        {
            InitializeComponent();
            DbUtil.Init(DbUtil.DB_PATH);

            this.Loaded += MainWindow_Loaded;

            this.DataContext = new MainWindowViewModel();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow = this;
            //Screen s = Screen.PrimaryScreen;
            //this.Left = s.WorkingArea.Left;
            //this.Top = s.WorkingArea.Top;
            //this.Width = s.WorkingArea.Width;
            //this.Height = s.WorkingArea.Height;
           
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Process.GetCurrentProcess().Kill();
        }
    }
}
