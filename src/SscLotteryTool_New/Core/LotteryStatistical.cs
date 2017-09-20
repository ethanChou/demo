using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SscLotteryTool
{
    /// <summary>
    /// 数字统计
    /// </summary>
    public class LotteryStatistical : ViewModelBase
    {
        /// <summary>
        /// http://pptv5.com/kj/fen_data.php
        /// http://www.pptv5.com/kj/fen_data.php
        /// </summary>
        private const string Url = "https://mma.qq.com/cgi-bin/im/online&callback=test";
        private Thread _runThread;
        private Thread _getDataThread;
        private bool _isStop = false;
        /// <summary>
        /// 数据更新
        /// </summary>
        public event Action<List<LotteryNumber>> LotteryRefresh;

        private Queue<LotteryNumber> _buffer = new Queue<LotteryNumber>();
        private Stopwatch _watch = new Stopwatch();
        private LotteryNumber _lastData;
        private static object _lock = new object();
        private List<LotteryNumber> _lastDatas;
        private string baseRoot, dataDir;
        //private int RefrehsInterval;
        public LotteryStatistical(int interval = 60)
        {
            //RefrehsInterval = 20;
        }

        public void Start()
        {

            baseRoot = AppDomain.CurrentDomain.BaseDirectory;
            dataDir = Path.Combine(baseRoot, "Data");

            InitDir(dataDir);

            _isStop = false;
            _getDataThread = new Thread(RunData);
            _runThread = new Thread(Run);
            _runThread.Start();
            _getDataThread.Start();


        }

        private void InitDir(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception)
            {

            }
        }

        public void Stop()
        {
            _isStop = true;
            _runThread.Join();
            _getDataThread.Join();
        }

        private void RunData()
        {
            while (!_isStop)
            {
                try
                {
                    LotteryNumber result = GetDataEx(Url);

                    if (result == null) continue;

                    if (_lastData == null || !_lastData.ActualIndex.Equals(result.ActualIndex))
                    {
                        lock (_lock)
                        {
                            _buffer.Enqueue(result);
                            _lastData = result;
                        }
                    }

                    string tempDir = Path.Combine(dataDir, String.Format("{0}{1}{2}", result.Time.Year, result.Time.Month, result.Time.Day), result.Time.Hour.ToString());

                    InitDir(tempDir);

                    using (FileStream fileStream = new FileStream(Path.Combine(tempDir, "ssc.txt"), FileMode.OpenOrCreate))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            streamWriter.AutoFlush = true;
                            streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                            streamWriter.WriteLine("[{0}] 在线人数: {1},期号：{2},开奖号码：{3}", result.onlinetime, result.onlinenumber, result.ActualIndex, result.ActualNum);
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception)
                {

                }
            }
        }

        private void Run()
        {
            while (!_isStop)
            {
                try
                {
                    LotteryNumber number;
                    lock (_lock)
                    {
                        if (_buffer.Count == 0)
                        {
                            //Console.WriteLine("队列为空");
                            continue;
                        }
                        number = _buffer.Dequeue();
                    }

                    List<LotteryNumber> result = new List<LotteryNumber>() { number };
                    _watch.Restart();

                    if (LotteryRefresh != null)
                    {
                        LotteryRefresh(result);
                    }

                    //List<LotteryNumber> result = GetData(Url);
                    //if (result.Count == 0)
                    //{
                    //    Thread.Sleep(10);
                    //    continue;
                    //}

                    //if (_lastDatas == null)
                    //{
                    //    _lastDatas = result;
                    //}
                    //else
                    //{
                    //    if (_lastDatas.Count > 0 && result.Count > 0)
                    //    {
                    //        if (_lastDatas[0].ActualIndex == result[0].ActualIndex)
                    //        {
                    //            Thread.Sleep(2000);
                    //            continue;
                    //        }
                    //    }
                    //}
                    //if (LotteryRefresh != null)
                    //{
                    //    LotteryRefresh(result);
                    //}
                    //_lastDatas = result;

                    _watch.Stop();

                    Console.WriteLine("----------------------- UI Time-------------{0}", _watch.ElapsedMilliseconds);

                    #region refer js code

                    //                  $(document).ready(function() {
                    //  ajaxdata();

                    //  function ajaxdata() {
                    //        $.ajax({
                    //            url: '/kj/fen_data.php',
                    //            type: 'GET',
                    //            dataType: 'json',
                    //            success: function(result) {

                    //                //console.log(result);
                    //                var html='';
                    //                for(var i=0;i<result.length;i++) {
                    //                  var item = result[i];
                    //                 // console.log(item);
                    //                  var linenum = item.onlinenumber;//当前在线人数；
                    //                  var linetime = item.onlinetime;//时间
                    //                  var linechange = item.onlinechange;//变化值
                    //                  var adddata = String(linenum).split('');
                    //                  var total = 0;
                    //                  for (var m = 0; m < adddata.length; m++) {
                    //                    total += parseInt(adddata[m]);
                    //                  }
                    //                 // console.log(total);
                    //                  var fornum = String(total).slice(-1);
                    //                  var lastnum = String(linenum).slice(-4);
                    //                  var nums = fornum + lastnum;
                    //                  var num = nums.split('');

                    //                  var htmlStr = '<li>'+num[0]+','+num[1]+','+num[2]+','+num[3]+','+num[4]+'</li>';
                    //                  /*$.each(num, function (key, value) {
                    //                    htmlStr += value+",";
                    //                  });*/
                    //                  html += htmlStr;
                    //                }
                    //                $('#num_list').html(html);
                    //            }
                    //        });

                    //    }
                    //});

                    #endregion
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(1000);
            }
        }


        private List<LotteryNumber> GetData(string url)
        {
            _watch.Restart();
            List<LotteryNumber> resultList = new List<LotteryNumber>();

            //string url = "https://mma.qq.com/cgi-bin/im/online&callback=test";
            //https://mma.qq.com/mqqactivity/cgi/starmap/get_online?callback=test
            //https://mma.qq.com/cgi-bin/im/online&callback=test
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.5");

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream rs = response.GetResponseStream();
                if (rs != null)
                {
                    using (StreamReader reader = new StreamReader(rs))
                    {
                        var jsonData = reader.ReadToEnd();
                        // online_resp({"c":242949766,"ec":0,"h":272203829})
                        DateTime ct = DateTime.Now;
                        Debug.WriteLine("{0}:{1}", ct.ToString("HH:mm:ss"), jsonData);
                        if (jsonData.StartsWith("online_resp"))
                        {
                            int s = jsonData.IndexOf(':') + 1;
                            int e = jsonData.IndexOf(',');
                            //Console.WriteLine(jsonData.Substring(s, e - s));
                            int num = int.Parse(jsonData.Substring(s, e - s));
                            LotteryNumber lnub = new LotteryNumber();
                            lnub.onlinetime = ct.ToString();
                            lnub.onlinenumber = num;
                            resultList.Add(lnub);
                        }
                        else
                        {

                        }
                    }
                }
                response.Close();
            }
            catch (Exception)
            {

                //throw;
            }

            //try
            //{
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //    Encoding encoding = Encoding.Default;
            //    request.Method = "get";
            //    request.Accept = "application/json";
            //    request.ContentType = "application/json";

            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    Stream rs = response.GetResponseStream();
            //    if (rs != null)
            //    {
            //        using (StreamReader reader = new StreamReader(rs, encoding))
            //        {
            //            var jsonData = reader.ReadToEnd();
            //            resultList = jsonData.Deserialize<List<LotteryNumber>>();
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            _watch.Stop();
            Console.WriteLine("Take Time {0}", _watch.ElapsedMilliseconds);
            return resultList;
        }

        private LotteryNumber GetDataEx(string url)
        {
            //_watch.Restart();
            LotteryNumber result = new LotteryNumber();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.5");

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream rs = response.GetResponseStream();
                if (rs != null)
                {
                    using (StreamReader reader = new StreamReader(rs))
                    {
                        var jsonData = reader.ReadToEnd();
                        // online_resp({"c":242949766,"ec":0,"h":272203829})
                        DateTime ct = DateTime.Now;
                        Debug.WriteLine("====={0}:{1}", ct.ToString("HH:mm:ss"), jsonData);
                        if (jsonData.StartsWith("online_resp"))
                        {
                            int s = jsonData.IndexOf(':') + 1;
                            int e = jsonData.IndexOf(',');
                            //Console.WriteLine(jsonData.Substring(s, e - s));
                            int num = int.Parse(jsonData.Substring(s, e - s));
                            LotteryNumber lnub = new LotteryNumber();
                            lnub.onlinetime = ct.ToString();
                            lnub.onlinenumber = num;
                            result = lnub;
                        }
                        else
                        {

                        }
                    }
                }
                response.Close();
            }
            catch (Exception)
            {
                //throw;
            }

            //_watch.Stop();
            //Console.WriteLine("Take Time {0}", _watch.ElapsedMilliseconds);
            return result;
        }


        //private List<LotteryNumber> GetDataEx(string url, string cookie = "__cfduid=d661681e05fabf41ec51f59a7f73010711505575723")
        //{
        //    List<LotteryNumber> resultList = new List<LotteryNumber>();
        //    try
        //    {
        //        string urlex = "http://www.77tj.org/api/tencent/online";
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlex);

        //        request.Method = "GET";

        //        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
        //        request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.5");
        //        request.Referer = "http://www.77tj.org/api/tencent/online";
        //        request.Host = "www.77tj.org";
        //        if (!string.IsNullOrEmpty(cookie)) request.Headers.Add("Cookie", cookie);

        //        if (!string.IsNullOrEmpty(_cookie)) request.Headers.Add("Cookie", _cookie);

        //        try
        //        {
        //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //            Stream rs = response.GetResponseStream();
        //            if (rs != null)
        //            {
        //                using (StreamReader reader = new StreamReader(rs))
        //                {
        //                    var jsonData = reader.ReadToEnd();
        //                    //Console.WriteLine(jsonData);
        //                    resultList = jsonData.Deserialize<List<LotteryNumber>>();
        //                }
        //            }
        //        }
        //        catch (WebException e)
        //        {
        //            HttpWebResponse resp = (HttpWebResponse)e.Response;
        //            string ckie = "";
        //            if (string.IsNullOrEmpty(cookie))
        //            {
        //                ckie = resp.Headers.Get("Set-Cookie").Split(';')[0];
        //            }
        //            else
        //            {
        //                ckie = cookie;
        //            }
        //            Stream rs = resp.GetResponseStream();
        //            if (rs != null)
        //            {
        //                using (StreamReader reader = new StreamReader(rs))
        //                {
        //                    var jsonData = reader.ReadToEnd();
        //                    Thread.Sleep(4000);
        //                    return Check(ckie, jsonData);
        //                    //resultList = jsonData.Deserialize<List<LotteryNumber>>();
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return resultList;
        //}
        //private string _cookie;
        //private List<LotteryNumber> Check(string cookie, string jsonData)
        //{
        //    Console.WriteLine(jsonData);
        //    Regex regHtml = new Regex("name=\"jschl_vc\" value=\".*\"");
        //    Match m = regHtml.Match(jsonData);
        //    string userid = "", pass = "", uid = "";
        //    if (m.Success)
        //    {
        //        userid = m.Value;
        //        int si = userid.IndexOf("value=\"") + 7;
        //        userid = userid.Substring(si, userid.Length - si - 1);
        //    }
        //    Regex regHtm2 = new Regex("name=\"pass\" value=\".*\"");
        //    m = regHtm2.Match(jsonData);
        //    if (m.Success)
        //    {
        //        pass = m.Value;
        //        int si = pass.IndexOf("value=\"") + 7;
        //        pass = pass.Substring(si, pass.Length - si - 1);
        //    }
        //    //jschl-answer
        //    Regex regex = new Regex("var s,t,o,p,b,r,e,a,k,i,n,g,f, .*");
        //    m = regex.Match(jsonData);
        //    if (m.Success)
        //    {
        //        string res = m.Value;
        //        int index = res.IndexOf("=");
        //        var beginTag = res.Substring(31, index - 31);
        //        var first = res.Substring(31, res.Length - 31);
        //        Regex reg1 = new Regex(beginTag + ".*\\;");
        //        MatchCollection m1 = reg1.Matches(jsonData);
        //        if (m1.Count > 0)
        //        {
        //            string start = "var a={};var t=\"www.77tj.org\",";
        //            for (int i = 0; i < m1.Count; i++)
        //            {
        //                start += m1[i].Value;
        //            }
        //            start = start.Substring(0, start.Length - 3);
        //            uid = EvalJScript(start).ToString();
        //        }
        //    }

        //    string urlFormat = "http://www.77tj.org/cdn-cgi/l/chk_jschl?jschl_vc={0}&pass={1}&jschl_answer={2}";
        //    string destUrl = string.Format(urlFormat, userid, pass, uid);
        //    // http://www.77tj.org/cdn-cgi/l/chk_jschl?jschl_vc=33c2614ca9e8909669ebb82651cb319f&pass=1505636219.125-4Qonl5nHmA&jschl_answer=-61

        //    try
        //    {
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destUrl);

        //        request.Method = "GET";

        //        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //        request.UserAgent = "Mozilla/5.0  (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
        //        request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
        //        request.Headers.Add("Accept-Encoding" ,"gzip, deflate");
        //        request.Headers.Add("Upgrade-Insecure-Request", "1");
        //        request.KeepAlive = true;
        //        request.AllowAutoRedirect = false;
        //        request.Referer = "http://www.77tj.org/api/tencent/online";
        //        if (!string.IsNullOrEmpty(cookie))
        //        {
        //            request.Headers.Add("Cookie", cookie);
        //        }
        //        request.Host = "www.77tj.org";
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        if (response.Headers.Get("Set-Cookie") != null)
        //        {
        //            string cookie2 = response.Headers.Get("Set-Cookie").Split(';')[0];
        //            string newCookie = cookie + ";" + cookie2;
        //            _cookie = newCookie;
        //            MainWindow.config["cookie"] = _cookie;

        //            return GetDataEx("");
        //        }
        //        return null;
        //    }
        //    catch (WebException e)
        //    {
        //        HttpWebResponse resp = (HttpWebResponse)e.Response;
        //        if (resp.Headers.Get("Set-Cookie") != null)
        //        {
        //            string cookie2 = resp.Headers.Get("Set-Cookie").Split(';')[0];
        //            string newCookie = cookie + ";" + cookie2;
        //            return GetDataEx("", newCookie);
        //        }
        //        else
        //        {

        //            return GetDataEx("", cookie);
        //        }
        //        return null;




        //    }
        //}

        //static VsaEngine Engine = VsaEngine.CreateEngine();
        //static object EvalJScript(string JScript)
        //{
        //    object Result = null;
        //    try
        //    {
        //        Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //    return Result;

        //}
    }
}
