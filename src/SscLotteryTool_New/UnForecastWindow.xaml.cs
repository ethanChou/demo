using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SscLotteryTool
{
    /// <summary>
    /// UnForecastWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnForecastWindow : DemoWindow
    {
     
        //private List<DisplayData> DisplayList = new List<DisplayData>();
        private const int Head = 1;
        private int RowCount ;
        //int ColumnCount = 10;
        private SolidColorBrush BordBursh = new SolidColorBrush(Colors.Black);
        private Border[,] Borders;
        private Dictionary<string, DoubleTextBox> TextBoxs;
        Dictionary<int, int> result = new Dictionary<int, int>();
        private int Limit = 0;

        public UnForecastWindow()
        {
            InitializeComponent();

            //KeyValueConfig config = new KeyValueConfig(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Config.ini"));
            //Start_Limit = int.Parse(config["UnStartLimit", "11"]);
            //ColumnCount = int.Parse(config["UnColumnCount", "10"]);
            
            Start();
            this.Loaded += UnForecastWindow_Loaded;
        }

        void UnForecastWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Container.ActualWidth.ToString());
            //MessageBox.Show(this.scrollViewer1.ActualWidth.ToString());
        }

        public void Start()
        {
            //StartAnalyse(false);
            //SaveData(DisplayList);

            result.Clear();
            for (int i = 0; i < MainWindow.SaveColumnCount; i++)
            {
                List<DisplayData> datas = MainWindow.DisplayList1.FindAll(t => t.Tick == i + MainWindow.Start_Limit);
                Debug.WriteLine("{0} 连杀： {1}", i + MainWindow.Start_Limit, datas.Count);
                result.Add(i + MainWindow.Start_Limit, datas.Count);
            }
            int max = 0; int index;
            foreach (var item in result)
            {
                if (item.Value > max)
                {
                    max = item.Value;
                    index = item.Key;
                }
            }

            if (max < 14) max = 14;


            RowCount = max + Head;

            Borders = new Border[RowCount, MainWindow.SaveColumnCount];
            if (TextBoxs == null) TextBoxs = new Dictionary<string, DoubleTextBox>();
            TextBoxs.Clear();
            StartCore();
        }

        private void StartCore()
        {
            if (System.Threading.Thread.CurrentThread != this.Dispatcher.Thread)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    StartCore();
                }));
            }
            else
            {
                try
                {
                    Container.Children.Clear(); ;
                    Container.RowDefinitions.Clear();
                    Container.ColumnDefinitions.Clear();

                    Container.Height = RowCount * 50;

                    Init(Container);

                    for (int i = 0; i < MainWindow.SaveColumnCount; i++)
                    {
                        //List<DisplayData> datas = MainWindow.DisplayList1.FindAll(t => t.Tick == i + MainWindow.Start_Limit);
                        List<DisplayData> datas = Find(i + MainWindow.Start_Limit);

                        for (int j = 0; j < datas.Count; j++)
                        {
                            if (TextBoxs.ContainsKey(string.Format("{0}-{1}", j + 1, i)))
                            {
                                DoubleTextBox tb = TextBoxs[string.Format("{0}-{1}", j + 1, i)];
                                tb.Box1.Text = datas[j].HighData.ToString();
                                tb.Box2.Text = datas[j].LowData.ToString();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private List<DisplayData> Find(int limit)
        {
            List<DisplayData> t = new List<DisplayData>();
            for (int i = 0; i < MainWindow.DisplayList1.Count; i++)
            {
                var item = MainWindow.DisplayList1[i];
                if (item.Tick == limit)
                {
                    t.Add(item);
                }
            }
            return t;
        }

        public void Init(Grid container)
        {
            for (int i = 0; i < MainWindow.SaveColumnCount; i++)
            {
                container.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < RowCount; i++)
            {
                if (i != 0)
                    container.RowDefinitions.Add(new RowDefinition());
                else
                    container.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });
            }

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < MainWindow.SaveColumnCount; j++)
                {
                    var border = new Border()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = BordBursh,
                    };

                    if (i == 0)
                    {
                        border.Child = GetTextBlock(string.Format("{0}连杀[{1}]", j + MainWindow.Start_Limit, result[j + MainWindow.Start_Limit]));
                    }
                    else
                    {
                        border.Child = GetDisplayContext(i, j);
                    }

                    border.SetValue(Grid.RowProperty, i);
                    border.SetValue(Grid.ColumnProperty, j);
                    container.Children.Add(border);
                    Borders[i, j] = border;
                }
            }
        }

        private TextBlock GetTextBlock(string text)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = Brushes.Red
            };
            return t;
        }

        private Grid GetDisplayContext(int r, int c)
        {
            Grid gd1 = new Grid();
            gd1.RowDefinitions.Add(new RowDefinition());
            gd1.RowDefinitions.Add(new RowDefinition());

            gd1.ColumnDefinitions.Add(new ColumnDefinition());
            gd1.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock tb = (new TextBlock() { Text = "十位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tb.SetValue(Grid.RowProperty, 0);
            tb.SetValue(Grid.ColumnProperty, 0);

            TextBox txt = (new TextBox() { Name = "one", FontSize = 15, IsReadOnly = true, Margin = new Thickness(0), VerticalAlignment = System.Windows.VerticalAlignment.Stretch });
            txt.SetValue(Grid.RowProperty, 0);
            txt.SetValue(Grid.ColumnProperty, 1);

            TextBlock tb1 = (new TextBlock() { Text = "个位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tb1.SetValue(Grid.RowProperty, 1);
            tb1.SetValue(Grid.ColumnProperty, 0);

            TextBox txt1 = (new TextBox() { Name = "two", FontSize = 15, IsReadOnly = true, Margin = new Thickness(0), VerticalAlignment = System.Windows.VerticalAlignment.Stretch });
            txt1.SetValue(Grid.RowProperty, 1);
            txt1.SetValue(Grid.ColumnProperty, 1);

            gd1.Children.Add(tb);
            gd1.Children.Add(txt);
            gd1.Children.Add(tb1);
            gd1.Children.Add(txt1);

            TextBoxs.Add(string.Format("{0}-{1}", r, c), new DoubleTextBox() { RowIndex = r, ColumnIndex = c, Box1 = txt, Box2 = txt1 });
            return gd1;
        }
    }
}
