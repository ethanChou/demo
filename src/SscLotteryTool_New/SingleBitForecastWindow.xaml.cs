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
    /// SingleBitForecastWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SingleBitForecastWindow : DemoWindow
    {
        public SingleBitForecastWindow()
        {
            InitializeComponent();

            Borders = new Border[RowCount, ColumIndex];
            Init(Container);

            this.Start();
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
                for (int i = 0; i < MainWindow.LastHighDatas.Count; i++)
                {
                    var d = MainWindow.LastHighDatas[i];
                    int num = d.HighData.D1;

                    var border = Borders[1, i + 1];

                    TextBlock tb = border.Child as TextBlock;
                    tb.Text = d.Tick.ToString();
                }

                for (int i = 0; i < MainWindow.LastLowDatas.Count; i++)
                {
                    var d = MainWindow.LastLowDatas[i];
                    int num = d.LowData.D1;

                    var border = Borders[4, i + 1];

                    TextBlock tb = border.Child as TextBlock;
                    tb.Text = d.Tick.ToString();
                }

                var data1 = MainWindow.DisplayList3;
                var data2 = MainWindow.DisplayList4;

                for (int i = 0; i < data1.Count; i++)
                {
                    var d = data1[i];
                    int num = d.HighData.D1;

                    var border = Borders[2, i + 1];

                    TextBlock tb = border.Child as TextBlock;
                    tb.Text = d.Tick.ToString();
                }

                for (int i = 0; i < data2.Count; i++)
                {
                    var d = data2[i];
                    int num = d.LowData.D1;

                    var border = Borders[5, i + 1];

                    TextBlock tb = border.Child as TextBlock;
                    tb.Text = d.Tick.ToString();
                }



            }
        }

        private int RowCount = 6;
        private SolidColorBrush BordBursh = new SolidColorBrush(Colors.Black);
        private Border[,] Borders;
        private int ColumIndex = 11;

        private void Init(Grid container)
        {

            for (int i = 0; i < ColumIndex; i++)
            {
                container.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < RowCount; i++)
            {
                RowDefinition row;

                if (i == 0)
                {
                    row = new RowDefinition() { Height = new GridLength(60) };
                }
                else if (i == 3)
                {
                    row = new RowDefinition() { Height = new GridLength(60) };
                }
                else
                {
                    row = new RowDefinition();
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
                        BorderBrush = BordBursh,
                    };

                    if (j > 0)
                    {
                        if (i == 0 || i == 3)
                            border.Child = GetTextBlock(string.Format("{0}", j - 1));
                        else if (i == 1 || i == 4)
                            border.Child = GetTextBlock2("");
                        else
                            border.Child = GetTextBlock("", false);
                    }
                    else
                    {
                        if (i == 0)
                            border.Child = GetTextBlock1("十位");

                        else if (i == 3)
                            border.Child = GetTextBlock1("个位");
                        else if (i == 1 || i == 4)
                            border.Child = GetTextBlock1("历史");
                        else
                            border.Child = GetTextBlock1("本次");
                    }

                    border.SetValue(Grid.RowProperty, i);
                    border.SetValue(Grid.ColumnProperty, j);
                    container.Children.Add(border);
                    Borders[i, j] = border;
                }
            }
        }

        private TextBlock GetTextBlock(string text, bool isRed = true)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = isRed ? Brushes.Red : Brushes.Black
            };
            return t;
        }

        private TextBlock GetTextBlock1(string text)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = Brushes.Black
            };
            return t;
        }

        private TextBlock GetTextBlock2(string text)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = Brushes.Gray
            };
            return t;
        }
    }
}
