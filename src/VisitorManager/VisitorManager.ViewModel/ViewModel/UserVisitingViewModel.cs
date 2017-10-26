using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using VisitorManager.Model;
using System.Windows.Threading;
using System.Threading;
using VisitorManager.ViewModel.Common;

namespace VisitorManager
{

    public class UserVisitingViewModel : ViewModelBase
    {
        public static UserVisitingViewModel Single { get; set; }


        private DispatcherTimer _workTimer;
        private ObservableCollection<Visitor> _visistors;
        private int _departmentIndex = -1;
        private int _employeeIndex = -1;
        private ICommand _searchCmd;
        private int _statusIndex = 0;
        private int _currentVisitors = 0;
        public UserVisitingViewModel()
        {
            VisitorDeleteCommands.Deleted += VisitorDeleteCommands_Deleted;
            UserRegisterViewModel.NewVisitor += UserRegisterViewModel_NewVisitor;
            _visistors = new ObservableCollection<Visitor>();

            _workTimer = new DispatcherTimer();
            _workTimer.Interval = new TimeSpan(0, 0, 1);
            _workTimer.Tick += WorkTimer_Tick;
            _workTimer.Start();
        }

        void VisitorDeleteCommands_Deleted(object obj)
        {
           
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DbUtil.DB_PATH))
            {
                _workTimer.Stop();
                UpdateVisitors(StatusIndex + 1);
            }
        }

        public void UpdateData()
        {
            _workTimer.Start();
        }

       

        void UserRegisterViewModel_NewVisitor(Visitor obj)
        {
            Visistors.Add(obj);
        }

        private void UpdateVisitors(int status)
        {
            Visistors.Clear();
            string wheresql = "";
            if (status <= 5 && status >= 0)
            {
                wheresql = "vt_status==" + status;
            }

            if (DepartmentIndex >= 0)
            {
                wheresql += string.Format(" and vt_visit_department==\"{0}\"", Departments[DepartmentIndex].dep_name);
            }

            if (EmployeeIndex >= 0)
            {
                wheresql += string.Format(" and vt_visit_employee==\"{0}\"", Employees[EmployeeIndex].emp_name);
            }

            var dt = DbUtil.SelectModel<Visitor>(wheresql);
            foreach (var t in dt)
            {
                Visistors.Add(t);
                t.NoticeAll();
            }

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

        public int StatusIndex
        {
            get
            {
                return _statusIndex;
            }

            set
            {
                _statusIndex = value;
                NotifyChange("StatusIndex");
            }
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


        private void SearchCommand(object arg)
        {
            UpdateVisitors(StatusIndex + 1);
        }

        private void ClearCommand(object arg)
        {

            DepartmentIndex = -1;
            EmployeeIndex = -1;
        }
    }
}
