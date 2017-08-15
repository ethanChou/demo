using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using SscLotteryTool.View;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;

namespace SscLotteryTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : DemoWindow
    {
        private const int RowHeader = 3;
        private const int ColumnHeader = 1;
        public const string RightString = "F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z ";
        public const string WrongString = "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z ";

        private int RowCount = RowHeader + 12;
        private int ColumnCount = ColumnHeader + 30;
        private int LimitChar = 3;
        private int RefrehsInterval = 60;
        private Border[,] AllBorders;
        private Dictionary<Border, List<System.Windows.Shapes.Path>> _borderList;
        private Dictionary<int, TextBox[]> _textBoxs;
        private List<LotteryNumber> _lastDatas;
        private static LotteryNumberCollection _lotterCollection;
        private static object _lock = new object();
        public static List<RandomData> RandomHighList = new List<RandomData>();
        public static List<RandomData> RandomLowList = new List<RandomData>();
        public static LotteryNumberCollection Datas
        {
            get
            {
                return _lotterCollection;
            }
            private set
            {
                lock (_lock)
                {
                    _lotterCollection = value;
                }
            }
        }
        public event Action UpdateData;
        public static int SaveColumnCount = 12;
        public static int Start_Limit = 10;

        public static int SaveColumnCountNew = 11;
        public static int Start_LimitNew = 8;

        public static int Start_Limit_Same = 5;
        public static int SaveColumnCount_Same = 13;
        /// <summary>
        /// 连中
        /// </summary>
        public static List<DisplayData> DisplayList = new List<DisplayData>();
        /// <summary>
        /// 连杀1
        /// </summary>
        public static List<DisplayData> DisplayList1 = new List<DisplayData>();
        /// <summary>
        /// 连杀2
        /// </summary>
        public static List<DisplayData> DisplayList2 = new List<DisplayData>();

        /// <summary>
        /// 连中2 Hight
        /// </summary>
        public static List<DisplayData> DisplayList3 = new List<DisplayData>();
        /// <summary>
        /// 连中2  Low 
        /// </summary>
        public static List<DisplayData> DisplayList4 = new List<DisplayData>();
        /// <summary>
        /// 连中2 上一次，历史
        /// </summary>
        public static List<DisplayData> LastHighDatas = new List<DisplayData>();
        /// <summary>
        /// 连中2 上一次，历史
        /// </summary>
        public static List<DisplayData> LastLowDatas = new List<DisplayData>();

        public static List<ForecastNumber> AutoForecastNums = new List<ForecastNumber>();

        private static List<ForecastNumber> UserForecastNums = new List<ForecastNumber>();

        public static List<DisplayForecast> AutoDisplays = new List<DisplayForecast>();
        public static List<DisplayForecast> UserDisplays = new List<DisplayForecast>();

        private int Limit = 0;
        private bool _forecast = false;
        private bool _unforecast = false;
        private bool _unforecast2 = false;
        private bool _unforecast3 = false;
        private bool _unforecast4 = false;

        private static List<int> UserHighBits = new List<int>();
        private static List<int> UserLowBits = new List<int>();
        private static KeyValueConfig config;

        public MainWindow()
        {
            InitializeComponent();
            StartTime = DateTime.Now;

            GetRandomData();
            _lastDatas = new List<LotteryNumber>();
            Datas = LotteryNumberCollection.Load();
            _lastDatas.AddRange(Datas.Items);
            _borderList = new Dictionary<Border, List<System.Windows.Shapes.Path>>();
            _textBoxs = new Dictionary<int, TextBox[]>();
            this.Loaded += MainWindow_Loaded;
            config = new KeyValueConfig(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Config.ini"));
            LimitChar = int.Parse(config["Limitchar", "3"]);
            RowCount = int.Parse(config["RowSize", "12"]) + RowHeader;
            ColumnCount = int.Parse(config["ColumnSize", "30"]) + ColumnHeader;
            RefrehsInterval = int.Parse(config["RefrehsInterval", "60"]);
            HighNumber = config["HighNumber", ""];
            LowNumber = config["LowNumber", ""];

            if (!string.IsNullOrEmpty(HighNumber))
            {
                UserHighBits = Method.GetNumber(HighNumber);
            }
            if (!string.IsNullOrEmpty(LowNumber))
            {
                UserLowBits = Method.GetNumber(LowNumber);
            }

        }

        private static string _highNumber;

        public static string HighNumber
        {
            get { return MainWindow._highNumber; }
            set
            {
                MainWindow._highNumber = value;
                UserHighBits = Method.GetNumber(HighNumber);
                lock (_lock)
                {
                    foreach (var item in UserForecastNums)
                    {
                        item.HighBits = UserHighBits;
                    }
                }
                UpdateUserDisplays();

            }
        }
        private static string _lowNumber;

        public static string LowNumber
        {
            get { return MainWindow._lowNumber; }
            set
            {
                MainWindow._lowNumber = value;
                UserLowBits = Method.GetNumber(LowNumber);
                lock (_lock)
                {
                    foreach (var item in UserForecastNums)
                    {
                        item.LowBits = UserLowBits;
                    }
                }
                UpdateUserDisplays();
            }
        }

        public static string AutoHighNumber { get; set; }
        public static string AutoLowNumber { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AllBorders = new Border[RowCount, ColumnCount];
            Init(Container);
            LotteryStatistical stat = new LotteryStatistical(RefrehsInterval);
            stat.LotteryRefresh += Worker;
            stat.Start();
        }

        private void Worker(List<LotteryNumber> obj)
        {
            lock (_lock)
            {
                if (DateTime.Now.Day != StartTime.Day)
                {
                    _lastDatas.Clear();
                    UserForecastNums.Clear();
                    AutoForecastNums.Clear();
                    DisplayList3.Clear();
                    DisplayList4.Clear();
                    StartTime = DateTime.Now;
                }
                _lastDatas.Sort(new LotteryCompare());
                bool newNum = false;
                foreach (var d in obj)
                {
                    if (!_lastDatas.Exists(t => t.ActualIndex == d.ActualIndex))
                    {
                        if (_lastDatas.Count >= 30)
                        {
                            _lastDatas.RemoveAt(_lastDatas.Count - 1);
                        }
                        _lastDatas.Insert(0, d);
                        newNum = true;
                    }
                }

                _lastDatas.Sort(new LotteryCompare());

                LotteryNumber lastNum = _lastDatas[0];

                if (FilterData(_lastDatas))
                {
                    Console.WriteLine("Fileter Data {0},{1}", lastNum.ActualIndex, lastNum.ActualNum);
                    return;
                }

                RefreshUI();

                Datas.Items = _lastDatas;
                //连中
                StartAnalyse();
                //连杀1
                StartAnalyse1();
                //连杀2
                StartAnalyse2();

                LastHighDatas.Clear();
                foreach (var item in DisplayList3)
                {
                    LastHighDatas.Add(item.Clone());
                }

                LastLowDatas.Clear();
                foreach (var item in DisplayList4)
                {
                    LastLowDatas.Add(item.Clone());
                }
                if (IsOpenStatic)
                {
                    //ForecastNumber fnum1 = new ForecastNumber();
                    //fnum1.Time = lastNum.Time;
                    //fnum1.HighBits = UserHighBits;
                    //fnum1.LowBits = UserLowBits;
                    //fnum1.High = lastNum.HightBit;
                    //fnum1.Low = lastNum.LowBit;

                    //lock (_lock)
                    //{
                    //    UserForecastNums.Add(fnum1);
                    //}

                    //if (AutoForecastNums.Count == 40)
                    //{
                    //    AutoForecastNums.RemoveRange(35, 5);
                    //}

                    ForecastNumber fnum = new ForecastNumber();
                    fnum.Time = lastNum.Time;
                    fnum.HighBits = n1;
                    fnum.LowBits = n2;
                    fnum.High = lastNum.HightBit;
                    fnum.Low = lastNum.LowBit;

                    if (n1.Count == 0 && n2.Count == 0)
                    {

                    }
                    else
                    {
                        if (AutoForecastNums.Count > 0)
                        {
                            if (AutoForecastNums[0].HighBits.Equals(n1))
                            {
                                AutoForecastNums[0].High = lastNum.HightBit;
                                AutoForecastNums[0].Low = lastNum.LowBit;
                                AutoForecastNums[0].Zhongjianghaoma = lastNum.ActualNum;
                            }
                        }
                        else
                        {
                            fnum.Zhongjianghaoma = lastNum.ActualNum;
                            AutoForecastNums.Insert(0, fnum);
                        }
                    }

                    //UpdateUserDisplays();

                    UpdateAutoDisplays();
                }

                //连中2
                StartAnalyse3();

                n1 = GetMaxNumber(DisplayList3.ToArray());
                AutoHighNumber = n1.Count == 0 ? "" : string.Format("{0}{1}{2}", n1[0], n1[1], n1[2]);
                n2 = GetMaxNumber(DisplayList4.ToArray(), false);
                AutoLowNumber = n2.Count == 0 ? "" : string.Format("{0}{1}{2}", n2[0], n2[1], n2[2]);

                if (IsOpenStatic)
                {
                    ForecastNumber fnum1 = new ForecastNumber();
                    fnum1.Time = lastNum.Time;
                    fnum1.HighBits = n1;
                    fnum1.LowBits = n2;
                    fnum1.High = -1;
                    fnum1.Low = -1;
                    AutoForecastNums.Insert(0, fnum1);
                }


                if (newNum)
                {
                    if (UpdateData != null)
                    {
                        UpdateData();
                    }
                }

            }
            lock (this)
            {
                Datas.Save();
            }
        }

        public static bool IsOpenStatic = false;
        public static DateTime _startTime;
        public static DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                lock (_lock)
                {
                    _startTime = value;
                }
            }
        }
        public static DateTime EndTime { get; set; }

        private List<int> n1 = new List<int>(), n2 = new List<int>();

        private static void UpdateUserDisplays()
        {
            UserDisplays.Clear();

            for (int i = 1; i <= 24; i++)
            {
                List<ForecastNumber> temps = GetUserNumber(i);

                int s = GetCount(temps);
                int f = GetCount(temps, false);

                UserDisplays.Add(new DisplayForecast() { Index = i, SuccessCount = s, FailureCount = f });
            }
        }

        private void UpdateAutoDisplays()
        {
            AutoDisplays.Clear();

            for (int i = 1; i <= 24; i++)
            {
                List<ForecastNumber> temps = GetAutoNumber(i);

                int s = GetCount(temps);
                int f = GetCount(temps, false);

                AutoDisplays.Add(new DisplayForecast() { Index = i, SuccessCount = s, FailureCount = f });

                //Console.WriteLine("$$$Auto$$$$$$$$$ {0}, {1}======={2}, temp.count:{3},total: {4}", i, s, f, temps.Count, AutoForecastNums.Count);
            }
        }


        private static List<ForecastNumber> GetUserNumber(int hour)
        {
            List<ForecastNumber> result = new List<ForecastNumber>();
            DateTime endTime = StartTime.AddHours(hour);
            DateTime startTime = StartTime.AddHours(hour - 1);
            if (startTime > DateTime.Now)
            {
                return result;
            }
            DateTime temp = endTime;

            lock (_lock)
            {
                foreach (var item in UserForecastNums)
                {
                    if (item.Time <= temp && item.Time > startTime)
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private static int GetCount(List<ForecastNumber> datas, bool f = true)
        {
            return datas.Count(t => t.IsSuccess == f);
        }

        private List<ForecastNumber> GetAutoNumber(int hour)
        {
            List<ForecastNumber> result = new List<ForecastNumber>();

            DateTime endTime = StartTime.AddHours(hour);
            DateTime startTime = StartTime.AddHours(hour - 1);
            if (startTime > DateTime.Now)
            {
                return result;
            }
            DateTime temp = endTime;

            // temp = endTime > DateTime.Now ? DateTime.Now : endTime;

            foreach (var item in AutoForecastNums)
            {
                //Console.WriteLine("{0}++++++++++++{1}", item.Time, temp);

                if (item.Time < temp && item.Time >= startTime)
                {
                    result.Add(item);
                }
            }
            return result;
        }


        private void RefreshUI()
        {

            for (int i = 0; i < _lastDatas.Count && i < IndexLimit; i++)
            {
                Border b = AllBorders[0, i + 1];
                Border b1 = AllBorders[1, i + 1];

                this.Dispatcher.Invoke(new Action(() =>
                {
                    b.Child = new TextBlock() { Text = _lastDatas[i].ActualIndex, FontWeight = FontWeights.Bold, FontSize = 15, Foreground = Brushes.Red, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                    StackPanel sp = new StackPanel();
                    sp.Children.Add(new TextBlock() { Text = _lastDatas[i].ActualNum, FontWeight = FontWeights.Bold, FontSize = 15, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center });
                    sp.Children.Add(new Rectangle() { Height = 2, Stroke = Brushes.Black, Fill = Brushes.Black, StrokeThickness = 2, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch });
                    sp.Children.Add(new TextBlock() { Text = (i + 1).ToString(), FontWeight = FontWeights.Bold, FontSize = 13, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center });
                    b1.Child = sp;
                    //b1.Child = new TextBlock() { Text = _lastDatas[i].ActualNum, FontWeight = FontWeights.Bold, FontSize = 15, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                }));
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                TimeSpan ts = DateTime.Now - StartTime;
                this.Title = String.Format("{0}-{1}", "腾讯分分彩", ts.ToString(@"dd\.hh\:mm\:ss"));

                UpateAll();

                foreach (var d in _textBoxs)
                {
                    var input = Datas.HistoryString.Find(t => t.Index == d.Key);
                    if (input == null)
                    {
                        Datas.HistoryString.Add(new InputStrings()
                        {
                            Index = d.Key,
                            X = d.Value[0].Text,
                            Y = d.Value[1].Text
                        });
                    }
                    else
                    {
                        input.X = d.Value[0].Text;
                        input.Y = d.Value[1].Text;
                    }
                }

            }));
        }

        private bool FilterData(List<LotteryNumber> obj)
        {
            bool f = false;
            try
            {

                int index = 0;
                List<int> useless = new List<int>();
                while (index + 1 != obj.Count - 1)
                {
                    var num = obj[index];
                    var num1 = obj[index + 1];

                    if (num.ActualNum == num1.ActualNum)
                    {
                        useless.Add(index);
                        if (index == 0)
                        {
                            f = true;
                        }
                    }
                    index++;
                }

                for (int i = 0; i < useless.Count; i++)
                {
                    obj.RemoveAt(useless[i]);
                }

            }
            catch (Exception)
            {
            }
            return f;
        }

        private void SaveData(List<DisplayData> data, bool f)
        {
            try
            {
                string fileName = string.Format("Data\\{0}\\{1}{2}.txt", DateTime.Now.ToString("yyyyMMdd"), f ? "连中" : "连杀", DateTime.Now.ToString("HHmmss"));
                string logFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string logDir = System.IO.Path.GetDirectoryName(logFile);
                if (logDir != null && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                using (FileStream fileStream = new FileStream(logFile, FileMode.OpenOrCreate))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        streamWriter.AutoFlush = true;
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);

                        for (int i = 0; i < SaveColumnCount; i++)
                        {
                            List<DisplayData> datas = DisplayList.FindAll(t => t.Tick == i + Start_Limit);
                            string numbers = string.Empty;
                            foreach (var item in datas)
                            {
                                numbers += string.Format("十位[{0}] 个位[{1}],", item.HighData, item.LowData);
                            }

                            string str = string.Format("{0} {1},总数:{2},数据: {3}", f ? "连中" : "连杀", i + Start_Limit, datas.Count, numbers);
                            streamWriter.WriteLine(str);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to logfile: " + ex.Message);
            }
        }

        private void GetRandomData()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j != i)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            if (k != j && k != i)
                            {
                                var dt = new RandomData() { D1 = i, D2 = j, D3 = k };
                                if (!RandomLowList.Exists(t => t.IsSample(dt)))
                                {
                                    RandomLowList.Add(dt);
                                    RandomHighList.Add(dt.Clone());
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 连中
        /// </summary>
        private void StartAnalyse()
        {
            int len = MainWindow.RandomLowList.Count;
            DisplayList.Clear();

            List<LotteryNumber> items = _lastDatas;

            items.Sort(new LotteryCompare());

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    //十位
                    RandomData dh = MainWindow.RandomHighList[i];
                    //个位 随机三个数
                    RandomData dl = MainWindow.RandomLowList[j];

                    int tick = GetTick(items, dh, dl);

                    if (tick >= Start_LimitNew)
                    {

                        DisplayList.Add(new DisplayData() { HighData = dh, LowData = dl, Tick = tick });

                    }
                }
            }
        }

        private void StartAnalyse1()
        {
            int len = MainWindow.RandomLowList.Count;

            DisplayList1.Clear();
            List<LotteryNumber> items = _lastDatas;

            items.Sort(new LotteryCompare());

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    //十位
                    RandomData dh = MainWindow.RandomHighList[i];
                    //个位 随机三个数
                    RandomData dl = MainWindow.RandomLowList[j];

                    int tick = GetUnTick(items, dh, dl);

                    if (tick >= Start_Limit)
                    {

                        DisplayList1.Add(new DisplayData() { HighData = dh, LowData = dl, Tick = tick });

                    }
                }
            }
        }

        private void StartAnalyse2()
        {

            int len = MainWindow.RandomLowList.Count;

            DisplayList2.Clear();

            List<LotteryNumber> items = _lastDatas;

            items.Sort(new LotteryCompare());

            for (int i = 0; i < len; i++)
            {
                //十位
                RandomData dh = MainWindow.RandomLowList[i];
                //个位 随机三个数
                RandomData dl = MainWindow.RandomLowList[i];

                int tick = GetUnTick(items, dh, dl);

                if (tick >= Start_Limit_Same)
                {
                    DisplayList2.Add(new DisplayData() { HighData = dh, LowData = dl, Tick = tick });
                }
            }
        }

        public List<int> GetMaxNumber(DisplayData[] datas, bool f = true)
        {
            Array.Sort(datas, new NumberTickCompare());

            //datas.Sort(new NumberTickCompare());
            List<int> result = new List<int>();
            if (datas.Length == 0)
            {
                return result;
            }

            result.Add(f ? datas[0].HighData.D1 : datas[0].LowData.D1);
            result.Add(f ? datas[1].HighData.D1 : datas[1].LowData.D1);
            result.Add(f ? datas[2].HighData.D1 : datas[2].LowData.D1);
            return result;
        }

        private void StartAnalyse3()
        {
            DisplayList3.Clear();
            DisplayList4.Clear();
            List<LotteryNumber> items = _lastDatas;

            items.Sort(new LotteryCompare());

            for (int i = 0; i < 10; i++)
            {
                int tick = GetTick(items, i, true);
                DisplayList3.Add(new DisplayData() { HighData = new RandomData() { D1 = i }, Tick = tick });
            }

            for (int i = 0; i < 10; i++)
            {
                int tick = GetTick(items, i, false);
                DisplayList4.Add(new DisplayData() { LowData = new RandomData() { D1 = i }, Tick = tick });
            }
        }

        private int GetTick(List<LotteryNumber> source, int highbit, bool isHigh = true)
        {
            int tick = 0;
            int lastTick = 0;
            for (int i = 0; i < source.Count - Limit; i++)
            {
                LotteryNumber num = source[i];
                int bit = isHigh ? num.HightBit : num.LowBit;

                if (bit != highbit)
                {
                    tick++;
                }
                else
                {
                    if (tick > lastTick)
                        lastTick = tick;
                    tick = 0;
                    break;
                }
            }
            if (tick > lastTick)
            {
                lastTick = tick;
            }
            return lastTick;
        }


        private int GetTick(List<LotteryNumber> source, RandomData dh, RandomData dl)
        {
            int tick = 0;
            int lastTick = 0;
            for (int i = 0; i < source.Count - Limit; i++)
            {
                LotteryNumber num = source[i];
                if (!dh.Contains(num.HightBit) && !dl.Contains(num.LowBit))
                {
                    tick++;
                }
                else
                {
                    if (tick > lastTick)
                        lastTick = tick;
                    tick = 0;
                    break;
                }
            }
            return lastTick;
        }

        private int GetUnTick(List<LotteryNumber> source, RandomData dh, RandomData dl)
        {

            int tick = 0;
            int lastTick = 0;
            for (int i = 0; i < source.Count - Limit; i++)
            {
                LotteryNumber num = source[i];
                if (dh.Contains(num.HightBit) || dl.Contains(num.LowBit))
                {
                    tick++;
                }
                else
                {
                    if (tick > lastTick)
                        lastTick = tick;
                    tick = 0;
                    break;
                }
            }
            return lastTick;
        }

        private void Init(Grid grid)
        {
            for (int i = 0; i < RowCount; i++)
            {
                var row = new RowDefinition();
                if (i == 0) row.Height = new GridLength(35);
                if (i == 1) row.Height = new GridLength(40);
                if (i == 2) row.Height = new GridLength(20);

                grid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < ColumnCount; i++)
            {
                var coldef = new ColumnDefinition();
                if (i == 0)
                {
                    coldef.Width = new GridLength(130);
                }

                grid.ColumnDefinitions.Add(coldef);
            }

            SolidColorBrush bordBursh = new SolidColorBrush(Colors.Black);
            SolidColorBrush rigthBursh = new SolidColorBrush(Colors.Green);
            SolidColorBrush worngBursh = new SolidColorBrush(Colors.Red);
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    var border = new Border()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = bordBursh,
                    };

                    if (i >= 3 && j >= 1)
                    {
                        Grid panel = new Grid();
                        panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                        var b = new Binding
                        {
                            Source = RightString
                        };

                        var b1 = new Binding
                        {
                            Source = WrongString
                        };

                        System.Windows.Shapes.Path p = new System.Windows.Shapes.Path()
                        {
                            Stretch = Stretch.Fill,
                            Fill = rigthBursh,
                            Visibility = Visibility.Collapsed,
                            Width = 45,
                            Height = 36
                        };

                        System.Windows.Shapes.Path p1 = new System.Windows.Shapes.Path()
                        {
                            Stretch = Stretch.Fill,
                            Fill = worngBursh,
                            Visibility = Visibility.Collapsed,
                            Width = 36,
                            Height = 36
                        };

                        BindingOperations.SetBinding(p, System.Windows.Shapes.Path.DataProperty, b);
                        BindingOperations.SetBinding(p1, System.Windows.Shapes.Path.DataProperty, b1);

                        panel.Children.Add(p);
                        panel.Children.Add(p1);

                        border.Child = panel;

                        _borderList.Add(border, new List<System.Windows.Shapes.Path> { p, p1 });
                    }

                    border.SetValue(Grid.RowProperty, i);
                    border.SetValue(Grid.ColumnProperty, j);

                    grid.Children.Add(border);

                    AllBorders[i, j] = border;


                }
            }

            var bd = AllBorders[0, 0];
            bd.Child = new TextBlock() { Text = "期号", FontWeight = FontWeights.Bold, FontSize = 20, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

            bd = AllBorders[1, 0];

            Grid gridTile = new Grid();
            gridTile.RowDefinitions.Add(new RowDefinition());
            gridTile.RowDefinitions.Add(new RowDefinition());
            gridTile.RowDefinitions.Add(new RowDefinition());

            gridTile.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
            gridTile.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock tbt = new TextBlock()
                {
                    Text = "开奖号码",
                    FontWeight = FontWeights.Bold,
                    FontSize = 17,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };

            tbt.SetValue(Grid.RowProperty, 0);
            tbt.SetValue(Grid.ColumnProperty, 0);
            tbt.SetValue(Grid.ColumnSpanProperty, 2);
            tbt.SetValue(Grid.RowSpanProperty, 3);


            var bd0 = AllBorders[2, 0];
            bd0.Child = new TextBlock() { Text = "统计类别", FontWeight = FontWeights.Bold, FontSize = 15, TextAlignment = TextAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

            var bd1 = AllBorders[2, 1];
            Button btn1 = new Button() { Content = "连中", Padding = new Thickness(0), };
            btn1.Click += btn1_Click;
            btn1.SetValue(Grid.RowProperty, 0);
            btn1.SetValue(Grid.ColumnProperty, 1);
            bd1.Child = btn1;

            var bd4 = AllBorders[2, 2];
            Button btn4 = new Button() { Content = "连中2", Padding = new Thickness(0), };
            btn4.Click += btn4_Click;
            btn4.SetValue(Grid.RowProperty, 2);
            btn4.SetValue(Grid.ColumnProperty, 1);
            bd4.Child = btn4;

            var bd2 = AllBorders[2, 3];
            Button btn2 = new Button() { Content = "连杀", Padding = new Thickness(0), };
            btn2.Click += btn2_Click;
            btn2.SetValue(Grid.RowProperty, 1);
            btn2.SetValue(Grid.ColumnProperty, 1);
            bd2.Child = btn2;

            var bd3 = AllBorders[2, 4];
            Button btn3 = new Button() { Content = "连杀2", Padding = new Thickness(0), };
            btn3.Click += btn3_Click;
            btn3.SetValue(Grid.RowProperty, 2);
            btn3.SetValue(Grid.ColumnProperty, 1);
            bd3.Child = btn3;

            var bd5 = AllBorders[2, 5];
            Button btn5 = new Button() { Content = "统计", Padding = new Thickness(0), };
            btn5.Click += btn5_Click; btn5.SetValue(Grid.RowProperty, 2);
            btn5.SetValue(Grid.ColumnProperty, 1);
            bd5.Child = btn5;




            gridTile.Children.Add(tbt);

            bd.Child = gridTile;

            for (int i = 3; i < RowCount; i++)
            {
                bd = AllBorders[i, 0];

                Grid gd1 = new Grid();
                gd1.RowDefinitions.Add(new RowDefinition());
                gd1.RowDefinitions.Add(new RowDefinition());

                gd1.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
                gd1.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                gd1.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock tb = (new TextBlock() { Text = "十位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
                tb.SetValue(Grid.RowProperty, 0);
                tb.SetValue(Grid.ColumnProperty, 0);

                NumberBox txt = (new NumberBox() { Name = "one", FontSize = 15, MaxLength = LimitChar, Tag = i, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
                var input = Datas.HistoryString.Find(t => t.Index == i);
                if (input != null)
                {
                    txt.Text = input.X;
                }
                txt.TextChanged += TextBox_TextChanged;
                txt.SetValue(Grid.RowProperty, 0);
                txt.SetValue(Grid.ColumnProperty, 1);

                Button btnclear1 = new Button() { Content = "清除", Padding = new Thickness(0), Tag = txt };
                btnclear1.Click += Btnclear1_Click;
                btnclear1.SetValue(Grid.RowProperty, 0);
                btnclear1.SetValue(Grid.ColumnProperty, 2);

                TextBlock tb1 = (new TextBlock() { Text = "个位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
                tb1.SetValue(Grid.RowProperty, 1);
                tb1.SetValue(Grid.ColumnProperty, 0);

                NumberBox txt1 = (new NumberBox() { Name = "two", FontSize = 15, MaxLength = LimitChar, Tag = i, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
                if (input != null)
                {
                    txt1.Text = input.Y;
                }

                txt1.TextChanged += TextBox_TextChanged;
                txt1.SetValue(Grid.RowProperty, 1);
                txt1.SetValue(Grid.ColumnProperty, 1);

                Button btnclear2 = new Button() { Content = "清除", Padding = new Thickness(0), Tag = txt1 };
                btnclear2.SetValue(Grid.RowProperty, 1);
                btnclear2.SetValue(Grid.ColumnProperty, 2);
                btnclear2.Click += Btnclear1_Click;

                _textBoxs.Add(i, new TextBox[] { txt, txt1 });

                gd1.Children.Add(tb);
                gd1.Children.Add(txt);
                gd1.Children.Add(btnclear1);
                gd1.Children.Add(tb1);
                gd1.Children.Add(txt1);
                gd1.Children.Add(btnclear2);
                bd.Child = gd1;
            }
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            if (_unforecast4) return;

            StatisticWindow2 fw = new StatisticWindow2();
            this.UpdateData += fw.Start;
            fw.Closed += fw_Closed;
            fw.Show();
            _unforecast4 = true;
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            if (_unforecast3) return;

            SingleBitForecastWindow fw = new SingleBitForecastWindow();
            this.UpdateData += fw.Start;
            fw.Closed += fw_Closed;
            fw.Show();
            _unforecast3 = true;
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            if (_unforecast2) return;

            UnForecastWindow2 fw = new UnForecastWindow2();
            this.UpdateData += fw.Start;
            fw.Closed += fw_Closed;
            //fw.Owner = this;
            fw.Show();
            _unforecast2 = true;
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            if (_unforecast) return;

            UnForecastWindow fw = new UnForecastWindow();
            this.UpdateData += fw.Start;
            fw.Closed += fw_Closed;
            //fw.Owner = this;
            fw.Show();
            _unforecast = true;
        }

        private void fw_Closed(object sender, EventArgs e)
        {
            if (sender is UnForecastWindow)
            {
                _unforecast = false;
                this.UpdateData -= ((UnForecastWindow)sender).Start;
            }

            if (sender is ForecastWindow)
            {
                _forecast = false;
                this.UpdateData -= ((ForecastWindow)sender).Start;
            }

            if (sender is UnForecastWindow2)
            {
                _unforecast2 = false;
                this.UpdateData -= ((UnForecastWindow2)sender).Start;
            }

            if (sender is SingleBitForecastWindow)
            {
                _unforecast3 = false;
                this.UpdateData -= ((SingleBitForecastWindow)sender).Start;
            }

            if (sender is StatisticWindow2)
            {
                _unforecast4 = false;
                this.UpdateData -= ((StatisticWindow2)sender).Start;
                //IsOpenStatic = false;
            }
            this.Activate();
            this.BringIntoView();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (_forecast) return;
            ForecastWindow fw = new ForecastWindow();
            this.UpdateData += fw.Start;
            fw.Closed += fw_Closed;
            //fw.Owner = this;
            fw.Show();
            _forecast = true;
        }

        private void Btnclear1_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                TextBox txt = btn.Tag as TextBox;
                txt.Text = "";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;

            if (box != null)
            {
                int index = (int)box.Tag;
                TextBox[] boxs = _textBoxs[index];
                Upate(boxs, index);
            }
        }

        private void UpateAll()
        {
            foreach (var b in _textBoxs)
            {
                Upate(b.Value, b.Key);
            }
        }
        int IndexLimit = 30;
        private void Upate(TextBox[] b, int index)
        {
            TextBox[] boxs = b;
            //十位
            List<int> buffer1 = new List<int>(3);
            //个位
            List<int> buffer2 = new List<int>(3);

            int sw = -1;
            if (string.IsNullOrEmpty(boxs[0].Text) && string.IsNullOrEmpty(boxs[1].Text))
            {
                for (int i = 0; i < _lastDatas.Count && i < IndexLimit; i++)
                {
                    Border bor = AllBorders[index, i + 1];
                    List<System.Windows.Shapes.Path> paths = _borderList[bor];
                    paths[0].Visibility = System.Windows.Visibility.Collapsed;
                    paths[1].Visibility = System.Windows.Visibility.Collapsed;//叉叉
                }
                return;
            }
            if (!string.IsNullOrEmpty(boxs[0].Text))
            {
                if (boxs[0].Text.StartsWith("0"))
                {
                    buffer1.Add(0);
                }
                sw = int.Parse(boxs[0].Text);
            }

            int gw = -1;
            if (!string.IsNullOrEmpty(boxs[1].Text))
            {
                if (boxs[1].Text.StartsWith("0"))
                {
                    buffer2.Add(0);
                }
                gw = int.Parse(boxs[1].Text);
            }

            int len1 = 0;
            int len2 = 0;

            //if (sw == 0)
            //{
            //    buffer1.Add(0);
            //}

            //if (gw == 0)
            //{
            //    buffer2.Add(0);
            //}

            while (sw > 0)
            {
                int y = sw % 10;
                if (len1 < 3) buffer1.Add(y);
                sw /= 10;
                len1++;
            }

            while (gw > 0)
            {
                int y = gw % 10;
                if (len2 < 3) buffer2.Add(y);
                gw /= 10;
                len2++;
            }

            for (int i = 0; i < _lastDatas.Count && i < IndexLimit; i++)
            {
                var dt = _lastDatas[i];

                int m1 = dt.Buffer[1];//十
                int m2 = dt.Buffer[0];//个

                Border bor = AllBorders[index, i + 1];

                List<System.Windows.Shapes.Path> paths = _borderList[bor];

                if (!buffer1.Exists(t => t == m1) && !buffer2.Exists(t => t == m2))
                {
                    paths[0].Visibility = System.Windows.Visibility.Visible;
                    paths[1].Visibility = System.Windows.Visibility.Collapsed;//叉叉
                }
                else
                {
                    paths[0].Visibility = System.Windows.Visibility.Collapsed;
                    paths[1].Visibility = System.Windows.Visibility.Visible;//對勾
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            try
            {
                config["HighNumber"] = HighNumber;
                config["LowNumber"] = LowNumber;

                foreach (var d in _textBoxs)
                {
                    var input = Datas.HistoryString.Find(t => t.Index == d.Key);
                    if (input == null)
                    {
                        Datas.HistoryString.Add(new InputStrings()
                        {
                            Index = d.Key,
                            X = d.Value[0].Text,
                            Y = d.Value[1].Text
                        });
                    }
                    else
                    {
                        input.X = d.Value[0].Text;
                        input.Y = d.Value[1].Text;
                    }
                }

                lock (this)
                {
                    Datas.Save();
                }
            }
            catch (Exception)
            {

            }

            Process.GetCurrentProcess().Kill();
        }
    }
}
