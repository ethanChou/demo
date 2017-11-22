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
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using WPF.Extend;
using PixelFormat = System.Windows.Media.PixelFormat;
using Rectangle = System.Windows.Shapes.Rectangle;
using VisitorManager.ViewModel;

namespace VisitorManager.Content
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureWindow : System.Windows.Window, INotifyPropertyChanged, ICaptureWindow
    {
        private Visibility _defaultVis = Visibility.Visible;
        private Bitmap _currentImage;
        private ICommand _addFileCmd;
        private WriteableBitmapSource _wbSource;
        private DispatcherTimer _drawTimer;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;
        private VideoCapabilities capabilty;
        private bool _isChecked = true;
        public CaptureWindow()
        {
            InitializeComponent();

            if (LocalConfig.IsAddLocalFile)
            {
                addLocalFile.Visibility = System.Windows.Visibility.Visible;
            }

            this.Loaded += CaptureWindow_Loaded;
            this.DataContext = this;
        }

        private bool _isEnabledButton = false;

        private void CaptureWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _drawTimer = new DispatcherTimer();
                _drawTimer.Interval = new TimeSpan(0, 0, 0, 0, 60);
                _drawTimer.Tick += _drawTimer_Tick;

                this._videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (_videoSource == null && _videoDevices.Count > 0)
                {
                    _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                    capabilty = _videoSource.VideoCapabilities[0];

                    if (_wbSource == null)
                    {
                        this._wbSource = new WriteableBitmapSource();
                        if (this._wbSource.SetupSurface(capabilty.FrameSize.Width, capabilty.FrameSize.Height, FrameFormat.RGB24))
                        {
                            this.videoShow.Source = this._wbSource.ImageSource;
                        }
                        else
                        {
                            MsgBox.Show("WriteableBitmapSource不支持该种帧格式.");
                        }
                    }

                    _videoSource.NewFrame += VideoSource_NewFrame;
                    _videoSource.PlayingFinished += VideoSource_PlayingFinished;
                    _videoSource.VideoSourceError += _videoSource_VideoSourceError;
                    _videoSource.Start();
                    _drawTimer.Start();
                    IsEnabledButton = true;
                }
                else
                {
                    this.Title += "-未发现摄像头";
                    IsChecked = false;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void _drawTimer_Tick(object sender, EventArgs e)
        {
            if (_currentImage == null) return;

            lock (_lock)
            {
                if (IsChecked) IsChecked = false;
                BitmapData data = _currentImage.LockBits(new System.Drawing.Rectangle(0, 0, _currentImage.Width, _currentImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                if (_wbSource != null)
                    this._wbSource.Render(data.Scan0);
                _currentImage.UnlockBits(data);
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

        bool _isCapture = false;
        private static object _lock = new object();
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs e)
        {
            try
            {
                lock (_lock)
                {
                    if (_currentImage != null) _currentImage.Dispose();
                    _currentImage = (Bitmap)e.Frame.Clone();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("NewFrame:" + ex.ToString());
            }
        }

        private ICommand _closeCmd;

        public ICommand CloseCmd
        {
            get { return _closeCmd ?? (_closeCmd = new DelegateCommand(CloseCommand)); }
        }

        private ICommand _captureCmd;

        public ICommand CaptureCmd
        {
            get { return _captureCmd ?? (_captureCmd = new DelegateCommand(CaptureCommand)); }
        }


        private ICommand _reCaptureCmd;

        public ICommand ReCaptureCmd
        {
            get { return _reCaptureCmd ?? (_reCaptureCmd = new DelegateCommand(ReCaptureCommand)); }
        }

        private ICommand _confirmCmd;

        public ICommand ConfirmCmd
        {
            get { return _confirmCmd ?? (_confirmCmd = new DelegateCommand(ConfirmCommand)); }
        }

        public ICommand AddFileCmd
        {
            get { return _addFileCmd ?? (_addFileCmd = new DelegateCommand(AddFileCommand)); }
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            try
            {
                if (_drawTimer.IsEnabled)
                    _drawTimer.Stop();
                if (_videoSource != null && _videoSource.IsRunning)
                    _videoSource.Stop();
            }
            catch
            { }
        }

        private void AddFileCommand(object arg)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPEG | *.jpg|PNG |*.png|BMP |*.bmp";
            if (dialog.ShowDialog() == true)
            {
                System.Drawing.Bitmap image = (Bitmap)PicUtil.GetImage(dialog.FileName);
                var bit = PicUtil.MakeThumbnail(image, 160, 0);
                lock (_lock)
                {
                    _currentImage = image;
                }
                videoShow.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private string _captureImageSrc = "";

        private void CaptureCommand(object arg)
        {
            if (IsEnabledButton)
            {
                _drawTimer.Stop();
            }
        }

        private void ReCaptureCommand(object arg)
        {
            if (IsEnabledButton)
            {
                _drawTimer.Start();
            }
        }

        private void CloseCommand(object arg)
        {
            this._drawTimer.Stop();
            this.videoShow.Source = null;
            if (_videoSource != null) this._videoSource.Stop();

            this.DialogResult = false;

            if (_currentImage != null) lock (_lock)
                {
                    _currentImage.Dispose();
                }
            this.Close();
        }

        private void ConfirmCommand(object arg)
        {
            try
            {
                this._drawTimer.Stop();
                this.videoShow.Source = null;
                if (_videoSource != null) this._videoSource.Stop();
            }
            catch (Exception ex)
            {

            }

            if (_currentImage != null)
            {
                var imgBytes = PicUtil.BitmapToBytes(_currentImage);
                var url = ThriftManager.UploadImg2Bimg(imgBytes);
                CaptureImageSrc = url;
            }
            _isCapture = false;
            _drawTimer.Stop();

            this.DialogResult = true;
            if (_currentImage != null) lock (_lock)
                {
                    _currentImage.Dispose();
                }
            this.Close();
        }

        #region Properties
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

        /// <summary>
        /// wait splash
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyChange("IsChecked");
            }
        }

        public bool IsEnabledButton
        {
            get { return _isEnabledButton; }
            set
            {
                _isEnabledButton = value;
                NotifyChange("IsEnabledButton");

            }
        }

        #endregion
    }
}
