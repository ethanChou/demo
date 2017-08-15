using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SscLotteryTool
{
    [Serializable]
    public class LotteryNumberCollection
    {
        /// <summary>
        /// Deserialize the XML file and load to data of the dataType
        /// </summary>
        public static object Deserialize(string file, Type dataType)
        {
            object data = null;
            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(dataType);

                string full = file;
                TextReader tr = new StreamReader(full);

                data = serializer.Deserialize(tr);

                tr.Close();

            }
            catch
            {
                // throw (e);
            }

            return data;
        }

        /// <summary>
        /// Serialize data and save in the xml file
        /// </summary>
        public static void Serialize(string file, object data)
        {
            try
            {
                Type dataType = data.GetType();
                XmlSerializer serializer =
                    new XmlSerializer(dataType);
                TextWriter tw =
                    new StreamWriter(file, false, Encoding.Unicode);
                serializer.Serialize(tw, data);
                tw.Close();
            }
            catch
            {
            }
        }

        public List<LotteryNumber> Items { get; set; }
        public List<InputStrings> HistoryString { get; set; }

        private LotteryNumberCollection()
        {
            Items = new List<LotteryNumber>();
            HistoryString = new List<InputStrings>();
        }

        static public LotteryNumberCollection Load()
        {
            LotteryNumberCollection config = new LotteryNumberCollection();
            config = (LotteryNumberCollection)Deserialize(
                 ConfigFileName, config.GetType());

            if (config == null)
            {
                config = new LotteryNumberCollection();
                config.Save();
            }

            return config;
        }

        public void Save()
        {
            Serialize(ConfigFileName, this);
        }

        private const string ConfigFileName = "Lottery.xml";
    }

    [Serializable]
    public class InputStrings
    {
        public int Index { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
    }

    [Serializable]
    public class LotteryNumber
    {

        public int onlinenumber { get; set; }
        public int onlinechange { get; set; }
        public string onlinetime { get; set; }

        [XmlIgnore]
        public DateTime Time
        {
            get { return DateTime.Parse(onlinetime); }
        }

        public LotteryNumber Clone()
        {
            LotteryNumber num = new LotteryNumber();
            num.onlinechange = this.onlinechange;
            num.onlinenumber = this.onlinenumber;
            num.onlinetime = this.onlinetime;
            return num;
        }

        //public string OnlinetimeShort
        //{
        //    get
        //    {
        //        return onlinetime.Substring(4, onlinetime.Length - 4);
        //    }
        //}

        [XmlIgnore]
        public int[] Buffer { get; set; }

        /// <summary>
        /// 开奖实际号码
        /// </summary>
        public string ActualNum
        {
            get
            {
                int x = onlinenumber;
                int n = 0;
                int[] buffer = new int[5];
                int s = 0;
                while (x != 0)
                {
                    int y = x % 10;
                    if (n < 4) buffer[n] = y;
                    s += y;
                    x /= 10;
                    n++;
                }
                int fm = s % 10;
                buffer[4] = fm;

                Buffer = buffer;

                int rs = buffer[4] * 10000 + buffer[3] * 1000 + buffer[2] * 100 + buffer[1] * 10 + buffer[0];
                string str = rs.ToString("D5");
                return str;
            }
        }

        public int LowBit
        {
            get
            {
                string t = this.ActualNum;
                return Buffer[0];
            }
        }

        public int HightBit
        {
            get
            {
                string t = this.ActualNum;
                return Buffer[1];
            }
        }

        /// <summary>
        /// 多少期
        /// </summary>

        public string ActualIndex
        {
            get
            {
                DateTime dt = DateTime.Parse(onlinetime);
                int tm = dt.TimeOfDay.Hours * 60 + dt.TimeOfDay.Minutes;
                var t = string.Format("{0}{1}", dt.ToString("MMdd"), tm.ToString("D4"));
                return t;
            }
        }

    }

    public class LotteryCompare : IComparer<LotteryNumber>
    {
        public int Compare(LotteryNumber x, LotteryNumber y)
        {
            int m = int.Parse(x.ActualIndex);
            int n = int.Parse(y.ActualIndex);
            if (m > n)
            {
                return -1;
            }

            if (m < n)
            {
                return 1;
            }

            return 0;
        }
    }

    public class NumberTickCompare : IComparer<DisplayData>
    {
        public int Compare(DisplayData x, DisplayData y)
        {
            int m = x.Tick;
            int n = y.Tick;
            if (m > n)
            {
                return -1;
            }

            if (m < n)
            {
                return 1;
            }

            return 0;
        }
    }
}
