using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using VisitorManager.Model;
using VisitorManager.ViewModel.Common;

namespace VisitorManager
{
    public static class DataManager
    {
        static DataManager()
        {
            UpateAll();
        }

        private static ObservableCollection<Employee> _employees;
        private static ObservableCollection<Department> _departments;
        private static ObservableCollection<Visitor> _pauseVisitors;
        private static ObservableCollection<Visitor> _waitingVisitors;
        public static ObservableCollection<Employee> Employees
        {
            get
            {
                return _employees;
            }

            set
            {
                _employees = value;
            }
        }

        public static ObservableCollection<Department> Departments
        {
            get
            {
                return _departments;
            }

            set
            {
                _departments = value;
            }
        }

        public static ObservableCollection<Visitor> PauseVisitors
        {
            get
            {
                return _pauseVisitors;
            }

            set
            {
                _pauseVisitors = value;
            }
        }

        public static ObservableCollection<Visitor> WaitingVisitors
        {
            get
            {
                return _waitingVisitors;
            }

            set
            {
                _waitingVisitors = value;
            }
        }

        public static void UpateAll()
        {
            Departments = new ObservableCollection<Department>(DbUtil.SelectModel<Department>());
            Employees = new ObservableCollection<Employee>(DbUtil.SelectModel<Employee>());
            //暂存待办
            PauseVisitors = new ObservableCollection<Visitor>(DbUtil.SelectModel<Visitor>("vt_status==4"));
            //等待进入
            WaitingVisitors = new ObservableCollection<Visitor>(DbUtil.SelectModel<Visitor>("vt_status==5"));
        }
    }
}
