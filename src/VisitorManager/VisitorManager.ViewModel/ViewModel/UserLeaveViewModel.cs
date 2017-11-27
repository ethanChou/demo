using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ThriftCommon;
using WPF.Extend;


namespace VisitorManager.ViewModel
{

    public class UserLeaveViewModel : ViewModelBase
    {
        MainWindowViewModel MainVM;
        public class VisitorEx : Visitor
        {
            public VisitorEx(Visitor v)
            {
                this.Vt_vl_id = v.Vt_vl_id;
                this.Vt_visit_employee_id = v.Vt_visit_employee_id;
                this.Vt_visit_department_id = v.Vt_visit_department_id;
                this.Vt_status = v.Vt_status;
                this.Vt_sex = v.Vt_sex;
                this.Vt_out_time = v.Vt_out_time;
                this.Vt_name = v.Vt_name;
                this.Vt_in_time = v.Vt_in_time;
                this.Vt_imgurl = v.Vt_imgurl;
                this.Vt_identify_type = v.Vt_identify_type;
                this.Vt_identify_no = v.Vt_identify_no;
                this.Vt_identify_imgurl = v.Vt_identify_imgurl;
                this.Vt_id = v.Vt_id;
                this.Tmpcard_no = v.Tmpcard_no;
            }
            private bool isChecked = false;

            public bool IsChecked
            {
                get
                {
                    return isChecked;
                }

                set
                {
                    isChecked = value;
                }
            }

            public bool IsEnable
            {
                get { return _isEnable; }
                set
                {
                    _isEnable = value;
                }
            }

            private bool _isEnable = false;
        }
        public static Visitor Visitor { get; set; }

        private ObservableCollection<VisitorEx> _fellowVisitors;

        private static string DefaultImageSrc = AppDomain.CurrentDomain.BaseDirectory + "Image\\Msg\\TransparentImg.png";
        private ICommand _leaveCmd;
        private ICommand _cancleCmd;
        public UserLeaveViewModel()
            : this(null)
        { }

        public UserLeaveViewModel(MainWindowViewModel parent)
        {
            MainVM = parent;
            CaptureImage = DefaultImageSrc;
            CardImage = DefaultImageSrc;

            _fellowVisitors = new ObservableCollection<VisitorEx>();
        }

