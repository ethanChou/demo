using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using WPF.Extend;
using PixelFormat = System.Windows.Media.PixelFormat;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace VisitorManager.Content
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureWindow : System.Windows.Window, INotifyPropertyChanged, ICaptureWindow
    {
        private Visibility _defaultVis = Visibility.Visible;
        private Bitmap _currentImage;

        private WriteableBitmapSource _wbSource;

        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        public CaptureWindow()
        {
            InitializeComponent();
            this.Loaded += CaptureWindow_Loaded;
            this.DataContext = this;
        }

        private void CaptureWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this._videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (_videoSource == null && _videoDevices.Count > 0)
                {
                    _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                    VideoCapabilities capabilty = _videoSource.VideoCapabilities[0];

                    if (_wbSource == null)
                    {
                        this._wbSource = new WriteableBitmapSource();
                        if (this._wbSource.SetupSurface(capabilty.FrameSize.Width, capabilty.FrameSize.Height, FrameFormat.RGB24))
                        {
                            this.videoShow.Dispatcher.Invoke(new Action(() =>
                            {
                                this.videoShow.Source = this._wbSource.ImageSource;
                            }));
                        }
                        else
                        {
                            throw new Exception("WriteableBitmapSource不支持该种帧格式：");
                        }
                    }


                    _videoSource.NewFrame += VideoSource_NewFrame;
                    _videoSource.PlayingFinished += VideoSource_PlayingFinished;
                    _videoSource.VideoSourceError += _videoSource_VideoSourceError;
                    _videoSource.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void _videoSource_VideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            Debug.WriteLine(eventArgs.ToString());
        }

        private void VideoSource_PlayingFinished(object sender, ReasonToFinishPlaying reason)
        {
            Debug.WriteLine(reason.ToString());
        }

        private static object _lock = new object();
        private bool _isStop = false;
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs e)
        {
            try
            {

                if (_isStop) return;

                Debug.WriteLine("New Frame");
                if (_defaultVis == Visibility.Visible) _defaultVis = Visibility.Collapsed;


                lock (_lock)
                {
                    if (_currentImage != null) _currentImage.Dispose();
                    _currentImage = (Bitmap)e.Frame.Clone();
                }

                BitmapData data = e.Frame.LockBits(new System.Drawing.Rectangle(0, 0, e.Frame.Width, e.Frame.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


                this._wbSource.Render(data.Scan0);


                e.Frame.UnlockBits(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("NewFrame:" + ex.ToString());
            }
            Debug.WriteLine("New Frame End");
        }

        private ICommand _closeCmd;

        public ICommand CloseCmd
        {
            get { return _closeCmd ?? (_closeCmd = new DelegateCommand(CloseCommand)); }
        }

        private void CloseCommand(object arg)
        {
            _isStop = true;
            this.videoShow.Source = null;

            _videoSource.Stop();

            this.DialogResult = false;

            if (_currentImage != null) _currentImage.Dispose();
            this.Close();
        }

        private ICommand _captureCmd;

        public ICommand CaptureCmd
        {
            get { return _captureCmd ?? (_captureCmd = new DelegateCommand(CaptureCommand)); }
        }

        private string _captureImageSrc = "";
        private void CaptureCommand(object arg)
        {
            lock (_lock)
            {
                if (_currentImage != null)
                {
                    string tmpsrc = string.Format("{0}\\VisitorLib\\capture\\{1}.jpg", "D:", DateTime.Now.ToFileTime());
                    string v = Path.GetDirectoryName(tmpsrc);
                    if (!Directory.Exists(v))
                    {
                        Directory.CreateDirectory(v);
                    }
                    _currentImage.Save(tmpsrc, ImageFormat.Jpeg);
                    CaptureImageSrc = tmpsrc;
                }

            }
        }

        private ICommand _cancelCmd;

        public ICommand CancelCmd
        {
            get { return _cancelCmd ?? (_cancelCmd = new DelegateCommand(CancelCommand)); }
        }


        private void CancelCommand(object arg)
        {
            CloseCommand(arg);
        }

        private ICommand _confirmCmd;

        public ICommand ConfirmCmd
        {
            get { return _confirmCmd ?? (_confirmCmd = new DelegateCommand(ConfirmCommand)); }
        }

        public Bitmap CurrentImage
        {
            get { return _currentImage; }
        }

        public string CaptureImageSrc
        {
            get { return _captureImageSrc; }
            set
            {
                _captureImageSrc = value;
                NotifyChange("CaptureImageSrc");
            }
        }

        private void ConfirmCommand(object arg)
        {
            _isStop = true;
            this.videoShow.Source = null;

            _videoSource.Stop();

            this.DialogResult = true;
            if (_currentImage != null) _currentImage.Dispose();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 封装PropertyChanged触发方法，触发指定属性
        /// </summary>
        /// <param name="propertyName">通知的属性名</param>
        public virtual void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string CaptureImagePath
        {
            get { return CaptureImageSrc; }
        }
    }
}
