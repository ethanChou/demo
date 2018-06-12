using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;

namespace Ftp
{
    class Program
    {
        static string remotingFolder = ConfigurationManager.AppSettings["remotingFolder"];  //远程ftp文件目录
        static string localFolder = ConfigurationManager.AppSettings["localFolder"];  //要下载到的本地目录
        static string ftpServer = ConfigurationManager.AppSettings["ftpServer"];  //ftp服务器
        static string user = ConfigurationManager.AppSettings["user"];  //用户名
        static string pwd = ConfigurationManager.AppSettings["pwd"];  //密码
        static string port = ConfigurationManager.AppSettings["port"];  //端口
        static void Main(string[] args)
        {
            FTPClient client = new FTPClient(ftpServer, "/", user, pwd, int.Parse(port));

            string str = @"E:\视频\案例测试视频_先锋巷\先锋巷.mp4";
            client.Put(str);
            client.Connect();
            GetFolder("*", remotingFolder, client, CreateFolder());
            client.DisConnect();
            ClearFolder();
            Console.WriteLine("下载完毕");
            System.Threading.Thread.Sleep(3000);
        }

        /// <summary>
        /// 在本地目录下创建一个以日期为名称的目录，我做这个ftp的主要目的是为了每天都备份
        /// </summary>
        /// <returns>创建的目录名</returns>
        private static string CreateFolder()
        {
            string folder = localFolder + "\\" + DateTime.Now.ToShortDateString();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// 在下载结束后清空程序目录的多余文件
        /// </summary>
        private static void ClearFolder()
        {
            string folder = Environment.CurrentDirectory;
            string[] dictorys = Directory.GetFiles(folder);
            foreach (string dictory in dictorys)
            {
                FileInfo info = new FileInfo(dictory);
                if (info.Length == 0)
                    File.Delete(dictory);
            }
        }

        /// <summary>
        /// 递归获取ftp文件夹的内容
        /// </summary>
        /// <param name="fileMark">文件标记</param>
        /// <param name="path">远程路径</param>
        /// <param name="client"></param>
        /// <param name="folder"></param>
        private static void GetFolder(string fileMark, string path, FTPClient client, string folder)
        {
            string[] dirs = client.Dir(path);  //获取目录下的内容
            client.ChDir(path);  //改变目录
            foreach (string dir in dirs)
            {
                string[] infos = dir.Split(' ');
                if (infos.Length <= 1)
                {
                    continue;
                }
                string info = infos[infos.Length - 1].Replace("\r", "");

                if (infos[9] == "<DIR>")  //为目录

                //if (dir.StartsWith("d") && !string.IsNullOrEmpty(dir))  //为目录
                {

                    if (!info.EndsWith(".") && !info.EndsWith(".."))  //筛选出真实的目录
                    {
                        Directory.CreateDirectory(folder + "\\" + info);
                        GetFolder(fileMark, path + "/" + info, client, folder + "\\" + info);
                        client.ChDir(path);
                    }
                }
                else //if (dir.StartsWith("-r"))  //为文件
                {
                    string file = folder + "\\" + info;
                    if (File.Exists(file))
                    {
                        long remotingSize = client.GetFileSize(info);
                        FileInfo fileInfo = new FileInfo(file);
                        long localSize = fileInfo.Length;

                        if (remotingSize != localSize)  //短点续传
                        {
                            client.GetBrokenFile(info, folder, info, localSize);
                        }
                    }
                    else
                    {
                        client.GetFile(info, folder, info);  //下载文件
                        Console.WriteLine("文件" + folder + info + "已经下载");
                    }
                }
            }

        }
    }
}