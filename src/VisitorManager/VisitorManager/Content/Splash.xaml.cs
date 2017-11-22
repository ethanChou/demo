using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VisitorManager.ViewModel;

namespace VisitorManager.Content
{
    /// <summary>
    /// Splash.xaml 的交互逻辑
    /// </summary>
    public partial class Splash : UserControl
    {
        public Splash()
        {
            InitializeComponent();
            timer = new Thread(Timer_Tick);
            timer.Start();
            VM = new SplashViewModel();
            this.DataContext = VM;
        }

        /// <summary>
        /// 等待过程，可以执行预处理任务
        /// </summary>
        public Action TaskExcute;
        public Action End;

        Thread timer;
        SplashViewModel VM;
        bool f1, f2, f3;

        private void UpdatePercent(double f)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                VM.Percent = f;
            }));
        }

        private void Timer_Tick()
        {
            double i = 0;
            while (i <= 100)
            {
                i++;
                UpdatePercent(i);
                Thread.Sleep(2);

                if (VM.Percent == 30)
                {
                    f1 = ConnectActivemqServer();
                    if (!f1)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            VM.DetectText += "连接Activemq服务失败,";
                        }));
                    }
                }
                if (VM.Percent == 50)
                {
                    f3 = ConnectThriftServer();
                    if (!f3)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            VM.DetectText += "连接Thrift服务失败,";
                        }));
                    }
                }

                if (VM.Percent == 60)
                {
                    f2 = ConnectCamera();
                    if (!f2)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            VM.DetectText += "未检测到视频摄像头,";
                        }));
                    }
                }

                if (VM.Percent == 100)
                {
                    if (!f1 || !f2 || !f3)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            VM.DetectText += "无法启用软件.";
                        }));
                    }
                    Thread.Sleep(500);
                    if ((f1 && f2 && f3) || LocalConfig.IsForce)
                    {
                        Excute();
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Visibility = Visibility.Collapsed;
                        }));
                    }
                }
            }
        }

        private bool ConnectActivemqServer()
        {
            return FreshCardManager.Start();
        }

        private bool ConnectThriftServer()
        {
            return ThriftManager.Start();
        }

        private bool ConnectCamera()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices != null && videoDevices.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void Excute()
        {
            if (TaskExcute != null) TaskExcute();
        }

        public class DetectInfo
        {

            private string _detectText;

            public string DetectText
            {
                get
                {
                    return _detectText;
                }

                set
                {
                    _detectText = value;
                }
            }
        }

        public class SplashViewModel : ViewModelBase
        {
            private string _detectText;
            private ObservableCollection<DetectInfo> _detectSource = new ObservableCollection<DetectInfo>();
            private double percent;

            public double Percent
            {
                get
                {
                    return percent;
                }

                set
                {
                    percent = value;
                    NotifyChange("Percent");
                }
            }

            public ObservableCollection<DetectInfo> DetectSource
            {
                get
                {
                    return _detectSource;
                }

                set
                {
                    _detectSource = value;
                    NotifyChange("DetectText");

                }
            }

            public string DetectText
            {
                get
                {
                    return _detectText;
                }

                set
                {
                    _detectText = value;
                    NotifyChange("DetectText");

                }
            }
        }
    }
}
