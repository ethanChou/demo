using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OASystem
{
    class Program
    {
        static int dtToday=DateTime.Now.Day;
        static bool hassendMorning = false;
        static DateTime dtMorningStart= DateTime.MinValue;
        static DateTime dtMorningEnd = DateTime.MinValue;
        static DateTime dtMorningTime = DateTime.MinValue;

        static DateTime dtAfternoonStart = DateTime.MinValue;
        static DateTime dtAfternoonEnd = DateTime.MinValue;
        static DateTime dtAfternoonTime = DateTime.MinValue;
        static bool hassendAfternoon = false;

        private static void Test()
        {
            Login();
            SaveAttendanceInfo(new DateTime(2017, 07, 27, 09, 00, 0).ToString("yyyy-MM-dd HH:mm"));
            //SaveAttendanceInfo(new DateTime(2017, 07, 21, 19, 40, 0).ToString("yyyy-MM-dd HH:mm"));

            //SaveAttendanceInfo(new DateTime(2017, 07, 24, 19, 15, 0).ToString("yyyy-MM-dd HH:mm"));

            //SaveAttendanceInfo(new DateTime(2017, 07, 25, 18, 40, 0).ToString("yyyy-MM-dd HH:mm"));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("程序启动");
            Test();
            return;
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    if (DateTime.Now.Day != dtToday)
                    {
                        hassendMorning = false;
                        hassendAfternoon = false;
                        dtToday = DateTime.Now.Day;

                        dtMorningStart = DateTime.MinValue; dtMorningEnd = DateTime.MinValue; dtMorningTime = DateTime.MinValue;
                        dtAfternoonStart = DateTime.MinValue; dtAfternoonEnd = DateTime.MinValue; dtAfternoonTime = DateTime.MinValue;
                    }
                    if (dtMorningStart == DateTime.MinValue)
                    {
                        dtMorningStart = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                          System.Configuration.ConfigurationManager.AppSettings.Get("MorningStartTime")));
                    }
                    if (dtMorningEnd == DateTime.MinValue)
                    {
                        dtMorningEnd = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                        System.Configuration.ConfigurationManager.AppSettings.Get("MorningEndTime")));
                    }
                    if (dtMorningTime == DateTime.MinValue)
                    {
                        dtMorningTime = GetDateRandom(dtMorningStart, dtMorningEnd);
                        Console.WriteLine("预计上午打卡时间为：" + dtMorningTime.ToString("yyyy-MM-dd HH:mm"));
                    }

                    if (dtAfternoonStart == DateTime.MinValue)
                    {
                        dtAfternoonStart = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                          System.Configuration.ConfigurationManager.AppSettings.Get("AfternoonStartTime")));
                    }
                    if (dtAfternoonEnd == DateTime.MinValue)
                    {
                        dtAfternoonEnd = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                            System.Configuration.ConfigurationManager.AppSettings.Get("AfternoonEndTime")));
                    }
                    if (dtAfternoonTime == DateTime.MinValue)
                    {
                        dtAfternoonTime = GetDateRandom(dtAfternoonStart, dtAfternoonEnd);
                        Console.WriteLine("预计下午打卡时间为：" + dtAfternoonTime.ToString("yyyy-MM-dd HH:mm"));
                    }

                    if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday&& hassendMorning==false&& hassendAfternoon==false)
                    {

                        if (DateTime.Now >= dtMorningStart && DateTime.Now <= dtMorningEnd && TimeEqual(DateTime.Now, dtMorningTime) && hassendMorning == false)
                        {
                            Console.WriteLine(string.Format("上午打卡，时间为：{0}", dtMorningTime.ToString("yyyy-MM-dd HH:mm")));
                            Login();
                            SaveAttendanceInfo(dtMorningTime.ToString("yyyy-MM-dd HH:mm"));
                            hassendMorning = true;
                        }
                        if (DateTime.Now >= dtAfternoonStart && DateTime.Now <= dtAfternoonEnd && TimeEqual(DateTime.Now, dtAfternoonTime) && hassendAfternoon == false)
                        {
                            Console.WriteLine(string.Format("下午打卡，时间为：{0}", dtAfternoonTime.ToString("yyyy-MM-dd HH:mm")));
                            Login();
                            SaveAttendanceInfo(dtAfternoonTime.ToString("yyyy-MM-dd HH:mm"));
                            hassendAfternoon = true;
                        }
                    }
                    Thread.Sleep(20000);
                }
            });
            thread.Start();
        }
        private static DateTime GetDateRandom(DateTime start, DateTime end)
        {
            double senconds = end.Subtract(start).TotalSeconds;
            Random randm = new Random(); int randseconds = randm.Next(1, (int)senconds);
            return start.AddSeconds(randseconds);
        }
        private static bool TimeEqual(DateTime dt1, DateTime dt2)
        {
            if (string.Equals(dt1.ToString("yyyy-MM-dd HH:mm"), dt2.ToString("yyyy-MM-dd HH:mm")))
            {
                return true;
            }
            return false;
        }
        internal class NullCls
        { }
        internal class LoginCls
        {
            public string clientVersion { get { return "6.1.0"; } }
            public string verifyCode { get { return null; } }
            public NullCls extAttrs { get { return new NullCls(); } }
            public string local { get { return "zh_CN"; } }
            public string timezone { get { return "GMT+08:00"; } }
            public int loginType { get { return 3; } }
            public string deviceCode { get { return System.Configuration.ConfigurationManager.AppSettings.Get("deviceCode"); } }
            public string password { get { return System.Configuration.ConfigurationManager.AppSettings.Get("password"); } }
            public string username { get { return System.Configuration.ConfigurationManager.AppSettings.Get("userName"); } }
            public string protocolType { get { return System.Configuration.ConfigurationManager.AppSettings.Get("protocolType"); } }
            public string token { get { return System.Configuration.ConfigurationManager.AppSettings.Get("token"); } }
            //    "deviceCode": "[iPhone]:ljyiPhone|5FFF9086-8C41-43E9-BF21-F84E28639F58",
            //"password": "Ono40dqc3nfyduEVEdDQNA==",
            //"clientVersion": "6.1.0",
            //"verifyCode": null,
            //"extAttrs": {},
            //"local": "zh_CN",
            //"timezone": "GMT+08:00",
            //"username": "Ono40dqc3nc8WSDHQbNEk2+tPeYEGggn",
            //"protocolType": "iPhone",
            //"token": "1857015855829ead6de1b3161ea84cb4e21291c42e5afe7a5cfdb7e786a504ee",
            //"loginType": 3
        }
        internal class SaveAddr
        {
            public string lbsAddr { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsAddress"); } }
            public string category { get { return "36"; } }
            public string lbsType { get { return "0"; } }
            public string lbsProvince { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsProvince"); } }
            public string lbsTown { get { return null; } }
            public string createDate { get; set; }
            public string lbsComment { get { return ""; } }
            public string lbsStreet { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsStreet"); } }
            public string lbsDimensionality { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsDimensionality"); } }
            public string lbsCountry { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsCountry"); } }
            public string lbsLongitude { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsLongitude"); } }
            public string lbsCity { get { return System.Configuration.ConfigurationManager.AppSettings.Get("lbsCity"); } }
            public string lbsContinent { get { return null; } }
            public List<int> listAttachment { get { return new List<int>(); } }
            //            [
            //    {
            //        "lbsAddr": "江苏省南京市雨花台区小行路158号(南京市公安局地铁分局后面)理想家青年公寓",
            //        "category": "36",
            //        "lbsType": "0",
            //        "lbsProvince": "江苏省",
            //        "lbsTown": null,
            //        "createDate": "2017-06-30  14:36",
            //        "lbsComment": "",
            //        "lbsStreet": "小行路158号(南京市公安局地铁分局后面)",
            //        "lbsDimensionality": "31.977808",
            //        "lbsCountry": "中国",
            //        "lbsContinent": null,
            //        "lbsLongitude": "118.739888",
            //        "lbsCity": "南京市",
            //        "listAttachment": []
            //        }
            //]
        }
        private static string _baseUrl = "http://moa.netposa.com:9999";
        private static string _address = "/seeyon/servlet/SeeyonMobileBrokerServlet?serviceProcess=A6A8_Common";
        private static string _cookie;
        private static HttpWebRequest Request(string address, string cookie)
        {
            string requestAddress = string.IsNullOrEmpty(address) ? string.Format("{0}/{1}", _baseUrl, _address) : string.Format("{0}/{1}/{2}", _baseUrl, _address, address);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestAddress);
            request.UserAgent = Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings.Get("phoneVersion")));
            request.Method = "POST";
            request.KeepAlive = false;
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }
            //request.Headers.Add("Cookies")
            request.Headers.Add("Accept-Encoding", "gzip");
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            return request;
        }
        private static void Login()
        {
            HttpWebRequest RequestGetOAProfile = Request("", "");
            List<LoginCls> lst = new List<LoginCls>();
            lst.Add(new LoginCls());
            string Json = HttpUtility.UrlEncode(JsonConvert.SerializeObject(lst, Formatting.None));
            byte[] postdata = System.Text.Encoding.UTF8.GetBytes(string.Format("managerMethod=transLogin&managerName=mLoginManager&arguments={0}", Json));
            RequestGetOAProfile.ContentLength = postdata.Length;

            RequestGetOAProfile.GetRequestStream().Write(postdata, 0, postdata.Length);

            HttpWebResponse res = (HttpWebResponse)RequestGetOAProfile.GetResponse();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                _cookie = res.Headers.Get("Set-Cookie").Split(';')[0];
                Stream myResponseStream = res.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();
            }
            RequestGetOAProfile.Abort();
        }
        private static void SaveAttendanceInfo(string datetime)
        {
            if (string.IsNullOrEmpty(_cookie))
            {
                return;
            }
            HttpWebRequest RequestSaveAddress = Request("", _cookie);
            SaveAddr addr = new SaveAddr();
            addr.createDate = datetime;//DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            List<SaveAddr> lst = new List<SaveAddr>();
            lst.Add(addr);
            string Json = HttpUtility.UrlEncode(JsonConvert.SerializeObject(lst, Formatting.None));
            byte[] postdata = System.Text.Encoding.UTF8.GetBytes(string.Format("managerMethod=transSaveAttendanceInfo&managerName=mLbsManager&arguments={0}", Json));
            RequestSaveAddress.ContentLength = postdata.Length;

            RequestSaveAddress.GetRequestStream().Write(postdata, 0, postdata.Length);

            HttpWebResponse res = (HttpWebResponse)RequestSaveAddress.GetResponse();
            if (res.StatusCode == HttpStatusCode.OK)
            {

                Stream myResponseStream = res.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();
            }
            RequestSaveAddress.Abort();
        }
    }
}
