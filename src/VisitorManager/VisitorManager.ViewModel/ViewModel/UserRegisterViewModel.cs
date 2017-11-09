﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NLog;
using ThriftCommon;
using WPF.Extend;
using System.Text.RegularExpressions;

namespace VisitorManager.ViewModel
{
    public class UserRegisterViewModel : ViewModelBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly MainWindowViewModel MainVM;
        /// <summary>
        /// 等待进入
        /// </summary>
        private ObservableCollection<Visitor> _waitVisitors;
        /// <summary>
        /// 近期采访记录
        /// </summary>
        private ObservableCollection<Visitor> _recentlyVisitors;
        /// <summary>
        /// 暂存待办
        /// </summary>
        private ObservableCollection<Visitor> _temporaryVisitors;

        private TreeNode _currentNode;
        private List<string> _objectList = new List<string>();
        private string _objectStr;
        private List<TreeNode> _items;
        private ICommand _captureCmd;
        private ICommand _addCmd;
        private ICommand _saveTempCmd;
        private ICommand _submitCmd;
        private ICommand _clearCmd;
        private ICommand _selectedCmd;
        private ICommand _selectedTreeNodeCmd;
        private static readonly string DefaultImageSrc = AppDomain.CurrentDomain.BaseDirectory + "Image\\Msg\\TransparentImg.png";
        private bool _isIDCardCheck = true;
        private bool _isCommonCardCheck = false;
        private string _captureImageSrc = DefaultImageSrc;
        private List<TreeNode> _srcItems;
        private TreeNodeCollection _nodesCollection = new TreeNodeCollection();
        private string _visitinglistID = "";
        private bool _isShowVisitinglistId = false;
        private string _visitingListId;

        #region 身份证信息

        private string _cardId = "";
        private string _visitorName = "";
        private bool _gender = false;
        private string _cardImgPath = DefaultImageSrc;
        /// <summary>
        /// 临时卡
        /// </summary>
        private string _passCardId = "";

        private IDispatcher _disp;

        #endregion

        public UserRegisterViewModel()
            : this(null)
        { }

        public UserRegisterViewModel(MainWindowViewModel parent)
        {
            MainVM = parent;

            _waitVisitors = new ObservableCollection<Visitor>();
            _recentlyVisitors = new ObservableCollection<Visitor>();
            _temporaryVisitors = new ObservableCollection<Visitor>();

            _objectList.AddRange(new List<string>() { "包", "箱子", "笔记本", "手机", "U盘", "移动硬盘", "其他" });

            ThriftManager.GetNodes();

            SrcItems = ThriftManager.All;
            //已经排序过了
            Items = ThriftManager.Tree;
            NodesCollection.AddRange(Items);
        }

        public Type CaptureWindowType { get; set; }

        public ICommand CaptureCmd
        {
            get { return _captureCmd ?? (_captureCmd = new DelegateCommand(CaptureCommand)); }
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

        public ICommand SelectedCmd
        {
            get { return _selectedCmd ?? (_selectedCmd = new DelegateCommand(SelectedCommand)); }
        }

        /// <summary>
        /// 提交按钮是否可用
        /// </summary>
        public bool EnableSubmit
        {
            get
            {
                if (WaitVisitors.Count > 0) return true;
                return false;
            }
        }

        public int CardIdType
        {
            get
            {
                return IsIdCardCheck ? 0 : 1;
            }

            set
            {
                var v = value;
                if (v == 0) IsIdCardCheck = true;
                else IsCommonCardCheck = true;
            }
        }

        public string CardId
        {
            get { return _cardId; }
            set
            {
                bool f = _cardId == value;

                _cardId = value;
                NotifyChange("CardId");
                if (!string.IsNullOrEmpty(_cardId) && !f)
                {
                    //刷最新来访信息
                    var vs = ThriftManager.GetVisitors(
                        String.Empty, String.Empty, String.Empty,
                        (IdentifyType)(CardIdType), String.Empty, _cardId, 0, 0,
                        Status.None,
                        String.Empty, String.Empty);
                    try
                    {
                        vs.Sort(new TimeComparer(false));
                        this.MainVM.MainWindow.Dispatcher.Invoke(new Action(() =>
                        {
                            RecentlyVisitors.Clear();
                            vs.ForEach(t => { RecentlyVisitors.Add(t); });
                        }));
                    }
                    catch (Exception)
                    {
                        
                        
                    }
                }
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

        public bool Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                NotifyChange("Gender");

            }
        }

        private string GenderStr
        {
            get { return _gender ? "男" : "女"; }

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

        /// <summary>
        /// 是否显示来访单编号
        /// </summary>
        public bool IsShowVisitinglistId
        {
            get { return _isShowVisitinglistId; }
            set
            {
                _isShowVisitinglistId = value;
                NotifyChange("IsShowVisitinglistId");
            }
        }

        /// <summary>
        /// 来访单号
        /// </summary>
        public string VisitinglistId
        {
            get { return _visitinglistID; }
            set
            {
                _visitinglistID = value;

                IsShowVisitinglistId = !string.IsNullOrEmpty(_visitinglistID);

                NotifyChange("VisitinglistId");
            }
        }

        public List<TreeNode> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyChange("Items");
            }
        }

