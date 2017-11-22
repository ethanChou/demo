using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using Thrift.Protocol;
using Thrift.Transport;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class ThriftManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static bool IsEnableProxy = false;

        private static LenelDataService.Iface _client;
        private static TTransport _transport;
        private static TProtocol _protocol;
        private static bool _isStart = false;
        private static Thread _detectThread;
        public static bool Start()
        {
            if (_isStart) return true;
            _transport = new TSocket(LocalConfig.ThriftIp, Int32.Parse(LocalConfig.ThriftPort));
            _protocol = new TBinaryProtocol(_transport);
            _detectThread = new Thread(RunDetect);

            if (IsEnableProxy)
                _client = new ThriftLocalProxy();
            else
                _client = new LenelDataService.Client(_protocol);
            try
            {
                _transport.Open();
                _detectThread.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "TSocket Open Fail");
                return false;
            }
            _isStart = true;
            return true;
        }

        private static void Check()
        {
            try
            {
                //if (_transport != null && !_transport.IsOpen)
                //{
                //    _transport.Open();
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void RunDetect()
        {
            try
            {
                DateTime dtn = DateTime.Now;
                while (true)
                {
                    DateTime dtcur = DateTime.Now;
                    if (dtcur.Hour - dtn.Hour >= 2)
                    {
                        var datas = GetVisitors(
                            String.Empty, String.Empty, String.Empty,
                            IdentifyType.IdCard, String.Empty, String.Empty, 0, 0,
                            Status.Visiting,
                            String.Empty,
                            String.Empty);
                        dtn = dtcur;
                        for (int i = 0; i < datas.Count; i++)
                        {
                            var dt = new DateTime(datas[i].Vt_in_time);
                            if ((dtcur - dt).Days >= 1)
                            {
                                //超过24时自动处理
                                datas[i].Vt_status = Status.NoComeBack;
                                bool f = UpdateVisitor(datas[i]);
                                logger.Info(string.Format("超过24时自动处理成卡未还,{0}, {1}, {2}, {3}, {4}",
                                    f, datas[i].Vt_name, datas[i].Vt_identify_no, datas[i].Tmpcard_no, datas[i].Vt_vl_id));
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        public static void Stop()
        {
            try
            {
                _transport.Close();
                _transport.Dispose();

                _protocol.Dispose();

                _transport.Dispose();

                _client = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stop");
            }
        }

        #region 数据模拟

        /// <summary>
        /// 未排序分类
        /// </summary>
        public static List<TreeNode> All { get; set; }

        /// <summary>
        /// 已排序，完整的树形结构
        /// </summary>
        public static List<TreeNode> Tree { get; set; }

        public static List<TreeNode> GetNodes()
        {
            if (_client == null) return new List<TreeNode>();
            List<TreeNode> nodes = new List<TreeNode>();
            var dps = GetDepartments("", "", "", "");

            foreach (var d in dps)
            {
                var node = new TreeNode { ID = d.Dep_id, Name = d.Dep_name, ParentID = d.Dep_parent_id };
                nodes.Add(node);
            }
            var eys = GetEmployees("", "", "", "", "");
            foreach (var e in eys)
            {
                var node = new TreeNode { ID = e.Emp_id, Name = e.Emp_name, ParentID = e.Dep_id, Type = 1, Telephone = e.Emp_tel, Tag = e };
                nodes.Add(node);
            }

            Tree = Method.Bindings(nodes);

            All = nodes;

            return nodes;
        }

        #endregion

        #region Iface

        private const string Error = "ThriftManager's LenelDataService.Iface is null,Please call ThriftManager.Start()";

        public static List<Department> GetDepartments(string ad_id, string id, string lnl_id, string name)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.GetDepartments(ad_id, id, lnl_id, name);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetDepartments");
            }
            return new List<Department>();
        }

        public static List<Employee> GetEmployees(string dep_id, string id, string lnl_id, string name, string card_NO)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.GetEmployees(dep_id, id, lnl_id, name, card_NO);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetEmployees");
            }
            return new List<Employee>();
        }

        public static bool DeleteVisitorList(string vl_id)
        {
            try
            {
                Check();

                if (_client == null) throw new NullReferenceException(Error);

                return _client.DeleteVisitorList(vl_id);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteVisitorList");
            }
            return false;
        }

        public static bool AddVisitorList(VisitorList vlist)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.AddVisitorList(vlist);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddVisitorList");
            }
            return false;
        }

        public static bool UpdateVisitorList(VisitorList vlist)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.UpdateVisitorList(vlist);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "UpdateVisitorList");
            }
            return false;
        }

        public static List<VisitorList> GetVisitorLists(string vl_id, long vl_in_time, long vl_out_time)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.GetVisitorLists(vl_id, vl_in_time, vl_out_time);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetVisitorLists");
            }
            return new List<VisitorList>();
        }

        public static bool AddVisitor(List<Visitor> visitors)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.AddVisitor(visitors);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddVisitor");
            }
            return false;
        }

        public static bool UpdateVisitor(Visitor visitor)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.UpdateVisitor(visitor);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "UpdateVisitor");
            }
            return false;
        }

        public static bool DeleteVisitor(string vt_id)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.DeleteVisitor(vt_id);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteVisitor");
            }
            return false;
        }

        public static List<Visitor> GetVisitors(long in_time, long out_time)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.GetVisitors(String.Empty, String.Empty, String.Empty, IdentifyType.Ohter, String.Empty, String.Empty, in_time, out_time, Status.None, String.Empty, String.Empty);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetVisitors");
            }
            return new List<Visitor>();
        }

        public static List<Visitor> GetVisitors(string vt_id, string vt_vl_id, string name, IdentifyType identify_type, string tmpcard_no, string vt_identify_NO, long in_time, long out_time, Status status, string dep_id, string emp_id)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);
                return _client.GetVisitors(vt_id, vt_vl_id, name, identify_type, tmpcard_no, vt_identify_NO, in_time, out_time, status, dep_id, emp_id);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetVisitors");
            }
            return new List<Visitor>();
        }


        public static List<BlackList> GetBlackList(string bl_identify_NO)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return GetBlackList("", IdentifyType.IdCard, bl_identify_NO, "");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetBlackList");
            }
            return new List<BlackList>();
        }

        public static List<BlackList> GetBlackList(string bl_id, IdentifyType identify_type, string bl_identify_NO = "", string name = "")
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.GetBlackList(bl_id, identify_type, bl_identify_NO, name);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetBlackList");
            }
            return new List<BlackList>();
        }

        /// <summary>
        /// 返回的url路径
        /// </summary>
        /// <param name="imgBytes"></param>
        /// <returns></returns>
        public static string UploadImg2Bimg(byte[] imgBytes)
        {
            try
            {
                Check();
                if (_client == null) throw new NullReferenceException(Error);

                return _client.UploadImg2Bimg(imgBytes);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "UploadImg2Bimg");
            }
            return String.Empty;
        }
        #endregion
    }
}
