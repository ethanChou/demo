using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using VisitorManager.Model;

namespace VisitorManager.ViewModel.Common
{
    public class DbUtil
    {
        static string _sqlConnectionStr;
        public static List<string> IngnoreColumns = new List<string>();
        /// <summary>
        /// 初始化用，初始化数据库连接
        /// </summary>
        /// <param name="dbfilePath"></param>
        /// <returns></returns>
        public static void Init(string dbfilePath)
        {
            _sqlConnectionStr = string.Format("Data Source={0}", dbfilePath);
            Initialize();
        }

        /// <summary>
        /// 是否存在某列
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsExistCloumns(string schemaName, string tableName, string columnName)
        {
            using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
            {
                string querystr = string.Format("select count(*) from information_schema.columns where table_schema='{0}' and table_name='{1}' and column_name='{2}' ", schemaName, tableName, columnName);
                try
                {

                    if (conn != null)
                    {
                        if (Int32.Parse(conn.ExecuteScalar(querystr).ToString()) > 0)
                        {
                            return true;
                        }
                    }
                  
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return false;
            }
        }



        /// <summary>
        /// 获取表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wheresql"></param>
        /// <returns></returns>
        public static List<T> SelectModel<T>(string wheresql="")
        {
            using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
            {
                try
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

                    if (conn != null)
                    {
                        List<T> list = conn.Query<T>(query).ToList();
                        return list;
                    }
                  
                  
                    
                }
                catch (Exception ex)
                {
                }
                return new List<T>();
            }
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="model">表对象</param>
        /// <param name="primaryKey">表主键，默认根据此字段更新表</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool UpdateModel<T>(T model, string primaryKey )
        {
            using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
            {
                try
                {
                    StringBuilder update = new StringBuilder(string.Format("UPDATE {0} SET ", typeof(T).Name));
                    System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
                    for (int i = 0; i < proerties.Length; i++)
                    {
                        if (proerties[i].Name == primaryKey || IngnoreColumns.Contains(proerties[i].Name))
                        {
                            continue;
                        }
                        update.Append(string.Format("{0} =@{1},", proerties[i].Name, proerties[i].Name));
                    }

                    string updatestr = "";
                    if (!string.IsNullOrEmpty(primaryKey))
                        updatestr = string.Format("{0} WHERE {1}=@{2}", update.ToString().Substring(0, update.Length - 1), primaryKey, primaryKey);
                    else {
                        updatestr = update.ToString().Substring(0, update.Length - 1);
                    }
                    if (conn != null)
                    {
                        return conn.Execute(updatestr, model) > 0;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 数据库增加一条新记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool InsertModel<T>(T model, string primaryKey = "")
        {

            using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
            {
                StringBuilder values = new StringBuilder(" VALUES(");
                StringBuilder insert = new StringBuilder(string.Format("INSERT INTO {0}( ", typeof(T).Name));
                System.Reflection.PropertyInfo[] proerties = model.GetType().GetProperties();
                for (int i = 0; i < proerties.Length; i++)
                {
                    if (proerties[i].Name == primaryKey)
                    {
                        continue;
                    }
                    insert.Append(i == proerties.Length - 1 ? string.Format("{0}) ", proerties[i].Name) : string.Format("{0}, ", proerties[i].Name));
                    values.Append(i == proerties.Length - 1 ? string.Format("@{0}) ", proerties[i].Name) : string.Format("@{0}, ", proerties[i].Name));
                }
                insert.Append(values);
                try
                {
                    if (conn != null)
                    {
                        conn.Execute(insert.ToString(), model);
                    }
                   
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;// = ex.Message;
                    return false;
                }
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
        public static bool DeleteModel<T>(T model, string primaryKey = "")
        {
            using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
            {
                StringBuilder delete = new StringBuilder(string.Format("DELETE FROM {0} ", typeof(T).Name));
                delete.Append(string.Format(" WHERE {0}=@{1}", primaryKey, primaryKey));
                try
                {
                    if (conn != null)
                    {
                        conn.Execute(delete.ToString(), model);
                    }
                   
                    return true;
                }
                catch (Exception ex)
                {
                    //error = ex.Message;
                    return false;
                }
            }
        }

        #region TEST
        private static string CREATE_TB_DEPARTMENT = "create table department(dep_id,dep_parent_id,ad_id,dep_NO,dep_name,dep_isspacial,dep_isdeleted)";
        private static string CREATE_TB_EMPLOYEE = "create table employee(emp_id,emp_NO,emp_name,dep_id,emp_cardNO,emp_tel,emp_imgurl,emp_isdeleted)";
        private static string CREATE_TB_VISITOR = "create table visitor(vt_id,vt_name,vt_sex,vt_identify_type,vt_identify_NO,vt_identify_imgurl,vt_address,vt_imgurl,tmpcard_id,tmpcard_type,vt_in_time,vt_out_time,vt_status,vt_visit_department_id,vt_visit_department,vt_visit_employee_id,vt_visit_employee,vt_visitinglist_id)";
        private static string CREATE_TB_VISITINGLIST = "create table visitinglist(vtl_id,vtl_time,vtl_tmp,vtl_tmp1)";

        public static string DB_PATH
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "manager.db"); }
        }

        private static SQLiteConnection GetSqlConnection(string sqlConnectionString)
        {
            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                return null;
            }
            SQLiteConnection conn = new SQLiteConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }

        public static void Initialize()
        {
            if (!File.Exists(DB_PATH))
            {
                SQLiteConnection.CreateFile(DB_PATH);

                using (IDbConnection conn = GetSqlConnection(_sqlConnectionStr))
                {
                    if (conn != null)
                    {
                        conn.Execute(CREATE_TB_DEPARTMENT);
                        conn.Execute(CREATE_TB_EMPLOYEE);
                        conn.Execute(CREATE_TB_VISITOR);
                        conn.Execute(CREATE_TB_VISITINGLIST);
                    }
                   
                }
            }
        }

        public static void Test()
        {
            Initialize();

            string source = string.Format("Data Source={0}", DB_PATH);
            using (IDbConnection conn = GetSqlConnection(source))
            {
                for (int i = 0; i < 10; i++)
                {
                    Department d = new Department()
                    {
                        dep_id = "0" + i,
                        dep_parent_id = "0",
                        ad_id = "0",
                        dep_NO = DateTime.Now.ToBinary().ToString(),
                        dep_name = "部门" + i,
                        dep_isdeleted = false,
                        dep_isspacial = false
                    };
                    string strsql = @"insert into department (dep_id,dep_parent_id,ad_id,dep_NO,dep_name,dep_isdeleted,dep_isspacial) values (@dep_id,@dep_parent_id,@ad_id,@dep_NO,@dep_name,@dep_isdeleted,@dep_isspacial)";
                  
                    int result = conn.Execute(strsql, d);
                }
            }

            source = string.Format("Data Source={0}", DB_PATH);
            using (IDbConnection conn = GetSqlConnection(source))
            {
                for (int i = 0; i < 10; i++)
                {
                    Employee d = new Employee()
                    {
                        emp_id = "0" + i,
                        emp_NO = "0",
                        dep_id = "0",
                        emp_cardNO = DateTime.Now.ToBinary().ToString(),
                        emp_name = "员工" + i,
                        emp_tel = "123456789",
                        emp_imgurl = "",
                        emp_isdeleted = false
                    };
                    string strsql = @"insert into employee (emp_id,emp_NO,dep_id,emp_cardNO,emp_name,emp_tel,emp_imgurl,emp_isdeleted) values (@emp_id,@emp_NO,@dep_id,@emp_cardNO,@emp_name,@emp_tel,@emp_imgurl,@emp_isdeleted)";
                    int result = conn.Execute(strsql, d);
                }
            }
        }

        #endregion
    }
}
