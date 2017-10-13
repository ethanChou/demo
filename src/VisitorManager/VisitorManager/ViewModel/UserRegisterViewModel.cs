using AForge.Video;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using AForge.Video.DirectShow;
using System.Windows.Input;
using Renderer.Core;
using VisitorManager.Common;
using VisitorManager.Model;
using FirstFloor.ModernUI.Windows.Controls;
using Image = System.Windows.Controls.Image;

namespace VisitorManager
{
    public class UserRegisterViewModel : ViewModelBase
    {
        private ObservableCollection<Visitor> _visitors = new ObservableCollection<Visitor>();
        private ObservableCollection<Visitor> _waitvisitors = new ObservableCollection<Visitor>();
        private Image _display;
        private WriteableBitmapSource _wbSource;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;
        private System.Threading.ManualResetEvent _pauseEvent = new System.Threading.ManualResetEvent(false);
        private Visibility _defaultVis = Visibility.Visible;

        private ICommand _captureCmd;
        private ICommand _cancleCmd;
        private ICommand _addCmd;
        private ICommand _saveTempCmd;
        private ICommand _submitCmd;
        private ICommand _clearCmd;
        private ICommand _selectedCmd;
        private int _departmentIndex = -1;
        private int _employeeIndex = -1;
        private Bitmap _currentImage;
        private string _captureImageSrc;

        #region 身份证信息

        private int _cardIdType = 0;
        private string _cardId = "";
        private string _visitorName = "";
        private string _visitorAddr = "";
        private bool _gender = false;

        #endregion

        #region 发卡区

        private int _passCardType = 0;

        private string _passCardId = "";

        #endregion

