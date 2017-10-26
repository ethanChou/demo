using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace VisitorManager
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Singleton = this;
        }

        public static MainWindowViewModel Singleton { get; set; }
        private bool _isCheckedVisitor = true;
        private bool _isCheckedRegister = false;
        private bool _isCheckedLeave = false;
        private bool _isCheckedStatis = false;
        private bool _isCheckedSearch = false;

        private Visibility _visitorVis= Visibility.Visible;
        private Visibility _registerVis= Visibility.Collapsed;
        private Visibility _exitVis = Visibility.Collapsed;
        private Visibility _statisticVis = Visibility.Collapsed;
        private Visibility _searchVis = Visibility.Collapsed;

        private ICommand _tabCmd;

        public ICommand TabCmd
        {
            get { return _tabCmd ?? (_tabCmd = new DelegateCommand(TabCommand)); }
        }

        public Visibility VisitorVis
        {
            get { return _visitorVis; }
            set
            {
                _visitorVis = value;
                NotifyChange("VisitorVis");
            }
        }

        public Visibility RegisterVis
        {
            get { return _registerVis; }
            set
            {
                _registerVis = value;
                NotifyChange("RegisterVis");
                
            }
        }

        public Visibility ExitVis
        {
            get { return _exitVis; }
            set
            {
                _exitVis = value;
                NotifyChange("ExitVis");
                
            }
        }

        public Visibility StatisticVis
        {
            get { return _statisticVis; }
            set
            {
                _statisticVis = value;
                NotifyChange("StatisticVis");
            }
        }

        public Visibility SearchVis
        {
            get { return _searchVis; }
            set
            {
                _searchVis = value;
                NotifyChange("SearchVis");
            }
        }

        public bool IsCheckedVisitor
        {
            get { return _isCheckedVisitor; }
            set
            {
                _isCheckedVisitor = value;
                NotifyChange("IsCheckedVisitor");
            }
        }

        public bool IsCheckedRegister
        {
            get { return _isCheckedRegister; }
            set
            {
                _isCheckedRegister = value;
                NotifyChange("IsCheckedRegister");
                
            }
        }

        public bool IsCheckedLeave
        {
            get { return _isCheckedLeave; }
            set
            {
                _isCheckedLeave = value;
                NotifyChange("IsCheckedLeave");
                
            }
        }

        public bool IsCheckedStatis
        {
            get { return _isCheckedStatis; }
            set
            {
                _isCheckedStatis = value;
                NotifyChange("IsCheckedStatis");
                
            }
        }

        public bool IsCheckedSearch
        {
            get { return _isCheckedSearch; }
            set
            {
                _isCheckedSearch = value;
                NotifyChange("IsCheckedSearch");
                
            }
        }

        private void TabCommand(object arg)
        {
            int index = int.Parse(arg.ToString());
            if (index==0)
            {
                VisitorVis = Visibility.Visible;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Collapsed;
                IsCheckedVisitor = true;
            }

            if (index == 1)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Visible;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Collapsed;
                IsCheckedRegister = true;

            }

            if (index == 2)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Visible;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Collapsed;
                IsCheckedLeave = true;
            }

            if (index == 3)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Visible;
                SearchVis = Visibility.Collapsed;
                IsCheckedStatis = true;
            }

            if (index == 4)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Visible;
                IsCheckedSearch = true;
            }
        }
    }
}
