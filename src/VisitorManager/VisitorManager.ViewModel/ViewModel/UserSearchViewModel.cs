using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ExcelReport;
using Microsoft.Win32;
using ThriftCommon;
using WPF.Extend;

namespace VisitorManager.ViewModel
{
    public class UserSearchViewModel : ViewModelBase
    {
        MainWindowViewModel _mainVM;
        public UserSearchViewModel()
            : this(null)
        {

        }

        public UserSearchViewModel(MainWindowViewModel parent)
        {
            _mainVM = parent;
            NodesCollection.AddRange(ThriftManager.Tree);
        }

        public Type WindowType { get; set; }
        private int _statusIndex = 0;
        private DateTime _beginTime = new DateTime(DateTime.Now.AddDays(-7).Year, DateTime.Now.AddDays(-7).Month, DateTime.Now.AddDays(-7).Day, 0, 0, 0);
        private DateTime _endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        private string _userName="";
        private ICommand _searchCmd;
        private ObservableCollection<Visitor> _resultList = new ObservableCollection<Visitor>();
        TreeNodeCollection _nodesCollection = new TreeNodeCollection();
        private TreeNode _depNode;
        private TreeNode _empNode;
        public ICommand SearchCmd
        {
            get { return _searchCmd ?? (_searchCmd = new DelegateCommand(SearchCommand)); }
        }

        public ObservableCollection<Visitor> ResultList
        {
            get { return _resultList; }
            set
            {
                _resultList = value;
                NotifyChange("ResultList");
            }
        }

        public DateTime BeginTime
        {
            get { return _beginTime; }
            set
            {
                _beginTime = value;
                NotifyChange("BeginTime");
            }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                NotifyChange("EndTime");
            }
        }

        public int StatusIndex
        {
            get { return _statusIndex; }
            set
            {
                _statusIndex = value;
                NotifyChange("StatusIndex");

            }
        }

        public TreeNodeCollection NodesCollection
        {
            get { return _nodesCollection; }
            set
            {
                _nodesCollection = value;
                NotifyChange("NodesCollection");
            }
        }

        internal void ViewVisitor(object arg)
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

        private void SearchCommand(object arg)
        {
            if ((EndTime - BeginTime).Days > 30)
            {
                MsgBox.Show("一次只能查询30天记录.");
                return;
            }
            GC.Collect();
            ResultList.Clear();
            var vs = ThriftManager.GetVisitors(
                String.Empty,
                String.Empty,
                String.Empty,
                IdentifyType.IdCard,
                String.Empty,
                String.Empty,
                new DateTime(BeginTime.Year,BeginTime.Month,BeginTime.Day,0,0,0).Ticks,
                 new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, 23, 59, 59).Ticks,
                (Status)StatusIndex,
                DepNode != null ? DepNode.ID : String.Empty,
                EmpNode != null ? EmpNode.ID : String.Empty);

           

            vs.Sort(new TimeComparer(false));

            foreach (var t in vs)
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (t.Vt_name.Contains(UserName))
                    {
                        ResultList.Add(t);
                    }
                }
                else
                {
                    ResultList.Add(t); 
                }
            }

            NotifyChange("ResultList");
        }

        private ICommand _exportCmd;
        public ICommand ExportCmd
        {
            get { return _exportCmd ?? (_exportCmd = new DelegateCommand(ExportCommand)); }
        }

        public TreeNode DepNode
        {
            get { return _depNode; }
            set { _depNode = value; }
        }

        public TreeNode EmpNode
        {
            get { return _empNode; }
            set { _empNode = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyChange("UserName");
            }
        }

        private void ExportCommand(object arg)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "数据文件（*.xlsx）|*.xlsx";
            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;
            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;
            sfd.FileName = BeginTime.ToString("yyyyMMddHHmmss");
            if (sfd.ShowDialog() == true)
            {
                List<Visitor> datas = ResultList.ToList();

                ParameterCollection collection = new ParameterCollection();
                collection.Load(@"Doc\SimpleStat.xml");

                List<ElementFormatter> tableFormatters = new List<ElementFormatter>();

                tableFormatters.Add(new TableFormatter<Visitor>(collection["访客登记表", "UserName"].X, datas,
                    new TableColumnInfo<Visitor>(collection["访客登记表", "UserName"].Y, t => t.Vt_name),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "Employee"].Y, t => GetName(t.Vt_visit_employee_id,1)),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "Department"].Y, t => GetName(t.Vt_visit_department_id,0)),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "InTime"].Y, t => DateTime.FromFileTime(t.Vt_in_time).ToString("yyyy-MM-dd HH:mm:ss")),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "OutTime"].Y, t => DateTime.FromFileTime(t.Vt_out_time).ToString("yyyy-MM-dd HH:mm:ss")),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "CardType"].Y, t => GetIdentifyTypeStr(t.Vt_identify_type)),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "CardNo"].Y, t => t.Vt_identify_no),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "TempCardNo"].Y, t => t.Tmpcard_no),
                    new TableColumnInfo<Visitor>(collection["访客登记表", "Status"].Y, t => GetStatusStr(t.Vt_status))
                    ));

                ExportHelper.ExportToLocal(@"Doc\SimpleStat.xlsx", sfd.FileName,
                      new SheetFormatterContainer("访客登记表", tableFormatters)
                      );
                MsgBox.Show("导出表格成功.", "提示");

                Process.Start("explorer.exe", "/select," + sfd.FileName);
            }
        }

        private string GetName(string id, int type)
        {
            var nodes = ThriftManager.All;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == id && nodes[i].Type == type)
                {
                    return nodes[i].Name;
                }
            }

            return "";
        }

        private string GetStatusStr(Status s)
        {
            switch (s)
            {
                case Status.Visiting:
                    return "正在访问";
                case Status.Leave:
                    return "已经离开";
                case Status.NoComeBack:
                    return "逾期未还";
                case Status.LostCard:
                    return "卡丢失";
                default:
                    return "";
            }
            return "";
        }

        private string GetIdentifyTypeStr(IdentifyType s)
        {
            switch (s)
            {
                case IdentifyType.IdCard:
                    return "二代身份证";
                    break;
                case IdentifyType.Employee:
                    return "正式卡";
                    break;
                case IdentifyType.Ohter:
                    return "其他";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("s", s, null);
            }

            return "";
        }
    }
}
