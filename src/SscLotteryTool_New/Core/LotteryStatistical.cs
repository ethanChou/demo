using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SscLotteryTool
{
    /// <summary>
    /// 数字统计
    /// </summary>
    public class LotteryStatistical : ViewModelBase
    {
        private const string Url = "http://www.pptv5.com/kj/fen_data.php";
        private Thread _runThread;
        private bool _isStop = false;
        /// <summary>
        /// 数据更新
        /// </summary>
        public event Action<List<LotteryNumber>> LotteryRefresh;

        private int RefrehsInterval;
        public LotteryStatistical(int interval = 60)
        {
            RefrehsInterval = 20;
        }

        public void Start()
        {
            _isStop = false;
            _runThread = new Thread(Run);
            //_runThread.SetApartmentState(ApartmentState.STA);
            _runThread.Start();
        }

        public void Stop()
        {
            _isStop = true;
            _runThread.Join();
        }
        List<LotteryNumber> _lastDatas;
        private void Run()
        {
            while (!_isStop)
            {
                try
                {
                    List<LotteryNumber> result = GetData(Url);
                    if (result.Count == 0)
                    {
                        continue;
                    }

                    if (_lastDatas == null)
                    {
                        _lastDatas = result;
                    }
                    else
                    {
                        if (_lastDatas.Count > 0 && result.Count > 0)
                        {
                            if (_lastDatas[0].ActualIndex == result[0].ActualIndex)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                        }
                    }
                    if (LotteryRefresh != null)
                    {
                        LotteryRefresh(result);
                    }

                    Console.WriteLine("{0},{1},{2}", DateTime.Now.ToString("HH:mm:ss"), result[0].onlinetime, result[0].ActualIndex);
                    _lastDatas = result;

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
            List<LotteryNumber> resultList = new List<LotteryNumber>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                Encoding encoding = Encoding.Default;
                request.Method = "get";
                request.Accept = "application/json";
                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream rs = response.GetResponseStream();
                if (rs != null)
                {
                    using (StreamReader reader = new StreamReader(rs, encoding))
                    {
                        var jsonData = reader.ReadToEnd();
                        resultList = jsonData.Deserialize<List<LotteryNumber>>();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
    }
}
