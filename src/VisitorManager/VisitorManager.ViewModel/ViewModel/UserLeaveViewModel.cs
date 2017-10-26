using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPF.Extend;
using VisitorManager.Model;
using VisitorManager.ViewModel.Common;

namespace VisitorManager
{

    public class UserLeaveViewModel : ViewModelBase
    {
        public static Visitor Visitor { get; set; }

        private ObservableCollection<Visitor> _fellowVisitors;

        private static List<Visitor> _checkList;

        private ICommand _leaveCmd;

        public UserLeaveViewModel()
        {
            GotoCommands.Navigated += GotoCommands_Navigated;

            SpashVis = Visibility.Visible;
            _checkList = new List<Visitor>();
            _fellowVisitors = new ObservableCollection<Visitor>();
            ComboxCheckedCommands.CheckedCmd = new DelegateCommand(CheckedCommand);
            ComboxCheckedCommands.UnCheckedCmd = new DelegateCommand(UnCheckedCommand);
        }

      

        public void Loaded()
        {
            if (Visitor != null)
            {
                CaptureImage = Visitor.vt_imgurl;
                CardImage = Visitor.vt_identify_imgurl;
                SpashVis = Visibility.Collapsed;
                UserName = Visitor.vt_name;
                UserId = Visitor.vt_identify_NO;
                Department = Visitor.vt_visit_department;
                Emploayee = Visitor.vt_visit_employee;
                HappenTime = Visitor.vt_in_time.ToString("yyyy-MM-dd HH:mm:ss");
                VisitinglistId = Visitor.vt_visitinglist_id;

                NotifyChange("CaptureImage");
                NotifyChange("CardImage");
                NotifyChange("SpashVis");
                NotifyChange("UserName");
                NotifyChange("UserId");
                NotifyChange("Department");
                NotifyChange("Emploayee");
                NotifyChange("HappenTime");
                NotifyChange("VisitinglistId");

                FellowVisitors.Clear();

                var list = DbUtil.SelectModel<Visitor>(string.Format("vt_visitinglist_id==\"{0}\"", Visitor.vt_visitinglist_id));

                list.ForEach(t => FellowVisitors.Add(t));
            }
        }

        public string CaptureImage { get; set; }

        public string CardImage { get; set; }

        public Visibility SpashVis { get; set; }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Department { get; set; }
        public string Emploayee { get; set; }
        public string HappenTime { get; set; }
        public string VisitinglistId { get; set; }

        public ObservableCollection<Visitor> FellowVisitors
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

        void GotoCommands_Navigated(object obj)
        {
            Visitor = obj as Visitor;
            Loaded();

            MainWindowViewModel.Singleton.TabCmd.Execute(2);
        }

        private void LeaveCommand(object arg)
        {
            if (_checkList.Count == 0)
            {
                MsgBox.Show("请勾选来访人员.", "提示", MessageBoxButton.OK);
                return;
            }
            if (MsgBox.Show("是否确认离开?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var item in _checkList)
                {
                    DbUtil.UpdateModel<Visitor>(item, "vt_id");
                }
                UserVisitingViewModel.Single.UpdateData();
            }
        }

        private static void CheckedCommand(object arg)
        {
            Visitor v = arg as Visitor;
            if (v != null)
            {
                if (!_checkList.Contains(v))
                {
                    v.vt_status = VisitorStatus.Out;
                    _checkList.Add(v);
                }
            }
        }

        private static void UnCheckedCommand(object arg)
        {
            Visitor v = arg as Visitor;
            if (v != null)
            {
                if (_checkList.Contains(v))
                {
                    v.vt_status = VisitorStatus.In;
                    _checkList.Remove(v);
                }
            }
        }
    }
}
