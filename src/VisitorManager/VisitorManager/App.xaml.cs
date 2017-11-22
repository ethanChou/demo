using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using NLog;
using VisitorManager.ViewModel;

namespace VisitorManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static EventWaitHandle ProgramStarted;
        [STAThread]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "VSManage", out createNew);
            if (!createNew)
            {
                MessageBox.Show("VSM程序已经启动.");
                Process.GetCurrentProcess().Kill();
            }

                       
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