        public UserRegisterViewModel(Image displayImage)
        {
            try
            {
                DeleteCommands.DeleteCmd = new DelegateCommand(DeleteVisitorCommand);

                this._display = displayImage;
                this._videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (_videoSource == null && _videoDevices.Count > 0)
                {
                    _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                    VideoCapabilities capabilty = _videoSource.VideoCapabilities[0];

                    this._wbSource = new WriteableBitmapSource();

                    if (this._wbSource.SetupSurface(
                        capabilty.FrameSize.Width, capabilty.FrameSize.Height, FrameFormat.RGB24))
                    {
                        this._display.Source = this._wbSource.ImageSource;
                    }
                    _videoSource.NewFrame += VideoSource_NewFrame;
                    _videoSource.PlayingFinished += VideoSource_PlayingFinished;
                    _videoSource.Start();
                    _pauseEvent.Set();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Visibility DefaultVis
        {
            get { return _defaultVis; }
            set
            {
                _defaultVis = value;
                NotifyChange("DefaultVis");
            }
        }

        public ICommand CaptureCmd
        {
            get { return _captureCmd ?? (_captureCmd = new DelegateCommand(CaptureCommand)); }
        }

        public ICommand CancleCmd
        {
            get { return _cancleCmd ?? (_cancleCmd = new DelegateCommand(CancleCommand)); }
        }

        public ICommand AddCmd
        {
            get { return _addCmd ?? (_addCmd = new DelegateCommand(AddCommand)); }
        }

        public ICommand SaveTempCmd
        {
            get { return _saveTempCmd ?? (_saveTempCmd = new DelegateCommand(SaveTempCommand)); }
        }

        public ICommand SubmitCmd
        {
            get { return _submitCmd ?? (_submitCmd = new DelegateCommand(SubmitCommand)); }
        }


        public ICommand ClearCmd
        {
            get { return _clearCmd ?? (_clearCmd = new DelegateCommand(ClearCommand)); }
        }

        public ObservableCollection<Department> Departments
        {
            get
            {
                return DataManager.Departments;
            }
            set
            {
                DataManager.Departments = value;
                NotifyChange("Departments");
            }
        }

        public ObservableCollection<Employee> Employees
        {
            get { return DataManager.Employees; }
            set
            {
                DataManager.Employees = value;
                NotifyChange("Employees");
            }
        }

        public int CardIdType
        {
            get { return _cardIdType; }
            set
            {
                _cardIdType = value;
                NotifyChange("CardIdType");
            }
        }

        public string CardId
        {
            get { return _cardId; }
            set
            {
                _cardId = value;
                NotifyChange("CardId");
            }
        }

        public string VisitorName
        {
            get { return _visitorName; }
            set
            {
                _visitorName = value;
                NotifyChange("VisitorName");
            }
        }

        public string VisitorAddr
        {
            get { return _visitorAddr; }
            set
            {
                _visitorAddr = value;
                NotifyChange("VisitorAddr");

            }
        }

        public bool Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                NotifyChange("Gender");

            }
        }

        public int PassCardType
        {
            get { return _passCardType; }
            set
            {
                _passCardType = value;
                NotifyChange("PassCardType");
            }
        }

        public string PassCardId
        {
            get { return _passCardId; }
            set
            {
                _passCardId = value;
                NotifyChange("PassCardId");

            }
        }

        public int DepartmentIndex
        {
            get { return _departmentIndex; }
            set
            {
                _departmentIndex = value;
                NotifyChange("DepartmentIndex");

            }
        }

        public int EmployeeIndex
        {
            get { return _employeeIndex; }
            set
            {
                _employeeIndex = value;
                NotifyChange("EmployeeIndex");
            }
        }

        public ObservableCollection<Visitor> Visitors
        {
            get
            {
                return DataManager.WaitingVisitors;
            }
            set
            {
                DataManager.WaitingVisitors = value;
            }
        }

        public ObservableCollection<Visitor> PauseVisitors
        {
            get
            {
                return DataManager.PauseVisitors;
            }
            set
            {
                DataManager.PauseVisitors = value;
            }
        }

        public ICommand SelectedCmd
        {
            get { return _selectedCmd ?? (_selectedCmd = new DelegateCommand(SelectedCommand)); }
        }

        public bool EnableSubmit
        {
            get
            {
                if (Visitors.Count > 0) return true;
                return false;
            }

        }

        public string CaptureImageSrc
        {
            get { return _captureImageSrc; }
            set
            {
                _captureImageSrc = value;
            }
        }

        private void DeleteVisitorCommand(object arg)
        {
            Visitor v = arg as Visitor;
            if (v != null)
            {
                if (ModernDialog.ShowMessage("是否立即删除？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Visitors.Remove(v);
                    DbUtil.DeleteModel<Visitor>(v, "vt_identify_NO");
                }
            }
        }

        private void SelectedCommand(object arg)
        {
            Visitor visitor = arg as Visitor;
            if (visitor != null)
            {
                CardIdType = visitor.vt_identify_type;
                CardId = visitor.vt_identify_NO;
                VisitorName = visitor.vt_name;
                Gender = visitor.vt_sex;
                VisitorAddr = visitor.vt_address;
                PassCardType = visitor.tmpcard_type;
                PassCardId = visitor.tmpcard_id;

                var dep = Departments.FirstOrDefault(t => t.dep_id == visitor.vt_visit_department_id);
                if (dep != null)
                {
                    int index = Departments.IndexOf(dep);
                    DepartmentIndex = index;
                }

                var emp = Employees.FirstOrDefault(t => t.emp_id == visitor.vt_visit_employee_id);
                if (emp != null)
                {
                    int index = Employees.IndexOf(emp);
                    EmployeeIndex = index;
                }
            }
        }



        private void CaptureCommand(object arg)
        {
            _pauseEvent.Reset();

            if (_currentImage != null)
            {
                CaptureImageSrc = string.Format("{0}\\VisitorLib\\capture\\{1}.jpg", "D:", DateTime.Now.ToFileTime());
                if (!Directory.Exists(Path.GetDirectoryName(CaptureImageSrc)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(CaptureImageSrc));
                }
                _currentImage.Save(CaptureImageSrc, ImageFormat.Jpeg);
            }
        }

        private void CancleCommand(object arg)
        {
            _pauseEvent.Set();
            CaptureImageSrc = "";
        }

        private void AddCommand(object arg)
        {
            if (string.IsNullOrEmpty(CardId))
            {
                var result = ModernDialog.ShowMessage("证件号码不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(VisitorName))
            {
                var result = ModernDialog.ShowMessage("姓名不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(PassCardId))
            {
                var result = ModernDialog.ShowMessage("通行证编号不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CaptureImageSrc))
            {
                var result = ModernDialog.ShowMessage("请抓拍来访者照片.", "提示", MessageBoxButton.OK);
                return;
            }

            if (Visitors.ToList().Find(t => t.vt_identify_NO == CardId) != null)
            {
                ModernDialog.ShowMessage("相同证件号码不能添加两次.", "提示", MessageBoxButton.OK);
                return;
            }
            if (DepartmentIndex < 0)
            {
                ModernDialog.ShowMessage("请选择被访单位.", "提示", MessageBoxButton.OK);
                return;
            }
            if (EmployeeIndex < 0)
            {
                ModernDialog.ShowMessage("请选择被访人员.", "提示", MessageBoxButton.OK);
                return;
            }

            Visitor visitor = PauseVisitors.FirstOrDefault(t => t.vt_identify_NO == CardId);
            if (visitor != null)
            {
                //之前已经暂存过，需要删除
                PauseVisitors.Remove(visitor);

                visitor.vt_identify_imgurl = CaptureImageSrc;
                visitor.vt_identify_type = CardIdType;
                visitor.vt_identify_NO = CardId;
                visitor.vt_imgurl = CaptureImageSrc;
                visitor.vt_name = VisitorName;
                visitor.vt_sex = Gender;
                //等待提交
                visitor.vt_status = VisitorStatus.Waiting;
                visitor.vt_in_time = DateTime.Now;
                visitor.vt_address = VisitorAddr;
                visitor.tmpcard_id = PassCardId;
                visitor.tmpcard_type = PassCardType;
                visitor.vt_visit_department_id = Departments[DepartmentIndex].dep_id;
                visitor.vt_visit_department = Departments[DepartmentIndex].dep_name;
                visitor.vt_visit_employee_id = Employees[EmployeeIndex].emp_id;
                visitor.vt_visit_employee = Employees[EmployeeIndex].emp_name;

                DbUtil.UpdateModel<Visitor>(visitor, "vt_id");
            }
            else
            {
                visitor = new Visitor()
                {
                    vt_id = Guid.NewGuid().ToString(),
                    vt_identify_imgurl = CaptureImageSrc,
                    vt_identify_type = CardIdType,
                    vt_identify_NO = CardId,
                    vt_imgurl = CaptureImageSrc,
                    vt_name = VisitorName,
                    vt_sex = Gender,
                    //等待提交
                    vt_status = VisitorStatus.Waiting,
                    vt_in_time = DateTime.Now,
                    vt_address = VisitorAddr,
                    tmpcard_id = PassCardId,
                    tmpcard_type = PassCardType,
                    vt_visit_department_id = Departments[DepartmentIndex].dep_id,
                    vt_visit_department = Departments[DepartmentIndex].dep_name,
                    vt_visit_employee_id = Employees[EmployeeIndex].emp_id,
                    vt_visit_employee = Employees[EmployeeIndex].emp_name,
                };

                DbUtil.InsertModel<Visitor>(visitor);
            }

            Visitors.Add(visitor);

            NotifyChange("EnableSubmit");

            Reset(false);
        }

        private void SaveTempCommand(object arg)
        {
            if (string.IsNullOrEmpty(CardId))
            {
                var result = ModernDialog.ShowMessage("证件号码不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (Visitors.FirstOrDefault(v => v.vt_identify_NO == CardId) != null)
            {
                var result = ModernDialog.ShowMessage("来访者已存在,不可暂存.", "提示", MessageBoxButton.OK);
                return;
            }

            Visitor oldVisitor = PauseVisitors.FirstOrDefault(v => v.vt_identify_NO == CardId);
            if (oldVisitor != null)
            {
                //存在的话，更新值
                oldVisitor.vt_identify_imgurl = CaptureImageSrc;
                oldVisitor.vt_identify_type = CardIdType;
                oldVisitor.vt_identify_NO = CardId;
                oldVisitor.vt_imgurl = CaptureImageSrc;
                oldVisitor.vt_in_time = DateTime.Now;
                oldVisitor.vt_address = VisitorAddr;
                oldVisitor.tmpcard_id = PassCardId;
                oldVisitor.tmpcard_type = PassCardType;
                oldVisitor.vt_name = VisitorName;
                oldVisitor.vt_sex = Gender;
                oldVisitor.vt_status = VisitorStatus.Pause;
                oldVisitor.vt_visit_department_id = Departments[DepartmentIndex].dep_id;
                oldVisitor.vt_visit_department = Departments[DepartmentIndex].dep_name;
                oldVisitor.vt_visit_employee_id = Employees[EmployeeIndex].emp_id;
                oldVisitor.vt_visit_employee = Employees[EmployeeIndex].emp_name;
                oldVisitor.NoticeAll();

                var res = DbUtil.UpdateModel<Visitor>(oldVisitor, "vt_id");
            }
            else
            {
                //不存在则添加
                Visitor visitor = new Visitor()
                {
                    vt_id = Guid.NewGuid().ToString(),
                    vt_identify_imgurl = CaptureImageSrc,
                    vt_identify_type = CardIdType,
                    vt_identify_NO = CardId,
                    vt_imgurl = CaptureImageSrc,
                    vt_in_time = DateTime.Now,
                    vt_address = VisitorAddr,
                    tmpcard_id = PassCardId,
                    tmpcard_type = PassCardType,
                    vt_name = VisitorName,
                    vt_sex = Gender,
                    vt_status = VisitorStatus.Pause,
                    vt_visit_department_id = Departments[DepartmentIndex].dep_id,
                    vt_visit_department = Departments[DepartmentIndex].dep_name,
                    vt_visit_employee_id = Employees[EmployeeIndex].emp_id,
                    vt_visit_employee = Employees[EmployeeIndex].emp_name,
                };
                DbUtil.InsertModel<Visitor>(visitor);
                PauseVisitors.Add(visitor);
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="arg"></param>
        private void SubmitCommand(object arg)
        {
            string id = GuidIndex;
            foreach (var v in Visitors)
            {
                //所有来访单号，在提交的时候才确认生成。
                v.vt_visitinglist_id = id;
                //状态修改成已进入状态
                v.vt_status = VisitorStatus.In;

                //只负责更新
                bool res = DbUtil.UpdateModel<Visitor>(v, "vt_id");

                if (NewVisitor != null)
                {
                    NewVisitor(v);
                }
            }

            VisitingList vl = new VisitingList();
            {
                vl.vtl_id = id;
                vl.vtl_time = DateTime.Now;
            }
            DbUtil.InsertModel<VisitingList>(vl);

            Reset();
        }

        public static event Action<Visitor> NewVisitor;

        private string _guidIndex;
        /// <summary>
        /// 每次都会生成新的一个id。
        /// </summary>
        private string GuidIndex
        {
            get
            {
                DateTime time = DateTime.Now;
                _guidIndex = time.ToString("yyyyMMddhhMMss");
                return _guidIndex;
            }
        }

        private void ClearCommand(object arg)
        {

        }

        private void VideoSource_PlayingFinished(object sender, ReasonToFinishPlaying reason)
        {
            DefaultVis = Visibility.Visible;
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs e)
        {
            if (_defaultVis == Visibility.Visible) DefaultVis = Visibility.Collapsed;

            if (e.Buffer != IntPtr.Zero && e.BufferLen > 0)
            {
                this._wbSource.Render(e.Buffer);
            }
            else
            {
                if (_currentImage != null) _currentImage.Dispose();
                _currentImage = (Bitmap)e.Frame.Clone();
                BitmapData data = e.Frame.LockBits(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                this._wbSource.Render(data.Scan0);
                e.Frame.UnlockBits(data);
            }
            _pauseEvent.WaitOne();
        }

        private void Reset(bool force = true)
        {
            EmployeeIndex = -1;
            DepartmentIndex = -1;

            CardIdType = 0;
            CardId = "";
            VisitorName = "";
            VisitorAddr = "";
            Gender = false;

            PassCardId = "";
            PassCardType = 0;

            CaptureImageSrc = "";

            CancleCommand(null);

            //刷新guid，重新生成来访单号
            if (force)
            {
                Visitors.Clear();
            }
        }
    }
}
