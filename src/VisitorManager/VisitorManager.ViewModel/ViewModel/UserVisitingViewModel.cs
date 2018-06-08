using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
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
        private bool _isCheckedBusyBox = false;
        public static UserVisitingViewModel Single { get; set; }
        private Timer _workTimer;
        private Timer _workTimer2;
        private ObservableCollection<Visitor> _visistors;
        private List<Visitor> _srcVisistors;
        private int _departmentIndex = -1;
        private int _employeeIndex = -1;
        private ICommand _searchCmd;
        private Status _statusIndex = Status.Visiting;
        private int _currentVisitors = 0;
        private ICommand _statusCmd;
        private string _conditionStr = "";
        private bool _isAutoGenerateWay = false;
        public UserVisitingViewModel()
            : this(null)
        { }

        public UserVisitingViewModel(MainWindowViewModel parent)
        {
            _mainVM = parent;
            _srcVisistors = new List<Visitor>();
            _visistors = new ObservableCollection<Visitor>();

            _workTimer2 = new Timer();
            _workTimer2.Interval = 100;
            _workTimer2.Elapsed += WorkTimer2_Tick;
            _workTimer2.Start();

            _workTimer = new Timer();
            _workTimer.Interval = 100;
            _workTimer.Elapsed += WorkTimer_Tick;
            _workTimer.Start();
            Single = this;
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


        void WorkTimer2_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DbUtil.DB_PATH))
            {

                _workTimer2.Stop();

                UpdateCountCore();

                GC.Collect();
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DbUtil.DB_PATH))
            {
                _workTimer.Stop();
                UpdateDataCore();
                UpdateCountCore();
                GC.Collect();
            }
        }

        private int _levaeCount = 0;
        private int _visitingCount = 0;
        private int _lostCount = 0;
        private int _totalCount = 0;
        private int _nocomebackCount = 0;
        private int _nocomebackInTimeCount = 0;

        public void BeginUpdateData()
        {
            if (!_workTimer.Enabled)
                _workTimer.Start();
        }

        public void BeginUpdateCount()
        {
            if (!_workTimer2.Enabled)
                _workTimer2.Start();
        }

        internal void UpdateData()
        {
            BeginUpdateData();
        }

        /// <summary>
        /// 获取当前系统的版本号
        /// </summary>
        /// <returns></returns>
        public string GetEdition()
        {
            return "V" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private Dispatcher Dispatcher
        {

            get { return _mainVM.MainWindow.Dispatcher; }
        }

        public string Version
        {
            get
            {
                var v = GetEdition();
                return v;
            }
        }

        private void UpdateCountCore()
        {
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

            int v1 = ThriftManager.GetVisitorCount(Status.NoComeBack, 0, 0);
            int v2 = ThriftManager.GetVisitorCount(Status.Visiting, bt.Ticks, et.Ticks);
            int v3 = ThriftManager.GetVisitorCount(Status.LostCard, bt.Ticks, et.Ticks);
            int v4 = ThriftManager.GetVisitorCount(Status.Leave, bt.Ticks, et.Ticks);
            int v5 = ThriftManager.GetVisitorCount(Status.None, bt.Ticks, et.Ticks);

            if (Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    VisitingCount = v2;
                    LostCount = v3;
                    LevaeCount = v4;
                    TotalCount = v5;
                    NocomebackCount = v1;
                    NocomebackInTimeCount = Math.Max(0, v5 - v2 - v3 - v4);
                }));
            }
            else
            {
                VisitingCount = v2;
                LostCount = v3;
                LevaeCount = v4;
                TotalCount = v5;
                NocomebackCount = v1;
                NocomebackInTimeCount = Math.Max(0, v5 - v2 - v3 - v4);
            }
        }

        private void UpdateDataCore()
        {
            Stopwatch w = null;
            try
            {
                IsCheckedBusyBox = true;

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Visistors.Clear();
                    }));
                }
                else
                {
                    Visistors.Clear();
                }

                DateTime bt = DateTime.Now, et = DateTime.Now;

                DateTime nt = DateTime.Now;
                if (SearchTimeIndex == 0)
                {
                    bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
                    et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);
                }
                if (SearchTimeIndex == 1) //三天
                {
                    et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);

                    nt = nt.AddDays(-3);
                    bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
                }

                if (SearchTimeIndex == 2) //一周
                {
                    et = new DateTime(nt.Year, nt.Month, nt.Day, 23, 59, 59);

                    nt = nt.AddDays(-7);
                    bt = new DateTime(nt.Year, nt.Month, nt.Day, 0, 0, 0);
                }
                w = new Stopwatch();
                w.Start();
                _srcVisistors = ThriftManager.GetVisitors(
                    String.Empty, String.Empty,
                    ConditionStr, IsAutoGenerateWay ? IdentifyType.Ohter : IdentifyType.IdCard,
                    String.Empty, String.Empty,
                    _statusIndex == Status.NoComeBack ? 0 : bt.Ticks, _statusIndex == Status.NoComeBack ? 0 : et.Ticks,
                    _statusIndex,
                    String.Empty, String.Empty);
                Console.WriteLine(w.ElapsedMilliseconds);
                _srcVisistors.Sort(new TimeComparer(false));

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Visistors.AddRange(_srcVisistors);

                        CurrentVisitors = Visistors.Count;
                    }));
                }
                else
                {
                    Visistors.AddRange(_srcVisistors);
                    CurrentVisitors = Visistors.Count;
                }
                //Console.WriteLine("test:"+w.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsCheckedBusyBox = false;
                // Console.WriteLine("test111:" + w.ElapsedMilliseconds);
            }

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
                BeginUpdateData();
                NotifyChange("SearchTimeIndex");
            }
        }

        public int LevaeCount
        {
            get { return _levaeCount; }
            set
            {
                _levaeCount = value;
                NotifyChange("LevaeCount");

            }
        }

        public int VisitingCount
        {
            get { return _visitingCount; }
            set
            {
                _visitingCount = value;
                NotifyChange("VisitingCount");
            }
        }

        public int LostCount
        {
            get { return _lostCount; }
            set
            {
                _lostCount = value;
                NotifyChange("LostCount");
            }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                NotifyChange("TotalCount");
            }
        }



        public bool IsCheckedBusyBox
        {
            get { return _isCheckedBusyBox; }
            set
            {
                _isCheckedBusyBox = value;
                NotifyChange("IsCheckedBusyBox");
            }
        }

        public bool IsAutoGenerateWay
        {
            get { return _isAutoGenerateWay; }
            set
            {
                _isAutoGenerateWay = value;
                BeginUpdateData();
                NotifyChange("TotalCount");
            }
        }

        public int NocomebackCount
        {
            get { return _nocomebackCount; }
            set
            {
                _nocomebackCount = value;
                NotifyChange("NocomebackCount");
            }
        }

        public int NocomebackInTimeCount
        {
            get { return _nocomebackInTimeCount; }
            set
            {
                _nocomebackInTimeCount = value;
                NotifyChange("NocomebackInTimeCount");

            }
        }

        private void StatusCommand(object arg)
        {
            _statusIndex = (Status)int.Parse(arg.ToString());
            BeginUpdateData();
        }

        private void SearchCommand(object arg)
        {
            UpdateDataCore();
            UpdateCountCore();
        }

        private void ClearCommand(object arg)
        {
            DepartmentIndex = -1;
            EmployeeIndex = -1;
        }
    }
}
