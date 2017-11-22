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
using VisitorManager.Content;
using VisitorManager.ViewModel;
using WPF.Extend;

namespace VisitorManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IContextWindow
    {
        MainWindowViewModel MainVM;
        Rect rcnormal;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;

            this.Splash.TaskExcute += Init;

            Close.Click += close_Click;
            Maximize.Click += max_Click;
            Restore.Click += restore_Click;
            Minsize.Click += min_Click;
        }

        private void Init()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.MainVM = new MainWindowViewModel(this);
                this.MainVM.Init(
                    new UserLeaveViewModel(MainVM),
                    new UserRegisterViewModel(MainVM) { CaptureWindowType = typeof(CaptureWindow),CameraWindowType=typeof(CameraWindow) },
                    new UserVisitingViewModel(MainVM) { WindowType = typeof(VisitorInfoWindow) },
                    new UserSearchViewModel(MainVM) { WindowType = typeof(VisitorInfoWindow) },
                    new UserStatisticViewModel(MainVM));
                visitors.Children.Add(new UserVisiting() { DataContext = MainVM.VistingVM });
                regiseter.Children.Add(new UserRegister() { DataContext = MainVM.RegisterVM });
                exit.Children.Add(new UserLeave() { DataContext = MainVM.LeaveVM });
                statistic.Children.Add(new UserStatistic() { DataContext = MainVM.StatisticVM });
                search.Children.Add(new UserSearch() { DataContext = MainVM.SearchVM });
                this.DataContext = MainVM;
            }));
        }

        private void min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void restore_Click(object sender, RoutedEventArgs e)
        {
            containrGrid.Margin = new Thickness(6);

            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            Maximize.Visibility = Visibility.Visible;
            Restore.Visibility = Visibility.Collapsed;
        }

        private void max_Click(object sender, RoutedEventArgs e)
        {
            containrGrid.Margin = new Thickness(0);
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Left = rc.Left;
            this.Top = rc.Top;
            this.Width = rc.Width;
            this.Height = rc.Height;
            Maximize.Visibility = Visibility.Collapsed;
            Restore.Visibility = Visibility.Visible;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            if (MsgBox.Show("是否退出系统？", "提示", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Process.GetCurrentProcess().Kill();
        }
    }
}