        public ObservableCollection<Visitor> RecentlyVisitors
        {
            get
            {
                return _recentlyVisitors;
            }

            set
            {
                _recentlyVisitors = value;
                NotifyChange("RecentlyVisitors");
            }
        }

        public string CardImgPath
        {
            get
            {
                return _cardImgPath;
            }

            set
            {
                _cardImgPath = value;
                if (string.IsNullOrEmpty(_cardImgPath))
                {
                    _cardImgPath = DefaultImageSrc;
                }
                NotifyChange("CardImgPath");
            }
        }

        public string CaptureImageSrc
        {
            get { return _captureImageSrc; }
            set
            {
                _captureImageSrc = value;
                if (string.IsNullOrEmpty(_captureImageSrc))
                {
                    _captureImageSrc = DefaultImageSrc;
                }
                NotifyChange("CaptureImageSrc");
            }
        }

        public TreeNodeCollection NodesCollection
        {
            get
            {
                return _nodesCollection;
            }

            set
            {
                _nodesCollection = value;
            }
        }

        public ICommand SelectedTreeNodeCmd
        {
            get
            {
                return _selectedTreeNodeCmd ?? (_selectedTreeNodeCmd = new DelegateCommand(SelectedTreeNodeCommand));
            }
        }

        private ICommand _deleteSelectedNodeCmd;

        private TreeNode _currentNodeCbx;
        public TreeNode CurrentNodeCbx
        {
            get { return _currentNodeCbx; }
            set
            {
                _currentNodeCbx = value;
                if (value != null)
                {
                    _currentNode = value;
                    NotifyChange("CurrentNode");
                }
            }
        }

