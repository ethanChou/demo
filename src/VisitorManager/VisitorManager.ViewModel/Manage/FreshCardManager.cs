using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using NLog;

namespace VisitorManager.ViewModel
{
    /// <summary>
    /// 内部员工卡
    /// </summary>
    public class FreshCardManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static IConnectionFactory factory;
        public static string ConnectStr
        {
            get
            {
                return string.Format("tcp://{0}:{1}", LocalConfig.ActivemqIp, LocalConfig.ActivemqPort);
            }
        }

        public delegate void DelegateRevMessage(string message);

        public static event DelegateRevMessage MessageReceived;

        public static string VisitorFreshCardTopic { get { return "register"; } }
        public static string AlarmTopic { get { return "alarm"; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectActiveMqStr">tcp://localhost:61616,网页查看地址为http://localhost:8161/ 用户密码admin</param>
        /// <param name="toppicName"></param>
        /// <returns></returns>
        public static bool Init(string connectActiveMqStr)
        {
            try
            {
                factory = new ConnectionFactory(connectActiveMqStr);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Init " + connectActiveMqStr);
                return false;
            }
            return true;
        }

        public static bool SendData(string jsonData, string toppicName)
        {

            if (factory == null)
            {
                Init(ConnectStr);
            }

            try
            {
                using (IConnection connection = factory.CreateConnection())
                {
                    //通过连接创建Session会话
                    using (ISession session = connection.CreateSession())
                    {
                        //通过会话创建生产者，方法里面new出来的是MQ中的toppic
                        IMessageProducer prod = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(toppicName));
                        //创建一个送的消息对象
                        ITextMessage message = prod.CreateTextMessage();
                        //给这个对象赋实际的消息
                        message.Text = jsonData;
                        ////设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                        //message.Properties.SetString("filter", "demo");
                        //生产者把消息发送出去，几个枚举参数MsgDeliveryMode是否长链，MsgPriority消息优先级别，发送最小单位，当然还有其他重载
                        prod.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString(), "SendData");
                return false;
            }
            return true;
        }
        static IConnection _connection;
        static Apache.NMS.ISession _session;
        private static bool isStart = false;
        public static bool Start(string toppicName = "")
        {
            if (isStart) return true;

            if (string.IsNullOrEmpty(toppicName))
            {
                toppicName = VisitorFreshCardTopic;
            }
            if (factory == null)
            {
                Init(ConnectStr);
            }
            try
            {
                //创建连接工厂
                if ((_connection == null && _session == null) || !_connection.IsStarted)
                {
                    //通过工厂构建连接
                    _connection = factory.CreateConnection();
                    //这个是连接的客户端名称标识
                    _connection.ClientId = Guid.NewGuid().ToString();
                    //启动连接，监听的话要主动启动连接
                    _connection.Start();
                    //通过连接创建一个会话
                    _session = _connection.CreateSession();
                }
                IMessageConsumer _consumer = _session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(toppicName), null, null, true);
                _consumer.Listener += new MessageListener((x) =>
                {
                    try
                    {
                        ITextMessage msg = (ITextMessage)x;
                        if (MessageReceived != null)
                        {
                            logger.Info(msg.Text, "MessageReceived");
                            MessageReceived(msg.Text);
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                  
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString(), "Start");
                return false;
            }
            isStart = true;
            return true;
        }
    }
}
