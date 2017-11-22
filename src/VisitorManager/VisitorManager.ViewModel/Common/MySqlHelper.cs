using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using Dapper;
using System.Data;

namespace VisitorManager.ViewModel
{
    public static class MySqlHelper
    {
        static MySqlConnection _conn;
        /// <summary>
        /// 初始化用，初始化数据库连接
        /// </summary>
        /// <param name="connectStr">"Database=数据库名字;Data Source=服务器的ip地址;User Id=用户名;Password=用户密码"</param>
        /// <returns></returns>
        public static bool InitSqlserver(string connectStr)
        {
            try
            {
                _conn = new MySqlConnection(connectStr);
                _conn.Open();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 写入备份表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InsertModelBack<T>(T model, out string error)
        {
            error = "";
            StringBuilder values = new StringBuilder(" VALUES(");
            StringBuilder insert = new StringBuilder(string.Format("INSERT INTO {0}( ", typeof(T).Name));
            System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
            for (int i = 0; i < proerties.Length; i++)
            {
                insert.Append(i == proerties.Length - 1 ? string.Format("{0}) ", proerties[i].Name) : string.Format("{0}, ", proerties[i].Name));
                values.Append(i == proerties.Length - 1 ? string.Format("@{0}) ", proerties[i].Name) : string.Format("@{0}, ", proerties[i].Name));
            }
            insert.Append(values);
            try
            {
                _conn.Execute(insert.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 获取表内容
        /// </summary>
        /// <param name="tablename">表名称</param>
        /// <param name="wheresql">sql语句，where后面用</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<T> SelectModel<T>(string wheresql)
        {
            string query;
            if (string.IsNullOrEmpty(wheresql))
            {
                query = "SELECT  * FROM " + typeof(T).Name;
            }
            else
            {
                query = "SELECT  * FROM " + typeof(T).Name + " WHERE " + wheresql;
            }
            List<T> list = _conn.Query<T>(query).ToList();
            return list;
        }
        /// <summary>
        /// 多表联合查询，待修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="tables"></param>
        /// <param name="wheresql"></param>
        /// <returns></returns>
        public static List<T> SelectModels<T>(object[] element, object[] tables, string wheresql)
        {
            string query;
            if (string.IsNullOrEmpty(wheresql))
            {
                query = "SELECT  * FROM " + typeof(T).Name;
            }
            else
            {
                query = "SELECT  * FROM " + typeof(T).Name + " WHERE " + wheresql;
            }
            List<T> list = _conn.Query<T>(query).ToList();
            return list;
        }
        /// <summary>
        /// 备份整个数据库表
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool BackDB(string dbName, out string error)
        {
            error = "";
            string dbBackStr = string.Format(" backup database {0} to disk='{1}{2}.bak'", dbName, dbName, DateTime.Now.ToString("yyMMddhhmm"));
            try
            {
                _conn.Execute(dbBackStr);
            }
            catch (Exception ex)
            {
                error = string.Format("备份数据库失败，错误信息{0}", ex.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="model">表对象</param>
        /// <param name="primaryKey">表主键，默认根据此字段更新表</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool UpdateModel<T>(T model, string primaryKey, out string error)
        {
            error = "";
            StringBuilder update = new StringBuilder(string.Format("UPDATE {0} SET ", typeof(T).Name));

            System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
            string keyValue = "";
            for (int i = 0; i < proerties.Length; i++)
            {
                if (proerties[i].Name == primaryKey)
                {
                    continue;

                }
                if (proerties[i].PropertyType.FullName == typeof(DateTime).ToString())
                {
                    if (proerties[i].GetValue(model, null).Equals(DateTime.MinValue))
                    {
                        continue;
                    }
                }
                if (proerties[i].GetValue(model, null) == null)
                {
                    continue;
                }
                update.Append(i == proerties.Length - 1 ? string.Format("{0} =@{1} ", proerties[i].Name, proerties[i].Name) :
                    string.Format("{0} =@{1},", proerties[i].Name, proerties[i].Name));
            }
            if (update.Length - 1 == update.ToString().LastIndexOf(','))
            {
                update = update.Remove(update.Length - 1, 1);
            }
            update.Append(string.Format(" WHERE {0}=@{1}", primaryKey, primaryKey));
            try
            {
                _conn.Execute(update.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 数据库增加一条新记录，不需要写入主键的表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InsertModel<T>(T model, string primaryKey, out string error)
        {
            error = "";
            StringBuilder values = new StringBuilder(" VALUES(");
            StringBuilder insert = new StringBuilder(string.Format("INSERT INTO {0}( ", typeof(T).Name));
            System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
            string keyValue = "";
            for (int i = 0; i < proerties.Length; i++)
            {
                if (proerties[i].Name == primaryKey)
                {
                    keyValue = proerties[i].GetValue(model, null).ToString();
                }
                if (proerties[i].PropertyType.FullName == typeof(DateTime).ToString())
                {
                    if (proerties[i].GetValue(model, null).Equals(DateTime.MinValue))
                    {
                        continue;
                    }
                }
                insert.Append(i == proerties.Length - 1 ? string.Format("{0}) ", proerties[i].Name) : string.Format("{0}, ", proerties[i].Name));
                values.Append(i == proerties.Length - 1 ? string.Format("@{0}) ", proerties[i].Name) : string.Format("@{0}, ", proerties[i].Name));
            }
            insert.Append(values);
            try
            {
                _conn.Execute(insert.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 数据库增加一条新记录，需要写入主键的表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InsertModel<T>(T model, out string error)
        {
            error = "";
            StringBuilder values = new StringBuilder(" VALUES(");
            StringBuilder insert = new StringBuilder(string.Format("INSERT INTO {0}( ", typeof(T).Name));
            StringBuilder backDeleteStr = new StringBuilder(string.Format("DELETE FROM {0} WHERE ", typeof(T).Name));

            System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
            for (int i = 0; i < proerties.Length; i++)
            {

                if (proerties[i].PropertyType.FullName == typeof(DateTime).ToString())
                {
                    if (proerties[i].GetValue(model, null).Equals(DateTime.MinValue))
                    {
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        object value = proerties[i].GetValue(model, null);
                        try
                        {
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                continue;
                            }
                        }
                        catch
                        {
                            continue;
                        }

                        if (i > 0)
                        {
                            backDeleteStr.Append(string.Format(" AND {0}='{1}'", proerties[i].Name, proerties[i].GetValue(model, null)));
                        }
                        else
                        {
                            backDeleteStr.Append(string.Format(" {0}='{1}' ", proerties[i].Name, proerties[i].GetValue(model, null)));
                        }
                    }
                    catch
                    { }
                }
                insert.Append(i == proerties.Length - 1 ? string.Format("{0}) ", proerties[i].Name) : string.Format("{0}, ", proerties[i].Name));
                values.Append(i == proerties.Length - 1 ? string.Format("@{0}) ", proerties[i].Name) : string.Format("@{0}, ", proerties[i].Name));
            }
            insert.Append(values);
            try
            {
                _conn.Execute(insert.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="model">表对象</param>
        /// <param name="primaryKey">表主键，默认根据此字段更新表</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool UpdateModelEx<T>(T model, string primaryKey, string[] updateItem, out string error)
        {
            error = "";
            StringBuilder update = new StringBuilder(string.Format("UPDATE {0} SET ", typeof(T).Name));

            System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
            string keyValue = "";
            for (int i = 0; i < proerties.Length; i++)
            {
                if (proerties[i].Name == primaryKey)
                {
                    continue;

                }
                if (proerties[i].PropertyType.FullName == typeof(DateTime).ToString())
                {
                    if (proerties[i].GetValue(model, null).Equals(DateTime.MinValue))
                    {
                        continue;
                    }
                }
                if (proerties[i].GetValue(model, null) == null)
                {
                    continue;
                }
                if (!updateItem.Contains(proerties[i].Name))
                {
                    continue;
                }
                update.Append(i == proerties.Length - 1 ? string.Format("{0} =@{1} ", proerties[i].Name, proerties[i].Name) :
                    string.Format("{0} =@{1},", proerties[i].Name, proerties[i].Name));
            }
            if (update.Length - 1 == update.ToString().LastIndexOf(','))
            {
                update = update.Remove(update.Length - 1, 1);
            }
            update.Append(string.Format(" WHERE {0}=@{1}", primaryKey, primaryKey));
            try
            {
                _conn.Execute(update.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 删除表数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool DeleteModel<T>(T model, string[] primaryKey, out string error)
        {
            error = "";
            StringBuilder delete = new StringBuilder(string.Format("DELETE FROM {0} WHERE", typeof(T).Name));
            StringBuilder wheresql = new StringBuilder("");
            for (int i = 0; i < primaryKey.Length; i++)
            {
                System.Reflection.PropertyInfo pinfo = model.GetType().GetProperties().ToList().Find(x => x.Name == primaryKey[i]);

                if (i > 0)
                {
                    delete.Append(string.Format("AND {0}=@{1} ", primaryKey[i], primaryKey[i]));
                }
                else
                {
                    delete.Append(string.Format(" {0}=@{1} ", primaryKey[i], primaryKey[i]));
                }
            }
            try
            {
                _conn.Execute(delete.ToString(), model);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 获取下一个主键ID，micType值等EMP主键Id=2
        /// </summary>
        /// <returns></returns>
        public static long GetNextId(long micType, out string errorcode)
        {
            try
            {
                var parems = new DynamicParameters();//建立一个parem对象
                parems.Add("@micType", micType);
                parems.Add("@nextID", 0, DbType.Int64, ParameterDirection.Output);//输出返回值
                parems.Add("@errorCode", "", DbType.String, ParameterDirection.Output);//输出返回值.
                _conn.Execute("sp_GetNextID", parems, null, null, CommandType.StoredProcedure);
                long res = parems.Get<long>("@nextID");//获取数据库输出的值
                errorcode = parems.Get<string>("@errorCode");//获取数据库输出的值
                return res;
            }
            catch (Exception ex)
            {
                errorcode = ex.ToString();
                return -1;
            }
        }
        public enum DBOpRator
        {
            Add,
            Modify,
            Delete
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public static void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
        }
    }
}
