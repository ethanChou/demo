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
    public partial class StatisticWindow : DemoWindow
    {
        private Border[,] Borders;

        private int ColumIndex = 25;
        private int RowCount = 9;

        public StatisticWindow()
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
                Highbox.Text = MainWindow.AutoHighNumber;
                Lowbox.Text = MainWindow.AutoLowNumber;

                for (int i = 1; i <= MainWindow.UserDisplays.Count; i++)
                {
                    var item = MainWindow.UserDisplays[i - 1];
                    var b1 = Borders[1, i];
                    var b2 = Borders[2, i];
                    var b3 = Borders[3, i];
                    if (b1.Child == null)
                        b1.Child = GetTextBlock("", 15, false);

                    if (b2.Child == null)
                        b2.Child = GetTextBlock("", 15, false);

                    if (b3.Child == null)
                        b3.Child = GetTextBlock("", 15, false);

                    ((TextBlock)(b1.Child)).Text = item.SuccessCount.ToString();
                    ((TextBlock)(b2.Child)).Text = item.FailureCount.ToString();
                    ((TextBlock)(b3.Child)).Text = (item.SuccessCount - item.FailureCount).ToString();
                    //Console.WriteLine("###User######## {0}, {1}======={2}", i, item.SuccessCount, item.FailureCount);
                }

                for (int i = 1; i <= MainWindow.AutoDisplays.Count; i++)
                {
                    var item = MainWindow.AutoDisplays[i - 1];

                    var b1 = Borders[5, i];
                    var b2 = Borders[6, i];
                    var b3 = Borders[7, i];
                    if (b1.Child == null)
                        b1.Child = GetTextBlock("", 15, false);

                    if (b2.Child == null)
                        b2.Child = GetTextBlock("", 15, false);

                    if (b3.Child == null)
                        b3.Child = GetTextBlock("", 15, false);

                    ((TextBlock)(b1.Child)).Text = item.SuccessCount.ToString();
                    ((TextBlock)(b2.Child)).Text = item.FailureCount.ToString();
                    ((TextBlock)(b3.Child)).Text = (item.SuccessCount - item.FailureCount).ToString();

                    //Console.WriteLine("###Auto######## {0}, {1}======={2}",i,item.SuccessCount,item.FailureCount);
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
                    cd.Width = new GridLength(80);
                }
                container.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < RowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                if (i == 0 || i == 4)
                {
                    row.Height = new GridLength(60);
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
                    if (i == 4 && j == 0)
                    {
                        border.BorderBrush = Brushes.Red;
                    }
                    else
                    {
                        border.BorderBrush = Brushes.Black;
                    }
                    border.SetValue(Grid.RowProperty, i);
                    border.SetValue(Grid.ColumnProperty, j);
                    container.Children.Add(border);
                    Borders[i, j] = border;
                }
            }

            for (int i = 0; i < 24; i++)
            {
                var b = Borders[0, i + 1];
                var b1 = Borders[4, i + 1];
                string m = string.Format("{0}H", (i + 1).ToString());
                b.Child = GetTextBlock(m);
                b1.Child = GetTextBlock(m);
            }
            var bd0 = Borders[0, 0];
            Grid gd1 = new Grid();
            bd0.Child = gd1;
            gd1.RowDefinitions.Add(new RowDefinition());
            gd1.RowDefinitions.Add(new RowDefinition());
            gd1.ColumnDefinitions.Add(new ColumnDefinition());
            gd1.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock tb = (new TextBlock() { Text = "十位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tb.SetValue(Grid.RowProperty, 0);
            tb.SetValue(Grid.ColumnProperty, 0);

            NumberBox txt = (new NumberBox() { Name = "one", FontSize = 15, MaxLength = 3, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
            txt.Text = MainWindow.HighNumber;
            txt.TextChanged += txt_TextChanged;
            txt.SetValue(Grid.RowProperty, 0);
            txt.SetValue(Grid.ColumnProperty, 1);

            TextBlock tb1 = (new TextBlock() { Text = "个位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tb1.SetValue(Grid.RowProperty, 1);
            tb1.SetValue(Grid.ColumnProperty, 0);

            NumberBox txt1 = (new NumberBox() { Name = "two", FontSize = 15, MaxLength = 3, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
            txt1.Text = MainWindow.LowNumber;
            txt1.TextChanged += txt1_TextChanged;
            txt1.SetValue(Grid.RowProperty, 1);
            txt1.SetValue(Grid.ColumnProperty, 1);

            gd1.Children.Add(tb);
            gd1.Children.Add(txt);

            gd1.Children.Add(tb1);
            gd1.Children.Add(txt1);

            var bd1 = Borders[1, 0];
            bd1.Child = GetTextBlock("中", 15, false);

            var bd2 = Borders[2, 0];
            bd2.Child = GetTextBlock("不中", 15, false);

            var bd3 = Borders[3, 0];
            bd3.Child = GetTextBlock("总计", 15, false);

            var bd4 = Borders[4, 0];
            //bd4.Child = GetTextBlock("自动", 18, false);
            Grid gd2 = new Grid();
            bd4.Child = gd2;
            gd2.RowDefinitions.Add(new RowDefinition());
            gd2.RowDefinitions.Add(new RowDefinition());
            gd2.ColumnDefinitions.Add(new ColumnDefinition());
            gd2.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock tbq = (new TextBlock() { Text = "十位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tbq.SetValue(Grid.RowProperty, 0);
            tbq.SetValue(Grid.ColumnProperty, 0);

            NumberBox txtq = (new NumberBox() { Name = "one", FontSize = 15, MaxLength = 3, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
            Highbox = txtq;
            txtq.IsReadOnly = true;
            txtq.Text = MainWindow.AutoHighNumber;

            txtq.SetValue(Grid.RowProperty, 0);
            txtq.SetValue(Grid.ColumnProperty, 1);

            TextBlock tb1q = (new TextBlock() { Text = "个位", FontSize = 15, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextAlignment = TextAlignment.Center });
            tb1q.SetValue(Grid.RowProperty, 1);
            tb1q.SetValue(Grid.ColumnProperty, 0);

            NumberBox txt1q = (new NumberBox() { Name = "two", FontSize = 15, MaxLength = 3, VerticalContentAlignment = System.Windows.VerticalAlignment.Center });
            txt1q.IsReadOnly = true;
            Lowbox = txt1q;
            txt1q.Text = MainWindow.AutoLowNumber;
            
            txt1q.SetValue(Grid.RowProperty, 1);
            txt1q.SetValue(Grid.ColumnProperty, 1);

            gd2.Children.Add(tbq);
            gd2.Children.Add(txtq);

            gd2.Children.Add(tb1q);
            gd2.Children.Add(txt1q);

            var bd5 = Borders[5, 0];
            bd5.Child = GetTextBlock("中", 15, false);

            var bd6 = Borders[6, 0];
            bd6.Child = GetTextBlock("不中", 15, false);

            var bd7 = Borders[7, 0];
            bd7.Child = GetTextBlock("总计", 15, false);
        }

        private NumberBox Highbox;
        private NumberBox Lowbox;

        private void txt1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (MainWindow.LowNumber != tb.Text)
            {
                MainWindow.LowNumber = tb.Text;
                Start();
            }
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (MainWindow.HighNumber != tb.Text)
            {
                MainWindow.HighNumber = tb.Text;
                Start();
            }
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
    }
}