        public string CaptureImage { get; set; }
        public string CardImage { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Department { get; set; }
        public string Emploayee { get; set; }
        public string HappenTime { get; set; }
        public string VisitinglistId { get; set; }
        public string ObjectStr { get; set; }

        public ObservableCollection<VisitorEx> FellowVisitors
        {
            get { return _fellowVisitors; }
            set { _fellowVisitors = value; }
        }

        public ICommand LeaveCmd
        {
            get
            {
                return _leaveCmd ?? (_leaveCmd = new DelegateCommand(LeaveCommand));
            }
        }

        public ICommand CancleCmd
        {
            get { return _cancleCmd ?? (_cancleCmd = new DelegateCommand(CancleCommand)); }
        }

        public Visibility ShowLostCardBtnVis
        {
            get { return _showLostCardBtnVis; }
            set
            {
                _showLostCardBtnVis = value;
                NotifyChange("ShowLostCardBtnVis");
            }
        }


        private void UpdateVisitor(Visitor v)
        {
            if (v == null) return;
            CaptureImage = v.Vt_imgurl;
            CardImage = v.Vt_identify_imgurl;
            UserName = v.Vt_name;
            UserId = v.Vt_identify_no;
            Department = v.Vt_visit_department_id;
            Emploayee = v.Vt_visit_employee_id;
            HappenTime = new DateTime(v.Vt_in_time).ToString("yyyy-MM-dd HH:mm:ss");
            VisitinglistId = v.Vt_vl_id;

            NotifyChange("CaptureImage");
            NotifyChange("CardImage");
            NotifyChange("SpashVis");
            NotifyChange("UserName");
            NotifyChange("UserId");
            NotifyChange("Department");
            NotifyChange("Emploayee");
            NotifyChange("HappenTime");
            NotifyChange("VisitinglistId");
        }

        private void CancleCommand(object arg)
        {
            Reset();
            MainWindowViewModel.Singleton.TabCmd.Execute(0);
        }

        private void LeaveCommand(object arg)
        {
            var tmp = FellowVisitors.FirstOrDefault(t => t.IsChecked == true);
            if (tmp == null)
            {
                MsgBox.Show("请勾选来访人员.", "提示", MessageBoxButton.OK);
                return;
            }

            int leveType = int.Parse(arg.ToString());
            if (MsgBox.Show("是否确认离开?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var item in FellowVisitors)
                {
                    if (item.IsChecked)
                    {
                        item.Vt_status = leveType == 0 ? Status.Leave : Status.LostCard;
                        item.Vt_out_time = DateTime.Now.Ticks;
                        bool f = ThriftManager.UpdateVisitor(item);
                        if (!f)
                        {
                            MsgBox.Show("删除数据失败,请重启程序.", "提示", MessageBoxButton.OK);
                            return;
                        }
                    }
                }

                FellowVisitors.Clear();

                Reset();

                UserVisitingViewModel.Single.BeginUpdateData();

                MainWindowViewModel.Singleton.TabCmd.Execute(0);
            }
        }

        private void Reset()
        {
            CaptureImage = "";
            CardImage = "";
            UserName = "";
            UserId = "";
            Department = "";
            Emploayee = "";
            HappenTime = "";
            VisitinglistId = "";
            ObjectStr = "";
            FellowVisitors.Clear();
            NotifyChange("CaptureImage");
            NotifyChange("CardImage");
            NotifyChange("UserName");
            NotifyChange("UserId");
            NotifyChange("Department");
            NotifyChange("Emploayee");
            NotifyChange("HappenTime");
            NotifyChange("VisitinglistId");
            NotifyChange("ObjectStr");
            NotifyChange("ShowLostCardBtnVis");

        }

        private Visibility _showLostCardBtnVis = Visibility.Collapsed;

        /// <summary>
        /// 正在访问用户离开
        /// </summary>
        /// <param name="v"></param>
        internal void VisitorLeave(Visitor v)
        {
            if (!MainVM.MainWindow.Dispatcher.CheckAccess())
            {
                MainVM.MainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    VisitorLeave(v);
                }));
                return;
            }
            Visitor = v;
            if (Visitor != null)
            {
                UpdateVisitor(Visitor);

                var res = ThriftManager.GetVisitorLists(v.Vt_vl_id, 0, 0);

                if (res.Count > 0)
                    ObjectStr = res[0].Vl_carryThings;
                NotifyChange("ObjectStr");
                FellowVisitors.Clear();

                var list = ThriftManager.GetVisitors("", Visitor.Vt_vl_id, "", IdentifyType.Ohter, "", "", 0, 0, Status.None, "", "");

                list.ForEach(t =>
                {
                    var vex = new VisitorEx(t);
                    
                    if (t.Vt_id == v.Vt_id && (v.Vt_status == Status.Visiting||v.Vt_status== Status.LostCard))
                    {
                        vex.IsChecked = t.Vt_id == v.Vt_id;
                        vex.IsEnable = true;
                    }

                    FellowVisitors.Add(vex);
                });
                list = null;
            }
        }

        internal void SelectedCommand(object arg)
        {
            VisitorEx v = arg as VisitorEx;
            if (v != null)
            {
                Visitor = v;
                UpdateVisitor(v);
            }
        }

        //internal void CheckedCommand(object arg)
        //{
        //    VisitorEx v = arg as VisitorEx;
        //    if (v != null)
        //    {
        //        if (!_checkList.Contains(v))
        //        {
        //            _checkList.Add(v);
        //        }
        //    }
        //}

        //internal void UnCheckedCommand(object arg)
        //{
        //    VisitorEx v = arg as VisitorEx;
        //    if (v != null)
        //    {
        //        if (_checkList.Contains(v))
        //        {
        //            _checkList.Remove(v);
        //        }
        //    }
        //}
    }
}