        public TreeNode CurrentNode
        {
            get
            {
                return _currentNode;
            }
            set
            {
                _currentNode = value;
                NotifyChange("CurrentNode");
                if (_currentNode == null) return;
                if (_currentNode.Type == 0) //机构节点
                {
                    for (int i = 0; i < this.NodesCollection.Count; i++)
                    {
                        if (this.NodesCollection[i].ID == _currentNode.ID)
                        {
                            this.NodesCollection.Index = i;
                            if (this.NodesCollection.Childrens.Count > 0)
                                this.NodesCollection.IndexForChilds = 0;
                        }
                    }
                }
                if (_currentNode.Type == 1) //人员节点
                {
                    for (int i = 0; i < this.NodesCollection.Count; i++)
                    {
                        if (this.NodesCollection[i].ID == _currentNode.ParentID)
                        {
                            this.NodesCollection.Index = i;
                            if (this.NodesCollection.Childrens.Count > 0)
                            {
                                for (int j = 0; j < this.NodesCollection.Childrens.Count; j++)
                                {
                                    if (this.NodesCollection.Childrens[j].ID == _currentNode.ID)
                                    {
                                        this.NodesCollection.IndexForChilds = j;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public ICommand DeleteSelectedNodeCmd
        {
            get
            {
                return _deleteSelectedNodeCmd ?? (_deleteSelectedNodeCmd = new DelegateCommand(DeleteSelectedNodeCommand));
            }
            set
            {
                _deleteSelectedNodeCmd = value;
            }
        }

        public Visibility DeleteSelectedNodeBtnVis
        {
            get
            {
                return _deleteSelectedNodeBtnVis;
            }
            set
            {
                _deleteSelectedNodeBtnVis = value;
                NotifyChange("DeleteSelectedNodeBtnVis");
            }
        }

        public bool IsIdCardCheck
        {
            get { return _isIDCardCheck; }
            set
            {
                _isIDCardCheck = value;
                NotifyChange("IsIdCardCheck");
            }
        }

        public bool IsCommonCardCheck
        {
            get { return _isCommonCardCheck; }
            set
            {
                _isCommonCardCheck = value;
                NotifyChange("IsCommonCardCheck");

            }
        }

        /// <summary>
        /// 携带物品
        /// </summary>
        public List<string> ObjectList
        {
            get { return _objectList; }
            set { _objectList = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ObjectStr
        {
            get { return _objectStr; }
            set
            {
                _objectStr = value;
                NotifyChange("ObjectStr");
            }
        }

        /// <summary>
        /// 绑定框TreeView的Tag上，供数据查询使用
        /// </summary>
        public List<TreeNode> SrcItems
        {
            get
            {
                return _srcItems;
            }

            set
            {
                _srcItems = value;
            }
        }

        private Visibility _deleteSelectedNodeBtnVis = Visibility.Collapsed;

        /// <summary>
        /// 当前添加的Visitor集合
        /// </summary>
        public ObservableCollection<Visitor> WaitVisitors
        {
            get
            {
                return _waitVisitors;
            }
            set
            {
                _waitVisitors = value;
            }
        }

        public ObservableCollection<Visitor> TemporaryVisitors
        {
            get
            {
                return _temporaryVisitors;
            }
            set
            {
                _temporaryVisitors = value;
            }
        }

        internal void IDCardRecevied(IDCardData data)
        {
            if (data == null) return;

            CardIdType = 0;
            CardId = data.IdCardNO;

            VisitorName = data.Name;
            CardImgPath = data.BmpPath;
            Gender = data.Sex == "男" ? true : false;

            var res = ThriftManager.GetBlackList(data.IdCardNO);

            if (res.Count > 0)
            {
                _disp.Dispatcher.Invoke(() => { MsgBox.Show("黑名单中出现此人."); });
            }
        }

        /// <summary>
        /// 刷临时卡，更新数据
        /// </summary>
        /// <param name="id"></param>
        internal void UpdateTempCardId(string id)
        {
            //临时卡
            this.PassCardId = id;
        }

        internal void FreshCardReceived(VisitorFreshCardMessage data)
        {
            if (data == null) return;

            if (data.cardholder_type == CardHolderType.Tmproty)
            {
                //临时卡
                this.PassCardId = data.card_no;
            }
            else if (data.cardholder_type == CardHolderType.CardHolder)
            {
                bool exist = false;
                TreeNode treeNode = null;
                for (int i = 0; i < NodesCollection.Count; i++)
                {
                    if (NodesCollection[i].ID == data.cardholder.Dept)
                    {
                        NodesCollection.Index = i;

                        for (int j = 0; j < NodesCollection.Childrens.Count; j++)
                        {
                            if (NodesCollection.Childrens[j].ID == data.cardholder.ID)
                            {
                                exist = true;
                                NodesCollection.IndexForChilds = j;
                                treeNode = NodesCollection.Childrens[j];
                                break;
                            }
                        }
                    }
                }
                //如果不存在，直接当成idcard使用
                if (!exist)
                {
                    CardIdType = 1;
                    CardId = data.card_no;
                    VisitorName = data.cardholder.LastName + data.cardholder.FirstName;
                    CardImgPath = data.img_url;
                    Gender = true;
                }
                else
                {
                    //如果存在，说明是本单位员工的卡，找到对应的单位，人员选择。
                    //相当于自动选择被访单位和被访人员
                    CurrentNode = treeNode;
                    DeleteSelectedNodeBtnVis = Visibility.Visible;
                }
            }
        }

        internal void ClearVisitingId()
        {
            VisitinglistId = "";
        }

        /// <summary>
        /// 添加同行人，需要把来访单好传过来
        /// </summary>
        /// <param name="arg"></param>
        internal void AddPeerVisitor(object arg)
        {
            Visitor v = arg as Visitor;
            if (v != null)
            {
                VisitinglistId = v.Vt_vl_id;
                var res = ThriftManager.GetVisitorLists(VisitinglistId, 0, 0);
                if (res.Count > 0)
                {
                    ObjectStr = res[0].Vl_carryThings;
                }
            }
        }

        /// <summary>
        /// 删除等待列表里面的Visitor
        /// </summary>
        /// <param name="arg"></param>
        internal void DeleteWaitVisitor(object arg)
        {
            Visitor v = arg as Visitor;
            if (v != null)
            {
                if (MsgBox.Show("是否立即删除？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    WaitVisitors.Remove(v);
                }
            }
        }

        /// <summary>
        /// 删除暂存列表里面的Visistor
        /// </summary>
        /// <param name="obj"></param>
        internal void DeleteTempVisitor(object obj)
        {
            Visitor v = obj as Visitor;
            if (v != null)
            {
                TemporaryVisitors.Remove(v);
            }
        }

        /// <summary>
        /// 来访单每次的ID,每次都会生成新的一个id。
        /// </summary>
        private string GetNewVisitingListID()
        {
            DateTime time = DateTime.Now;
            _visitingListId = time.ToString("yyyyMMddhhMMss");
            return _visitingListId;
        }

        /// <summary>
        /// 选择中暂存列表里面的项，重新编辑
        /// </summary>
        /// <param name="arg"></param>
        private void SelectedCommand(object arg)
        {
            Visitor visitor = arg as Visitor;
            if (visitor != null)
            {
                CardImgPath = visitor.Vt_identify_imgurl;
                CaptureImageSrc = visitor.Vt_imgurl;
                CardIdType = (int)visitor.Vt_identify_type;
                CardId = visitor.Vt_identify_no;
                VisitorName = visitor.Vt_name;
                Gender = visitor.Vt_sex == "男" ? true : false;

                PassCardId = visitor.Tmpcard_no;

                var dep = NodesCollection.FirstOrDefault(t => t.ID == visitor.Vt_visit_department_id);
                if (dep != null)
                {
                    int index = NodesCollection.IndexOf(dep);
                    NodesCollection.Index = index;
                }

                var emp = NodesCollection.Childrens.FirstOrDefault(t => t.ID == visitor.Vt_visit_employee_id);
                if (emp != null)
                {
                    int index = NodesCollection.Childrens.IndexOf(emp);
                    NodesCollection.IndexForChilds = index;
                }
            }
        }

        private void CaptureCommand(object arg)
        {
            if (CaptureWindowType != null)
            {
                ICaptureWindow window = (ICaptureWindow)Activator.CreateInstance(CaptureWindowType);
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    CaptureImageSrc = window.CaptureImagePath;
                }
                window = null;
            }
        }

        private void AddCommand(object arg)
        {

            if (string.IsNullOrEmpty(CardId))
            {
                var result = MsgBox.Show("证件号码不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (CardId != null && CardIdType == 0)
            {
                if ((!Regex.IsMatch(CardId, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
                {
                    MsgBox.Show("请输入正确的身份证号码.", "提示");
                    return;
                }
            }

            if (string.IsNullOrEmpty(VisitorName))
            {
                var result = MsgBox.Show("姓名不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(PassCardId))
            {
                var result = MsgBox.Show("通行证编号不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CaptureImageSrc) || CaptureImageSrc == DefaultImageSrc)
            {
                var result = MsgBox.Show("请抓拍来访者照片.", "提示", MessageBoxButton.OK);
                return;
            }

            if (WaitVisitors.ToList().Find(t => t.Vt_identify_no == CardId) != null)
            {
                MsgBox.Show("相同证件号码不能添加两次.", "提示", MessageBoxButton.OK);
                return;
            }

            if (CurrentNode == null)
            {
                MsgBox.Show("请选择被访人员.", "提示", MessageBoxButton.OK);
                return;
            }

            if (CurrentNode.Type == 0)
            {
                MsgBox.Show("请选择人员节点.", "提示", MessageBoxButton.OK);
                return;
            }

            Visitor visitor = this.TemporaryVisitors.FirstOrDefault(t => t.Vt_identify_no == CardId);
            if (visitor != null)
            {
                //之前已经暂存过，需要删除
                TemporaryVisitors.Remove(visitor);
            }

            visitor = new Visitor()
            {
                Vt_id = Guid.NewGuid().ToString(),
                Vt_identify_imgurl = CardImgPath,
                Vt_identify_type = (IdentifyType)CardIdType,
                Vt_identify_no = CardId,
                Vt_imgurl = CaptureImageSrc,
                Vt_name = VisitorName,
                Vt_sex = GenderStr,
                Vt_visit_department_id = CurrentNode.ParentID,
                Vt_visit_employee_id = CurrentNode.ID,
                Vt_in_time = DateTime.Now.Ticks,
                Tmpcard_no = PassCardId,
            };

            WaitVisitors.Add(visitor);

            NotifyChange("EnableSubmit");

            Reset(false);
        }

        private void SaveTempCommand(object arg)
        {
            if (string.IsNullOrEmpty(CardId))
            {
                var result = MsgBox.Show("证件号码不能为空.", "提示", MessageBoxButton.OK);
                return;
            }

            if (WaitVisitors.FirstOrDefault(v => v.Vt_identify_no == CardId) != null)
            {
                var result = MsgBox.Show("来访者已存在,不可暂存.", "提示", MessageBoxButton.OK);
                return;
            }

            Visitor visitor = TemporaryVisitors.FirstOrDefault(v => v.Vt_identify_no == CardId);
            //存在的话，把之前的全部删除，然后重新取最新数据添加
            if (visitor != null)
                TemporaryVisitors.Remove(visitor);

            visitor = new Visitor()
            {
                Vt_id = Guid.NewGuid().ToString(),
                Vt_identify_imgurl = CardImgPath,
                Vt_identify_type = (IdentifyType)(CardIdType),
                Vt_identify_no = CardId,
                Vt_imgurl = CaptureImageSrc,
                Vt_in_time = DateTime.Now.Ticks,
                Tmpcard_no = PassCardId,
                Vt_name = VisitorName,
                Vt_sex = GenderStr,
            };

            if (CurrentNode != null)
            {
                if (CurrentNode.Type == 0)
                {
                    visitor.Vt_visit_department_id = CurrentNode.ID;
                }
                if (CurrentNode.Type == 1)
                {
                    visitor.Vt_visit_department_id = CurrentNode.ParentID;
                    visitor.Vt_visit_employee_id = CurrentNode.ID;
                }
            }
            TemporaryVisitors.Add(visitor);
        }

        private void SubmitCommand(object arg)
        {
            //如果存在来访单号，合并
            string id = string.IsNullOrEmpty(VisitinglistId) ? GetNewVisitingListID() : VisitinglistId;
            foreach (var v in WaitVisitors)
            {
                //所有来访单号，在提交的时候才确认生成。
                v.Vt_vl_id = id;
                //状态修改成已进入状态
                v.Vt_status = Status.Visiting;

                if (MainVM != null)
                {
                    MainVM.VistingVM.UpdateVisitor(v);
                }
            }

            var result = ThriftManager.GetVisitorLists(id, 0, 0);
            logger.Info(string.Format("GetVisitorLists ID {0},Count {1}", id, result.Count));
            bool flag = false;
            if (result.Count == 0)
            {
                // 来访单
                VisitorList vl = new VisitorList();
                {
                    vl.Vl_id = id;
                    vl.Vl_in_time = DateTime.Now.Ticks;
                    vl.Vl_carryThings = ObjectStr;
                }
                flag = ThriftManager.AddVisitorList(vl);
                if (!flag)
                {
                    MsgBox.Show("提交来访单到服务失败.", "提示", MessageBoxButton.OK);
                    return;
                }
            }
            else
            {
                VisitorList vl = result[0];
                vl.Vl_carryThings = ObjectStr;
                ThriftManager.UpdateVisitorList(vl);
            }
            flag = ThriftManager.AddVisitor(WaitVisitors.ToList());
            if (!flag)
            {
                MsgBox.Show("提交访问者到服务失败.", "提示", MessageBoxButton.OK);
                return;
            }
            Reset();
            ObjectStr = "";
        }

        private void DeleteSelectedNodeCommand(object arg)
        {
            CurrentNode = null;
            DeleteSelectedNodeBtnVis = Visibility.Collapsed;
        }

        private void SelectedTreeNodeCommand(object arg)
        {
            TreeNode tn = arg as TreeNode;
            if (tn != null)
            {
                CurrentNode = tn;
                DeleteSelectedNodeBtnVis = Visibility.Visible;
            }
        }

        private void ClearCommand(object arg)
        {

        }

        private void Reset(bool force = true)
        {
            CardIdType = 0;
            CardId = "";
            VisitorName = "";
            Gender = false;

            PassCardId = "";
            CardImgPath = "";
            CaptureImageSrc = "";

            //刷新guid，重新生成来访单号
            if (force)
            {
                WaitVisitors.Clear();
                DeleteSelectedNodeCommand(null);
            }
        }
    }
}
