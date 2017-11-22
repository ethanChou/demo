using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;

namespace VisitorManager.ViewModel
{
    /// <summary>
    /// 写LOG的具体实现位置,代码实现和老的LogPrinter是一样的
    /// </summary>
    public class LogPrinter
    {
        private static string FilePath = "Log";

        private static FileStream _fileStream;
        private static StreamWriter _streamWriter;
        private static object _lock = new object();

        /// <summary>
        /// 打日志程序DLL（soft.core）所在的文件路径
        /// </summary>
        private static string _executingAssemblyDir =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        private static List<string> _logSysHeader = new List<string>();
        private static void InitSysHead()
        {
            //如果不清空，连续跑2天以上会重复打印日志头
            _logSysHeader = new List<string>();

            string line = "Operating System Version: " + Environment.OSVersion;
            _logSysHeader.Add(line);
            //             line = "User Name: " + SystemInformation.UserName;
            //             _logSysHeader.Add(line);
            line = GetCPUInfo(0);
            _logSysHeader.Add(line);
            line = GetLogicDisk(0);
            _logSysHeader.Add(line);
        }

        private static string GetCPUInfo(int type)
        {
            string result = "";

            RegistryKey rk = Registry.LocalMachine;
            rk = rk.OpenSubKey("HARDWARE", true);
            rk = rk.OpenSubKey("DESCRIPTION", true);
            rk = rk.OpenSubKey("System", true);
            rk = rk.OpenSubKey("CentralProcessor", true);
            rk = rk.OpenSubKey("0", true);
            result += "Processor Name: " + rk.GetValue("ProcessorNameString").ToString();

            return result;
        }

        private static string GetLogicDisk(int type)
        {
            string result = "";
            result += "Drivers: ";

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType != DriveType.Fixed)
                {
                    continue;
                }

                result += d.Name + " ";
                if (d.IsReady == true)
                {
                    result += d.TotalSize / (1024 * 1024 * 1024) + "GB;  ";
                }
            }

