using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.ViewModel
{
    public static class LocalConfig
    {
        static KeyValueConfig _config;
        static LocalConfig()
        {
            _config = new KeyValueConfig(AppDomain.CurrentDomain.BaseDirectory + "Config.ini");
            var acmqPort = _config["ActivemqPort", "61616"];

            var cardPort = _config["IdCardPort", "10001"];

            var thriftPort = _config["ThriftPort", "7911"];

            var thriftIp = _config["ThriftIp", "127.0.0.1"];

            var acmqIp = _config["ActivemqIp", "127.0.0.1"];

            var bimg = _config["BimgBaseUrl", "http://127.0.0.1:10551"];

            var db = _config["MysqlConn", "Database=vsm;Data Source=127.0.0.1;User Id=root;Password=root"];

            var t = _config["IsForce", "false"];
            var m = _config["IsAddLocalFile", "false"];
            var s = _config["IsCaptureAdvanced", "false"];
        }

        public static bool IsAddLocalFile
        {
            get { return bool.Parse(_config["IsAddLocalFile"]); }
        }

        public static bool IsCaptureAdvanced
        {
            get { return bool.Parse(_config["IsCaptureAdvanced"]); }
        }

        public static bool IsForce
        {
            get { return bool.Parse(_config["IsForce"]); }
        }

        public static string BimgBaseUrl
        {
            get
            {
                return _config["BimgBaseUrl"];
            }
        }

        public static string MysqlConn
        {
            get
            {
                return _config["MysqlConn"];
            }
        }

        public static int CardPort
        {

            get { return int.Parse(_config["IdCardPort"]); }
        }

        public static string ActivemqPort
        {

            get
            {
                return _config["ActivemqPort"];
            }
        }

        public static string ActivemqIp
        {

            get
            {
                return _config["ActivemqIp"];
            }
        }

        public static string ThriftPort
        {

            get
            {
                return _config["ThriftPort"];
            }
        }

        public static string ThriftIp
        {

            get
            {
                return _config["ThriftIp"];
            }
        }
    }
}
