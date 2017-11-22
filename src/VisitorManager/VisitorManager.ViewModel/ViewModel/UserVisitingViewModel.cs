using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ThriftCommon;
using WPF.Extend;

namespace VisitorManager.ViewModel
{
    public class UserVisitingViewModel : ViewModelBase
    {

        public Type WindowType { get; set; }
        MainWindowViewModel _mainVM;
        private int _searchTimeIndex = 0;
        public static UserVisitingViewModel Single { get; set; }
        private DispatcherTimer _workTimer;
        private ObservableCollection<Visitor> _visistors;
        private List<Visitor> _srcVisistors;
        private int _departmentIndex = -1;
        private int _employeeIndex = -1;
        private ICommand _searchCmd;
        private Status _statusIndex = Status.Visiting;
        private int _currentVisitors = 0;
        private ICommand _statusCmd;
        private string _conditionStr = "";
        public UserVisitingViewModel()
            : this(null)
        { }

        public UserVisitingViewModel(MainWindowViewModel parent)
        {
            _mainVM = parent;
            _srcVisistors = new List<Visitor>();
            _visistors = new ObservableCollection<Visitor>();

            _workTimer = new DispatcherTimer();
            _workTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _workTimer.Tick += WorkTimer_Tick;
            _workTimer.Start();
            Single = this;
        }


        /// <summary>
        /// 根据TMPID查找是否存在此正在访问用户
        /// </summary>
        /// 
        /// <remarks>不存在则返回null</remarks>
        /// <param name="tmpid"></param>
        /// <returns></returns>
        internal Visitor ExistsCard(string tmpid, bool isTempCard = true)
        {
            for (int i = 0; i < Visistors.Count; i++)
            {
                if (isTempCard)
                {
                    if (Visistors[i].Tmpcard_no == tmpid && Visistors[i].Vt_status == Status.Visiting)
                    {
                        return Visistors[i];
                    }
                }
                else
                {
                    if (Visistors[i].Vt_identify_no == tmpid && Visistors[i].Vt_status == Status.Visiting)
                    {
                        return Visistors[i];
                    }
                }
            }
            return null;
        }


        internal void VisitorDeleted(object obj)
        {
            Visitor v = obj as Visitor;
            if (v != null)
            {
                if (MsgBox.Show("是否删除此条记录?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    ThriftManager.DeleteVisitor(v.Vt_id);
                    Visistors.Remove(v);
                    CurrentVisitors = Visistors.Count;
                }
            }

        }

        internal void VisitorInfo(object arg)
        {
            Visitor vt = arg as Visitor;
            if (vt != null)
            {
                IContextWindow window = (IContextWindow)Activator.CreateInstance(WindowType);
                if (window != null)
                {
                    window.DataContext = vt;
                    window.ShowDialog();
                }
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DbUtil.DB_PATH))
            {
                _workTimer.Stop();
                GC.Collect();
                UpdateVisitors();
            }
        }

        public void UpdateData()
        {
            _workTimer.Start();
        }

        internal void UpdateVisitor(Visitor obj)
        {
            UpdateData();
        }

        /// <summary>
        /// 获取当前系统的版本号
        /// </summary>
        /// <returns></returns>
        public string GetEdition()
        {
            return "V" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public string Version
        {
            get
            {
                var v = GetEdition();
                return v;
            }
        }

        private void UpdateVisitors()
        {
            Visistors.Clear();

            DateTime bt = DateTime.Now, et = DateTime.Now;

            DateTime nt = DateTime.Now;
            if (SearchTimeIndex == 0)
            {
                bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
                et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);
            }
            if (SearchTimeIndex == 1)//三天
            {
                et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);

                nt = nt.AddDays(-3);
                bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
            }

            if (SearchTimeIndex == 2)//一周
            {
                et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);

                nt = nt.AddDays(-7);
                bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
            }

            _srcVisistors = ThriftManager.GetVisitors(
                String.Empty, String.Empty,
                ConditionStr, IdentifyType.IdCard,
                String.Empty, String.Empty, bt.Ticks, et.Ticks,
                _statusIndex,
                String.Empty, String.Empty);

            _srcVisistors.Sort(new TimeComparer(false));

            _srcVisistors.ForEach(t =>
            {
                Visistors.Add(t);
            });

            CurrentVisitors = Visistors.Count;
        }

        public ObservableCollection<Visitor> Visistors
        {
            get { return _visistors; }
            set { _visistors = value; }
        }

        public ICommand SearchCmd
        {
            get
            {
                return _searchCmd ?? (_searchCmd = new DelegateCommand(SearchCommand));
            }

        }

        private ICommand _clearCmd;

        public ICommand ClearCmd
        {
            get { return _clearCmd ?? (_clearCmd = new DelegateCommand(ClearCommand)); }
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

        public int CurrentVisitors
        {
            get { return _currentVisitors; }
            set
            {
                _currentVisitors = value;
                NotifyChange("CurrentVisitors");
            }
        }

        public ICommand StatusCmd
        {
            get { return _statusCmd ?? (_statusCmd = new DelegateCommand(StatusCommand)); }
        }

        public string ConditionStr
        {
            get { return _conditionStr; }
            set
            {
                _conditionStr = value;
                NotifyChange("ConditionStr");
            }
        }

        public int SearchTimeIndex
        {
            get { return _searchTimeIndex; }
            set
            {
                _searchTimeIndex = value;
                NotifyChange("SearchTimeIndex");
            }
        }

        private void StatusCommand(object arg)
        {
            _statusIndex = (Status)int.Parse(arg.ToString());

        }

        private void SearchCommand(object arg)
        {
            UpdateVisitors();
        }

        private void ClearCommand(object arg)
        {

            DepartmentIndex = -1;
            EmployeeIndex = -1;
        }
    }
}
