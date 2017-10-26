using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisitorManager.ViewModel.Common;

namespace VisitorManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Close.Click += close_Click;

            Maximize.Click += max_Click;

            Restore.Click += restore_Click;


            Minsize.Click += min_Click;

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

        private void min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void restore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;

        }
        Rect rcnormal;
        private void max_Click(object sender, RoutedEventArgs e)
        {


            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小


            this.WindowState = System.Windows.WindowState.Maximized;

            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Left = rc.Left;
            this.Top = rc.Top;
            this.Width = rc.Width;
            this.Height = rc.Height;

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Process.GetCurrentProcess().Kill();
        }
    }
}
