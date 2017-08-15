using SscLotteryTool.View;
using System;
using System.Collections.Generic;
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
    /// StatisticWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticWindow2 : DemoWindow
    {
        private Border[,] Borders;
        private Border[,] Borders1;

        private int ColumIndex = 31;
        private int RowCount = 7;

        private int Row = 4, Colum = 25;

        private TextBlock[] HighBorders;
        private TextBlock[] LowBorders;
        private TextBlock[] DestBorders;

        private TextBlock[] ZhongBorders;
        private TextBlock[] BuzhongBorders;
        private TextBlock[] ZongjiBorders;

        private Dictionary<int, List<System.Windows.Shapes.Path>> _borderList;

        public StatisticWindow2()
        {
            InitializeComponent();
            this.Loaded += StatisticWindow2_Loaded;
            Borders = new Border[RowCount, ColumIndex];
            Borders1 = new Border[Row, Colum];

            HighBorders = new TextBlock[30];
            LowBorders = new TextBlock[30];
            DestBorders = new TextBlock[30];

            ZhongBorders = new TextBlock[24];
            BuzhongBorders = new TextBlock[24];
            ZongjiBorders = new TextBlock[24];

            _borderList = new Dictionary<int, List<Path>>();
        }

        private void StatisticWindow2_Loaded(object sender, RoutedEventArgs e)
        {
            Init(Container);
            this.Start();
            if (MainWindow.IsOpenStatic)
            {
                startBlock.Text = MainWindow.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                endBlock.Text = "0000-00-00 00:00:00";
            }
            else
            {
                startBlock.Text = MainWindow.StartTime.ToString("yyyy-mm-dd HH:mm:ss");
                endBlock.Text = MainWindow.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public void Start()
        {
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
                for (int i = 0; i < MainWindow.AutoDisplays.Count; i++)
                {
                    var item = MainWindow.AutoDisplays[i];

                    ZhongBorders[i].Text = item.SuccessCount.ToString();
                    BuzhongBorders[i].Text = item.FailureCount.ToString();
                    ZongjiBorders[i].Text = (item.SuccessCount - item.FailureCount).ToString();
                }

                for (int i = 0; i < MainWindow.AutoForecastNums.Count && i < 30; i++)
                {
                    var data = MainWindow.AutoForecastNums[i];
                    HighBorders[i].Text = data.HighString;
                    LowBorders[i].Text = data.LowString;
                    DestBorders[i].Text = data.High == -1 ? "??" : string.Format("{0}{1}", data.High, data.Low);
                    if (data.High == -1)
                    {
                        DestBorders[i].ToolTip = "未开奖";
                    }
                    else
                    {
                        DestBorders[i].ToolTip = "开奖号码:" + data.Zhongjianghaoma;
                    }
                    if (data.High == -1 && data.Low == -1)
                    {
                        _borderList[i][0].Visibility = System.Windows.Visibility.Collapsed;
                        _borderList[i][1].Visibility = System.Windows.Visibility.Collapsed;
                        continue;
                    }

                    if (data.HighString == "" && data.LowString == "")
                    {
                        _borderList[i][0].Visibility = System.Windows.Visibility.Collapsed;
                        _borderList[i][1].Visibility = System.Windows.Visibility.Collapsed;
                    }
                    if (data.IsSuccess)
                    {
                        _borderList[i][0].Visibility = System.Windows.Visibility.Visible;
                        _borderList[i][1].Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        _borderList[i][0].Visibility = System.Windows.Visibility.Collapsed;
                        _borderList[i][1].Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        private void Init(Grid container)
        {
            for (int i = 0; i < ColumIndex; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (i == 0)
                {
                    cd.Width = new GridLength(36);
                }
                container.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < RowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                if (i == 0)
                {
                    row.Height = new GridLength(40);
                }

                if (i == 1 || i == 2)
                {
                    row.Height = new GridLength(60);
                }

                if (i == 3)
                {
                    row.Height = new GridLength(0);
                }

                if (i == 4)
                {
                    row.Height = new GridLength(90);
                }

                if (i == 5)
                {
                    row.Height = new GridLength(10);
                }

                container.RowDefinitions.Add(row);
            }

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumIndex; j++)
                {
                    var border = new Border()
                    {
                        BorderThickness = new Thickness(1),
                    };

                    border.BorderBrush = Brushes.Black;

                    if (i == 1 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, false);
                        border.Child = tb;
                        HighBorders[j - 1] = tb;
                    }

                    if (i == 2 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, false);
                        border.Child = tb;
                        LowBorders[j - 1] = tb;
                    }
                    if (i == 3 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, true);
                        border.Child = tb;
                        DestBorders[j - 1] = tb;
                    }

                    if (i == 4 && j > 0)
                    {
                        Grid panel = new Grid();
                        panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                        var b = new Binding
                        {
                            Source = MainWindow.RightString
                        };

                        var b1 = new Binding
                        {
                            Source = MainWindow.WrongString
                        };

                        System.Windows.Shapes.Path p = new System.Windows.Shapes.Path()
                        {
                            Stretch = Stretch.Fill,
                            Fill = Brushes.Green,
                            Visibility = Visibility.Collapsed,
                            Width = 25,
                            Height = 22
                        };

                        System.Windows.Shapes.Path p1 = new System.Windows.Shapes.Path()
                        {
                            Stretch = Stretch.Fill,
                            Fill = Brushes.Red,
                            Visibility = Visibility.Collapsed,
                            Width = 22,
                            Height = 22
                        };

                        BindingOperations.SetBinding(p, System.Windows.Shapes.Path.DataProperty, b);
                        BindingOperations.SetBinding(p1, System.Windows.Shapes.Path.DataProperty, b1);

                        panel.Children.Add(p);
                        panel.Children.Add(p1);

                        border.Child = panel;

                        _borderList.Add(j - 1, new List<System.Windows.Shapes.Path> { p, p1 });
                    }
                    if (i == 5)
                    {
                        border.SetValue(Grid.RowProperty, i);
                        border.SetValue(Grid.ColumnProperty, 0);
                        border.SetValue(Grid.ColumnSpanProperty, 31);
                        container.Children.Add(border);
                        Borders[i, j] = border;
                        break;
                    }

                    if (i == 6)
                    {
                        border.BorderBrush = null;
                        border.BorderThickness = new Thickness(0);
                        border.SetValue(Grid.RowProperty, i);
                        border.SetValue(Grid.ColumnProperty, 0);
                        border.SetValue(Grid.ColumnSpanProperty, 31);
                        container.Children.Add(border);
                        Borders[i, j] = border;
                        break;
                    }
                    else
                    {
                        border.SetValue(Grid.RowProperty, i);
                        border.SetValue(Grid.ColumnProperty, j);
                        container.Children.Add(border);
                        Borders[i, j] = border;
                    }
                }
            }

            for (int i = 0; i < 30; i++)
            {
                var b1 = Borders[0, i + 1];
                string m = string.Format("{0}", (i + 1).ToString());
                b1.Child = GetTextBlock(m);
            }


            var bd0 = Borders[1, 0];

            TextBlock tbk = GetTextBlock("十位", 15, false);
            tbk.SetValue(Grid.RowProperty, 0);
            tbk.SetValue(Grid.ColumnProperty, 0);
            bd0.Child = tbk;

            var bd1 = Borders[2, 0];

            TextBlock tb1 = GetTextBlock("个位", 15, false);
            tb1.SetValue(Grid.RowProperty, 1);
            tb1.SetValue(Grid.ColumnProperty, 0);
            bd1.Child = tb1;

            var bd3 = Borders[3, 0];
            bd3.Child = GetTextBlock("目标", 15, false);

            var bd2 = Borders[4, 0];
            bd2.Child = GetTextBlock("中奖", 15, false);


            //////////////////////////////////////////////////////////////////
            var bd4 = Borders[6, 0];

            Grid grid4 = new Grid();
            bd4.Child = grid4;

            for (int i = 0; i < Row; i++)
            {
                RowDefinition row = new RowDefinition();
                if (i == 0)
                {
                    row.Height = new GridLength(60);
                }

                grid4.RowDefinitions.Add(row);
            }

            for (int i = 0; i < Colum; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                if (i == 0)
                {
                    cd.Width = new GridLength(36);
                }
                grid4.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Colum; j++)
                {
                    var border = new Border()
                   {
                       BorderThickness = new Thickness(1),
                   };

                    if (i == 1 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, false);
                        border.Child = tb;
                        ZhongBorders[j - 1] = tb;
                    }
                    if (i == 2 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, false);
                        border.Child = tb;
                        BuzhongBorders[j - 1] = tb;

                    }
                    if (i == 3 && j > 0)
                    {
                        var tb = GetTextBlock("", 15, false);
                        border.Child = tb;
                        ZongjiBorders[j - 1] = tb;
                    }
                    border.BorderBrush = Brushes.Black;
                    border.SetValue(Grid.RowProperty, i);
                    border.SetValue(Grid.ColumnProperty, j);
                    grid4.Children.Add(border);
                    Borders1[i, j] = border;
                }
            }

            for (int i = 0; i < 24; i++)
            {
                var b1 = Borders1[0, i + 1];
                string m = string.Format("{0}H", (i + 1).ToString());
                b1.Child = GetTextBlock(m);
            }

            var bd5 = Borders1[1, 0];
            bd5.Child = GetTextBlock("中", 15, false);

            var bd6 = Borders1[2, 0];
            bd6.Child = GetTextBlock("不中", 15, false);

            var bd7 = Borders1[3, 0];
            bd7.Child = GetTextBlock("总计", 15, false);
        }

        private TextBlock GetTextBlock(string text, int size = 15, bool isRed = true)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontSize = size,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = isRed ? Brushes.Red : Brushes.Black
            };
            return t;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsOpenStatic = true;
            MainWindow.StartTime = DateTime.Now;
            startBlock.Text = MainWindow.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            endBlock.Text = "0000-00-00 00:00:00";
        }

        private void ButtonEnd_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.IsOpenStatic = false;
            MainWindow.EndTime = DateTime.Now;
            endBlock.Text = MainWindow.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
