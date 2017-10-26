using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace VisitorManager.App
{
    public class DemoWindow : System.Windows.Window
    {


        public bool ShowMaxButton
        {
            get { return (bool)GetValue(ShowMaxButtonProperty); }
            set { SetValue(ShowMaxButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMaxButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMaxButtonProperty =
            DependencyProperty.Register("ShowMaxButton", typeof(bool), typeof(DemoWindow), new PropertyMetadata(true));



        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMinButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMinButtonProperty =
            DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(DemoWindow), new PropertyMetadata(true));


        public override void OnApplyTemplate()
        {
            Button close = (base.Template.FindName("Close", this) as Button);
            close.Click += close_Click;

            Button max = (base.Template.FindName("Maximize", this) as Button);
            max.Click += max_Click;
            Button restore = (base.Template.FindName("Restore", this) as Button);
            restore.Click += restore_Click;
            if (!ShowMaxButton)
            {
                max.Visibility = System.Windows.Visibility.Collapsed;
                restore.Visibility = System.Windows.Visibility.Collapsed;
            }

            Button min = (base.Template.FindName("Minsize", this) as Button);
            min.Click += min_Click;

            if (!ShowMinButton)
            {
                min.Visibility = System.Windows.Visibility.Collapsed;
            }

            base.OnApplyTemplate();
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
    }
}
