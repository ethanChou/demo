using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using VisitorManager.ViewModel;
using WPF.Extend;

namespace VisitorManager.Content
{
    /// <summary>
    /// CameraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CameraWindow : Window, INotifyPropertyChanged, ICaptureWindow
    {
        private Visibility _defaultVis = Visibility.Visible;
        private Bitmap _currentImage;
        private ICommand _addFileCmd;

        public CameraWindow()
        {
            InitializeComponent();

            if (LocalConfig.IsAddLocalFile)
            {
                addLocalFile.Visibility = System.Windows.Visibility.Visible;
            }

            this.Loaded += CaptureWindow_Loaded;
            this.DataContext = this;
        }

        private FilterInfoCollection videoDevices;
        private void CaptureWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    this.Title+="-未发现摄像头.";
                }

                for (int i = 1, n = videoDevices.Count; i <= n; i++)
                {
                    string cameraName = i + " : " + videoDevices[i - 1].Name;

                    cboCamera.Items.Add(cameraName);
                }

                // check cameras count
                if (videoDevices.Count >= 1)
                {
                    cboCamera.SelectedIndex = 0;

                    VideoCaptureDevice videoSource1 = new VideoCaptureDevice(videoDevices[cboCamera.SelectedIndex].MonikerString);
                    videoSource1.DesiredFrameRate = 10;
                  
                    this.videoPlayer.VideoSource = videoSource1;
                    this.videoPlayer.Start();
                    if (videoPlayer.IsRunning)
                    {

                    }
                }
            }
            catch
            {

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
               if (videoPlayer.IsRunning) 
                {
                    videoPlayer.SignalToStop();
                    videoPlayer.WaitForStop();
                }
               
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
                image.Dispose();
                _currentImage =(Bitmap) bit;
                CaptureImageSrc = dialog.FileName;
            }
        }

        private string _captureImageSrc = "";

        private void CaptureCommand(object arg)
        {
            if (!videoPlayer.IsRunning) return;
            Bitmap bitmap = this.videoPlayer.GetCurrentVideoFrame();

            if (bitmap != null)
            {

                var bit = PicUtil.MakeThumbnail(bitmap, 160, 0);
                _currentImage =(Bitmap) bit;
                var tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                bit.Save(tmp, ImageFormat.Jpeg);
                CaptureImageSrc = tmp;

                bit.Dispose();

                bitmap.Dispose();
            }
        }

        private void CloseCommand(object arg)
        {
            this.DialogResult = false;

            if (_currentImage != null)

                _currentImage.Dispose();

            this.Close();
        }

        private void ConfirmCommand(object arg)
        {
            if (_currentImage != null)
            {
                var imgBytes = PicUtil.BitmapToBytes(_currentImage);
                var url = ThriftManager.UploadImg2Bimg(imgBytes);
                Console.WriteLine("Upload: "+url);
                CaptureImageSrc = url;
            }

            this.DialogResult = true;
            if (_currentImage != null)
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

       
        #endregion

        private void cboCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
