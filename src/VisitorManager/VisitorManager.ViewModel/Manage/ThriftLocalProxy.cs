using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Internal;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class ThriftLocalProxy : LenelDataService.Iface
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private string error;
        private string _bimgAddress;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectStr">连接字符串;Database=数据库名字;Data Source=服务器的ip地址;User Id=用户名;Password=用户密码</param>
        /// <param name="bimgAddress">bimg地址如：http://127.0.0.1:6551</param>

        public ThriftLocalProxy(string connectStr = "Database=vsm;Data Source=127.0.0.1;User Id=root;Password=root", string bimgAddress = "http://127.0.0.1:10551")
        {
            if (!MySqlHelper.InitSqlserver(connectStr))
                logger.Info("数据库初始化连接失败。。。");//初始化数据库
            _bimgAddress = bimgAddress;
        }
        public bool AddVisitor(List<Visitor> visitors)
        {
            try
            {
                visitors.ForEach((x) =>
                {
                    MySqlHelper.InsertModel(x, "vt_id", out error);

                });
            }
            catch (Exception ex)
            {
                logger.Info(ex.ToString());
                return false;
            }
            return true;
        }

        public bool AddVisitorList(VisitorList vlist)
        {
            return MySqlHelper.InsertModel(vlist, "vl_id", out error);
        }
        private bool Oprator<T>(T data, DbOprator dboprator, string primarykey)
        {
            switch (dboprator)
            {
                case DbOprator.Add:
                    return MySqlHelper.InsertModel(data, primarykey, out error);
                case DbOprator.Update:
                    return MySqlHelper.UpdateModel(data, primarykey, out error);
                case DbOprator.Delete:
                    return MySqlHelper.UpdateModel(data, primarykey, out error);
                default:
                    return true;
            }
        }
        public bool AdrelationOprator(DbOprator dboprator, Adrelation adrelation)
        {
            if (dboprator == DbOprator.Delete)
            {
                adrelation.Ad_isdeleted = true;
            }
            return Oprator(adrelation, dboprator, "ad_id");
        }

        public bool BlackListOprator(DbOprator dboprator, BlackList blacklist)
        {
            if (dboprator == DbOprator.Delete)
            {
                blacklist.Bl_isdeleted = true;
            }
            return Oprator(blacklist, dboprator, "bl_id");
        }

        public bool DepartmentOprator(DbOprator dboprator, Department department)
        {
            if (dboprator == DbOprator.Delete)
            {
                department.Dep_isdeleted = true;
            }
            return Oprator(department, dboprator, "dep_id");
        }

        public bool EmployeeOprator(DbOprator dboprator, Employee employee)
        {
            if (dboprator == DbOprator.Delete)
            {
                employee.Emp_isdeleted = true;
            }
            return Oprator(employee, dboprator, "emp_id");
        }

        public List<Adrelation> GetAdralations(string id, string lnl_id, string name)
        {
            string sql = " ad_isdeleted!=1";
            if (!string.IsNullOrEmpty(id))
            {
                sql += " and ad_id='" + id + "'";
            }
            if (!string.IsNullOrEmpty(lnl_id))
            {
                sql += " and lnl_id='" + lnl_id + "'";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and ad_name='" + name + "'";
            }
            return MySqlHelper.SelectModel<Adrelation>(sql);
        }

        public List<BlackList> GetBlackList(IdentifyType identify_type, string bl_identify_NO)
        {
            return GetBlackList("", identify_type, bl_identify_NO, "");
        }

        public List<BlackList> GetBlackList(string bl_id, IdentifyType identify_type, string bl_identify_NO, string name)
        {
            string sql = " bl_isdeleted!=1";
            if (!string.IsNullOrEmpty(bl_id))
            {
                sql += " and bl_id='" + bl_id + "'"; ;
            }
            if (!string.IsNullOrEmpty(bl_identify_NO))
            {
                sql += " and bl_identify_type='" + (int)identify_type + "' and bl_identify_NO='" + bl_identify_NO + "'";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and bl_name='" + name + "'";
            }
            return MySqlHelper.SelectModel<BlackList>(sql);
        }

        public List<Department> GetDepartments(string ad_id, string id, string lnl_id, string name)
        {
            string sql = " dep_isdeleted!=1 ";
            if (!string.IsNullOrEmpty(ad_id))
            {
                sql += " and ad_id='" + ad_id + "'";
            }
            if (!string.IsNullOrEmpty(id))
            {
                sql += " and dep_id='" + id + "'";
            }
            if (!string.IsNullOrEmpty(lnl_id))
            {
                sql += " and lnl_id='" + lnl_id + "'";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and dep_name='" + name + "'";
            }
            return MySqlHelper.SelectModel<Department>(sql);
        }

        public List<Employee> GetEmployees(string dep_id, string id, string lnl_id, string name, string card_NO)
        {
            string sql = " emp_isdeleted!=1 ";
            if (!string.IsNullOrEmpty(dep_id))
            {
                sql += " and dep_id='" + dep_id + "'";
            }
            if (!string.IsNullOrEmpty(id))
            {
                sql += " and emp_id='" + id + "'";
            }
            if (!string.IsNullOrEmpty(lnl_id))
            {
                sql += " and lnl_id='" + lnl_id + "'";
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql += " and emp_name LIKE '%" + name + "%'";
            }
            if (!string.IsNullOrEmpty(card_NO))
            {
                sql += " and card_no='" + card_NO + "'";
            }
            return MySqlHelper.SelectModel<Employee>(sql);
        }

        public List<VisitorList> GetVisitorLists(string vl_id, long vl_in_time, long vl_out_time)
        {
            string sql = " vl_id!='' ";
            if (!string.IsNullOrEmpty(vl_id))
            {
                sql += " and vl_id='" + vl_id + "'";
            }
            if (vl_in_time != 0)
            {
                sql += " and vl_in_time<=" + new DateTime(vl_in_time);
            }
            if (vl_out_time != 0)
            {
                sql += " and vl_out_time>=" + new DateTime(vl_out_time);
            }
            return MySqlHelper.SelectModel<VisitorList>(sql);
        }

        public List<Visitor> GetVisitors(string vt_id, string vt_vl_id, string name, IdentifyType identify_type, string vt_identify_NO, string tmpcard_no, long in_time, long out_time, Status status, string dep_id, string emp_id)
        {
            string sql = "vt_id!='' ";
            if (!string.IsNullOrEmpty(vt_id))
            {
                sql += " and vt_id='" + vt_id + "'";
            }
            if (!string.IsNullOrEmpty(vt_vl_id))
            {
                sql += " and vt_vl_id='" + vt_vl_id + "'";
            }
            if (!string.IsNullOrEmpty(vt_identify_NO))
            {
                sql += " and vt_identify_type='" + identify_type + "' vt_identify_NO='" + vt_identify_NO + "'";
            }
            if (!string.IsNullOrEmpty(tmpcard_no))
            {
                sql += " and tmpcard_NO='" + tmpcard_no + "'";
            }
            if (status != Status.None)
            {
                sql += " and vt_status='" + (int)status + "'";
            }
            if (in_time != 0)
            {
                sql += " and vt_in_time>=" + in_time;
            }
            if (out_time != 0)
            {
                sql += " and vt_out_time<=" + out_time;
            }
            if (!string.IsNullOrEmpty(dep_id) && dep_id != "0")
            {
                sql += " and vt_visit_department_id='" + dep_id + "'";
            }
            if (!string.IsNullOrEmpty(emp_id) && emp_id != "0")
            {
                sql += " and vt_visit_employee_id='" + emp_id + "'";
            }

            return MySqlHelper.SelectModel<Visitor>(sql);
        }

        public bool UpdateVisitor(Visitor visitor)
        {
            return MySqlHelper.UpdateModel(visitor, "vt_id", out error);
        }

        public bool UpdateVisitorList(VisitorList vlist)
        {
            return MySqlHelper.UpdateModel(vlist, "vl_id", out error);
        }
        public string UploadImg2Bimg(byte[] imgBytes)
        {
            if (string.IsNullOrEmpty(_bimgAddress)) throw new ArgumentException("_bimgAddress is null");
            return Method.UploadImg(_bimgAddress, imgBytes);
        }

        public List<ReaderList> GetReaders()
        {
            throw new NotImplementedException();
        }


        public bool SetVisitorReader(List<ReaderList> readers)
        {
            throw new NotImplementedException();
        }

        public bool SetFaceReader(List<ReaderList> readers)
        {
            throw new NotImplementedException();
        }



        public bool DeleteVisitorList(string vl_id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteVisitor(string vt_id)
        {
            throw new NotImplementedException();
        }


        public int GetVisitorCount(Status type, long starttime, long endtime)
        {
            throw new NotImplementedException();
        }


        public DataCount GetCount(string sql)
        {
            throw new NotImplementedException();
        }
    }
}
