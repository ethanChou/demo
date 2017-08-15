using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SscLotteryTool
{
    /// <summary>
    /// 配置信息类，负责读取配置文件，读写配置信息，和更新配置文件
    /// </summary>
    public class KeyValueConfig
    {
        #region Member

        private readonly Dictionary<string, string> cfg = new Dictionary<string, string>();
        public string Filename;

        #endregion

        #region Constructor

        public KeyValueConfig(string filename)
        {
            Filename = filename;
            LoadConfig();
        }

        ~KeyValueConfig()
        {
            SaveConfig();
        }

        #endregion

        #region this[]

        /// <summary>
        /// 查询和设置配置信息
        /// </summary>
        /// <param name="name">配置项名字</param>
        /// <returns>查找结果</returns>
        public string this[string name]
        {
            get
            {
                string ret;
                if (cfg.TryGetValue(name, out ret))
                    return ret;
                else
                {
                    return "";
                }
            }
            set
            {
                AddValue(name, value);
                SaveConfig();
            }
        }

        /// <summary>
        /// 查询配置信息，如果查不到的话，可以提供一个默认返回值
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="def">默认值</param>
        /// <returns>查找结果</returns>
        public string this[string name, string def]
        {
            get
            {
                string ret;
                if (cfg.TryGetValue(name, out ret))
                    return ret;
                else
                {
                    AddValue(name, def);
                    SaveConfig();
                    return def;
                }
            }
            set
            {
                AddValue(name, value);
                SaveConfig();
            }
        }

        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="key"></param>
        public void DeleteKey(string key)
        {
            if (IsExit(key))
                cfg.Remove(key);
            SaveConfig();
        }

        /// <summary>
        /// 是否存在该配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsExit(string key)
        {
            if (cfg.ContainsKey(key))
                return true;
            return false;
        }

        #endregion

        #region Method

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private void LoadConfig()
        {
            if (!File.Exists(Filename))
            {
                if (!Directory.Exists(Path.GetDirectoryName(Filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(Filename));

                FileStream stream = File.Create(Filename);
                stream.Close();
                return;
            }
            FileStream filestream = File.OpenRead(Filename);
            LoadConfigStream(filestream);
            filestream.Close();
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        private void SaveConfig()
        {
            FileStream filestream = File.OpenWrite(Filename);
            SaveConfigStream(filestream);
            filestream.Close();
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        private void LoadConfigStream(Stream stream)
        {
            StreamReader read = null;
            try
            {
                cfg.Clear();
                read = new StreamReader(stream, Encoding.Unicode);
                string line = read.ReadLine();

                int remark = 0;
                while (line != null)
                {
                    string name;
                    // 如果是注释的话，加一个特殊标记，以便保存时能正确保存原来的注释
                    if (Regex.Match(line, "^//").Success)
                    {
                        name = string.Format("//remark{0}", remark++);
                        AddValue(name, line);
                    }
                    else if (line.Length == 0)
                    {
                        name = string.Format("//line{0}", remark++);
                        AddValue(name, "");
                    }
                    else
                    {
                        MatchCollection pair = Regex.Matches(line, "[^=]+");
                        if (pair.Count >= 2)
                        {
                            if (pair.Count == 2)
                                AddValue(pair[0].Value.TrimEnd(), pair[1].Value.TrimStart());
                            else
                            {
                                string value = line.Substring(line.IndexOf('=') + 1).Trim();
                                AddValue(pair[0].Value.TrimEnd(), value);
                            }
                        }
                    }
                    line = read.ReadLine();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (read != null)
                {
                    read.Close();
                }
            }
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        private void SaveConfigStream(Stream stream)
        {
            try
            {
                var write = new StreamWriter(stream, Encoding.Unicode);
                foreach (var item in cfg)
                {
                    if (item.Key.IndexOf("//") == 0)
                        write.WriteLine(item.Value);
                    else if (item.Key.IndexOf("//line") == 0)
                        write.WriteLine("");
                    else
                        write.WriteLine("{0} = {1}", item.Key, item.Value);
                }
                write.Flush();
                write.Close();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        private void AddValue(string name, string value)
        {
            string old;
            if (cfg.TryGetValue(name, out old))
                cfg[name] = value;
            else
                cfg.Add(name, value);
        }

        #endregion
    }
}