            return result;
        }

        /// <summary>
        /// 打印头信息
        /// </summary>
        private static void PrintHeader()
        {
            lock (_lock)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Write("\r\n");
                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                        "/*******************************************************************************\r\n");

                    for (int i = 0; i < _logSysHeader.Count; i++)
                    {
                        _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " + _logSysHeader[i] + "\r\n");
                    }

                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                        "********************************************************************************/\r\n");
                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " + "\r\n");
                }
            }
        }

        public static bool OpenLog()
        {
            return OpenLog(FilePath);
        }

        private static bool ExistPath(string path)
        {
            return File.Exists(path);
        }

        private static bool _printHeader = false;
        /// <summary>
        /// 日志文件大小
        /// </summary>
        private static int MaxLen = 524288;

        private static DateTime _logTime = DateTime.MinValue;

        public static bool CloseLog()
        {
            bool flag = false;
            lock (_lock)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Close();
                    _streamWriter = null;
                }

                if (_fileStream != null)
                {
                    _fileStream.Close();
                    _fileStream = null;
                    flag = true;
                }
            }


            return flag;
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="functionName">方法名</param>
        /// <param name="logLine">要打印的字符串</param>
        /// <param name="args">参数</param>
        public static void Write(string logLine, params object[] args)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);//[0]为本身的方法 [1]为调用方法

            string functionName = sf.GetMethod().Name;//方法名
            string className = sf.GetMethod().DeclaringType.Name;//类名

            OpenLog();
            lock (_lock)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                        "[" + className + "] " + "[" + functionName + "] " +
                        string.Format(logLine, args) + "\r\n");
                }
            }
            CloseLog();
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="logLine">要打印的字符串</param>
        public static void Write(string logLine)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);//[0]为本身的方法 [1]为调用方法

            string functionName = sf.GetMethod().Name;//方法名
            string className = sf.GetMethod().DeclaringType.Name;//类名

            OpenLog();
            lock (_lock)
            {
                try
                {
                    if (_streamWriter != null)
                    {
                        _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                            "[" + className + "] " + "[" + functionName + "] " +
                            logLine + "\r\n");
                    }
                }
                catch
                { }
            }
            CloseLog();
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="className">上层反射得到的类名</param>
        /// <param name="functionName">上层反射得到的方法名</param>        
        /// <param name="logLine"></param>
        /// <param name="args"></param>
        public static void Write(string className,
            string functionName, string logLine, params object[] args)
        {
            OpenLog();
            lock (_lock)
            {
                try
                {
                    if (_streamWriter != null)
                    {
                        _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                            "[" + className + "] " + "[" + functionName + "] " +
                            string.Format(logLine, args) + "\r\n");
                    }
                }
                catch
                { }
            }
            CloseLog();
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="className">上层反射得到的类名</param>
        /// <param name="functionName">上层反射得到的方法名</param> 
        /// <param name="logLine">要打印的字符串</param>
        public static void Write(string className, string functionName, string logLine)
        {
            OpenLog();
            lock (_lock)
            {
                try
                {
                    if (_streamWriter != null)
                    {
                        _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " +
                            "[" + className + "] " + "[" + functionName + "] " +
                            logLine + "\r\n");
                    }
                }
                catch
                { }
            }
            CloseLog();
        }

        public static void WriteWithDate(string logLine)
        {
            //             if (_logDate.Date != DateTime.Now.Date)
            //             {
            //                 _logDate = DateTime.Now;
            //                 
            //                 
            //             }
            OpenLog();
            lock (_lock)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " + logLine);
                }
            }
            CloseLog();
        }

        /// <summary>
        /// 当不能通过Application.StartupPath来获取运行目录时使用此方法指定目录
        /// </summary>
        /// <param name="logLine">LOG语句</param>
        /// <param name="runPath">指定的运行目录</param>
        public static void WriteWithDate(string logLine, string runPath)
        {
            OpenLog(FilePath, runPath);
            lock (_lock)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Write("[" + DateTime.Now.ToString("T") + "] " + logLine);
                }
            }
            CloseLog();
        }

        public static bool OpenLog(string logDir)
        {
            //如果是CS程序，目录相同，打印在安装目录的log下面
            //如果是bs程序，则目录不同，打印在临时目录下
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            if (Path.GetDirectoryName(dirPath) != _executingAssemblyDir)
            {
                dirPath = Path.GetTempPath();
            }
            return OpenLog(logDir, dirPath);
        }

        public static bool MinFileModel = true;
        /// <summary>
        /// 在文件运行根目录下生成一个log文件夹，日志写在此文件夹中
        /// </summary>
        /// <param name="logDir">文件夹名称</param>
        /// <param name="runPath">根目录</param>
        /// <returns></returns>
        public static bool OpenLog(string logDir, string runPath)
        {
            FilePath = logDir;
            bool flag = false;
            lock (_lock)
            {
                runPath = runPath.TrimEnd('\\');
                string dir = string.Format("{0}\\{1}", runPath, logDir);

                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (_fileStream == null)
                    {
                        if (_logTime == DateTime.MinValue)
                        {

                            DirectoryInfo dirInfo = new DirectoryInfo(dir);
                            var dirArray = dirInfo.GetFiles();
                            if (dirArray.Length > 0)
                            {
                                var logList = dirArray.ToList();

                                var qry = from x in logList
                                          where x.Name.Length == 18
                                          orderby x.CreationTime
                                          select x;
                                var fileInfo = qry.LastOrDefault();

                                if (fileInfo != null && fileInfo.Name.Length == 18)
                                {
                                    try
                                    {
                                        _logTime =
                                            DateTime.ParseExact(Path.GetFileNameWithoutExtension(fileInfo.FullName),
                                                "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                                        if (_logTime.Day != DateTime.Now.Day && _logTime.Month != DateTime.Now.Month)
                                        {
                                            _logTime = DateTime.Now;
                                        }
                                    }
                                    catch
                                    {
                                        _logTime = DateTime.Now;
                                    }

                                }
                                else
                                {
                                    _logTime = DateTime.Now;
                                }
                            }
                            else
                            {
                                _logTime = DateTime.Now;
                            }


                        }

                        string timeStr = MinFileModel ? 
                            string.Format("{0:d04}{1:d02}{2:d02}{3:d02}{4:d02}{5:d02}",
                            _logTime.Year, _logTime.Month, _logTime.Day, _logTime.Hour, _logTime.Minute, _logTime.Second) :
                            string.Format("{0:d04}{1:d02}{2:d02}",
                           _logTime.Year, _logTime.Month, _logTime.Day);
                        string path =
                            string.Format("{0}\\{1}.txt", dir, timeStr);

                        if (ExistPath(path))
                        {
                            FileInfo fileInfo = new FileInfo(path);
                            if (fileInfo.Length >= MaxLen)
                            {
                                _logTime = DateTime.Now;
                                timeStr = string.Format(
                                    "{0:d04}{1:d02}{2:d02}{3:d02}{4:d02}{5:d02}",
                                    _logTime.Year, _logTime.Month, _logTime.Day, _logTime.Hour, _logTime.Minute, _logTime.Second);
                                path = string.Format("{0}\\{1}.txt", dir, timeStr);
                            }
                        }

                        //string timeStr = string.Format("{0:d04}{1:d02}{2:d02}",
                        //    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //string path =
                        //    string.Format("{0}\\{1}.txt", dir, timeStr);

                        _printHeader = false;
                        if (!ExistPath(path))
                        {
                            _printHeader = true;
                        }
                        _fileStream = new FileStream(path, FileMode.OpenOrCreate);

                        _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8);
                        _streamWriter.AutoFlush = true;
                        _streamWriter.BaseStream.Seek(0, SeekOrigin.End);

                        if (_printHeader)
                        {
                            InitSysHead();
                            // 打印头信息
                            PrintHeader();
                        }
                        flag = true;
                    }
                }
                catch
                {

                }

            }

            return flag;
        }
    }
}
