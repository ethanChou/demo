using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace VisitorManager.ViewModel
{
    /// <summary>
    /// 身份证卡
    /// </summary>
    public static class IDCardManager
    {
        public static event IDCardInfoExtractor.IDCardInfoCallBack RecevierCallback;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Thread _runThred;
        public static void Start()
        {
            IDCardInfoExtractor.Start(LocalConfig.CardPort, RecevierData);
            logger.Warn("IDCardInfoExtractor Start " + LocalConfig.CardPort);
            _runThred = new Thread(Run);
            _runThred.Start();
        }

        public static void Stop()
        {
            if (_runThred != null)

                _runThred.Abort();

            IDCardInfoExtractor.Stop(true);
        }

        private static void RecevierData(IDCardData data)
        {
            logger.Info(string.Format("{0} {1}", data.Name, data.IdCardNO));

            if (RecevierCallback != null) RecevierCallback(data);
        }

        /// <summary>
        /// 模拟数据测试接口
        /// </summary>
        static void Run()
        {
            try
            {
                const string zipToSubscribeTo = "10001";
                using (var subscriber = new SubscriberSocket())
                {
                    subscriber.Connect("tcp://127.0.0.1:5556");
                    subscriber.Subscribe(zipToSubscribeTo);

                    while (true)
                    {
                        string results = subscriber.ReceiveFrameString();
                        string[] split = results.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        try
                        {
                            IDCardData data = split[1].Deserialize<IDCardData>();
                            RecevierData(data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            logger.Error(e.ToString());
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("IDCardManager Error in " + ex.ToString());
            }
        }
    }
}
