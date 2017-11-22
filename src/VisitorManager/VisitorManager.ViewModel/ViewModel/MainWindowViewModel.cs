using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
       

        internal IContextWindow MainWindow { get; set; }

        public MainWindowViewModel(IContextWindow window)
        {
            Singleton = this;
            MainWindow = window;

            TabExitSource = (ImageSource)Application.Current.Resources["Man"];

            IDCardManager.Start();
            FreshCardManager.Start();
            ThriftManager.Start();

        }


        public void Init(UserLeaveViewModel lvm, UserRegisterViewModel rvm, UserVisitingViewModel vvm, UserSearchViewModel svm, UserStatisticViewModel stvm)
        {
            LeaveVM = lvm;
            RegisterVM = rvm;
            VistingVM = vvm;
            SearchVM = svm;
            StatisticVM = stvm;

            IDCardManager.RecevierCallback += IDCardDataRecevied;
            FreshCardManager.MessageReceived += FreshCardReceived;

            UserLeaveCommands.Selected = new Action<object>(LeaveVM.SelectedCommand);
            UserLeaveCommands.Leave = new Action<object>(VisitroLeave);

            UserRegisterCommands.PeerUserAdded += (t) =>
            {
                RegisterVM.AddPeerVisitor(t);
                TabCommand(1);
            };
            UserRegisterCommands.WaitUsersDeleted += (t) =>
            {
                RegisterVM.DeleteWaitVisitor(t);
            };
            UserRegisterCommands.TempUsersDeleted += (t) =>
            {
                RegisterVM.DeleteTempVisitor(t);
            };

            UserSearchCommands.ViewCmd = new DelegateCommand(SearchVM.ViewVisitor);

            UserVisitingCommands.Delete += VistingVM.VisitorDeleted;
            UserVisitingCommands.Info += VistingVM.VisitorInfo;
        }

        /// <summary>
        /// 员工卡
        /// </summary>
        /// <param name="message"></param>
        private void FreshCardReceived(string message)
        {
            VisitorFreshCardMessage data = message.Deserialize<VisitorFreshCardMessage>();
            if (data != null)
            {
                if (data.cardholder_type == CardHolderType.Tmproty)
                {
                    var vtor = VistingVM.ExistsCard(string.IsNullOrEmpty(data.cardholder.FirstName) ? data.card_no : string.Format("{0}({1})", data.cardholder.FirstName, data.card_no));
                    //临时卡正在使用
                    if (vtor != null)
                    {
                        LeaveVM.VisitorLeave(vtor);
                        LeaveVM.ShowLostCardBtnVis = Visibility.Collapsed;
                        TabCommand(2);
                    }
                    else
                    {
                        RegisterVM.UpdateTempCardId(data);
                        TabCommand(1);
                    }
                }
                else
                {
                    //正式卡
                    var vtor = VistingVM.ExistsCard(string.IsNullOrEmpty(data.cardholder.FirstName) ? data.card_no : string.Format("{0}({1})", data.cardholder.FirstName, data.card_no), false);
                    if (vtor == null)
                    {
                        RegisterVM.FreshCardReceived(data);
                        TabCommand(1);
                    }
                    else
                    {
                        LeaveVM.VisitorLeave(vtor);
                        LeaveVM.ShowLostCardBtnVis = Visibility.Collapsed;
                        TabCommand(2);
                    }
                }
            }
        }

        /// <summary>
        /// 刷身份证卡，产生消息，更新
        /// </summary>
        /// <param name="data"></param>
        private void IDCardDataRecevied(IDCardData data)
        {
            if (!MainWindow.Dispatcher.CheckAccess())
            {
                MainWindow.Dispatcher.Invoke(() =>
                {
                    IDCardDataRecevied(data);
                });
                return;
            }

            if (data != null)
            {
                var vtor = VistingVM.ExistsCard(data.IdCardNO, false);
                if (vtor == null)
                {
                    RegisterVM.IDCardRecevied(data);
                    TabCommand(1);
                }
                else {
                    LeaveVM.VisitorLeave(vtor);
                    LeaveVM.ShowLostCardBtnVis = Visibility.Collapsed;
                    TabCommand(2);
                }
            }
        }

        /// <summary>
        /// 首页访问用户离开
        /// </summary>
        /// <param name="arg"></param>
        private void VisitroLeave(object arg)
        {
            Visitor v = arg as Visitor;

            if (v != null)
            {
                LeaveVM.VisitorLeave(v);
                LeaveVM.ShowLostCardBtnVis = Visibility.Visible;
                TabCommand(2);
            }
        }

        #region MainWindow
        public static MainWindowViewModel Singleton { get; set; }
        private bool _isCheckedVisitor = true;
        private bool _isCheckedRegister = false;
        private bool _isCheckedLeave = false;
        private bool _isCheckedStatis = false;
        private bool _isCheckedSearch = false;

        private Visibility _visitorVis = Visibility.Visible;
        private Visibility _registerVis = Visibility.Collapsed;
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
            if (index == 0)
            {
                VisitorVis = Visibility.Visible;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Collapsed;
                IsCheckedVisitor = true;
                RegisterVM.ClearVisitingId();
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
                RegisterVM.ClearVisitingId();

            }

            if (index == 3)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Visible;
                SearchVis = Visibility.Collapsed;
                IsCheckedStatis = true;
                RegisterVM.ClearVisitingId();

            }

            if (index == 4)
            {
                VisitorVis = Visibility.Collapsed;
                RegisterVis = Visibility.Collapsed;
                ExitVis = Visibility.Collapsed;
                StatisticVis = Visibility.Collapsed;
                SearchVis = Visibility.Visible;
                IsCheckedSearch = true;
                RegisterVM.ClearVisitingId();

            }
        }

        ImageSource _tabVisitingSource;
        ImageSource _tabRegisterSource;
        ImageSource _tabExitSource;
        ImageSource _tabSearchSource;
        ImageSource _tabStatisticSource;

        public ImageSource TabVisitingSource
        {
            get
            {
                return _tabVisitingSource;
            }

            set
            {
                _tabVisitingSource = value;
                NotifyChange("TabVisitingSource");

            }
        }

        public ImageSource TabRegisterSource
        {
            get
            {
                return _tabRegisterSource;
            }

            set
            {
                _tabRegisterSource = value;
                NotifyChange("TabRegisterSource");

            }
        }

        public ImageSource TabExitSource
        {
            get
            {
                return _tabExitSource;
            }

            set
            {
                _tabExitSource = value;
                NotifyChange("TabExitSource");

            }
        }

        public ImageSource TabSearchSource
        {
            get
            {
                return _tabSearchSource;
            }

            set
            {
                _tabSearchSource = value;
                NotifyChange("TabSearchSource");

            }
        }

        public ImageSource TabStatisticSource
        {
            get
            {
                return _tabStatisticSource;
            }

            set
            {
                _tabStatisticSource = value;
                NotifyChange("TabStatisticSource");

            }
        }
        #endregion

        private UserLeaveViewModel _leaveVM;
        private UserRegisterViewModel _registerVM;
        private UserSearchViewModel _searchVM;
        private UserStatisticViewModel _statisticVM;
        private UserVisitingViewModel _vistingVM;

        public UserLeaveViewModel LeaveVM
        {
            get
            {
                return _leaveVM;
            }

            set
            {
                _leaveVM = value;
            }
        }

        public UserRegisterViewModel RegisterVM
        {
            get
            {
                return _registerVM;
            }

            set
            {
                _registerVM = value;
            }
        }

        public UserVisitingViewModel VistingVM
        {
            get
            {
                return _vistingVM;
            }

            set
            {
                _vistingVM = value;
            }
        }

        public UserStatisticViewModel StatisticVM
        {
            get
            {
                return _statisticVM;
            }

            set
            {
                _statisticVM = value;
            }
        }

        public UserSearchViewModel SearchVM
        {
            get
            {
                return _searchVM;
            }

            set
            {
                _searchVM = value;
            }
        }
    }
}
