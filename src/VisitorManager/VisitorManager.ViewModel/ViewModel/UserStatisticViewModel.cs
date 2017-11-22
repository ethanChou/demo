using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class UserStatisticViewModel : ViewModelBase
    {
        MainWindowViewModel _mainVM;
        private int _year = 2017;
        private int _beginMonth = 1;
        private int _endMonth = 12;
        private ICommand _startCmd;
        private IDocumentPaginatorSource _dataView;
        private List<string> _yearDatas = new List<string>();
        private List<string> _monthDatas = new List<string>();
        public UserStatisticViewModel()
            : this(null)
        {

        }

        public UserStatisticViewModel(MainWindowViewModel parent)
        {
            _mainVM = parent;
            for (int i = 1; i <= 12; i++)
            {
                _monthDatas.Add(i + "");
            }
            int max = Math.Max(2, DateTime.Now.Year - 2017 + 1);

            for (int i = 0; i < max; i++)
            {
                _yearDatas.Add(2017 + i + "");
            }
        }

        public class DataMonth
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int OutCount { get; set; }
            public int InnerCount { get; set; }
        }

        private void StartComand(object arg)
        {
            GC.Collect();
            DateTime dts = new DateTime(Year, BeginMonth, 1, 0, 0, 0);
            DateTime dte = new DateTime(Year, EndMonth, 1, 0, 0, 0);
            List<Visitor> datas = ThriftManager.GetVisitors(dts.Ticks, dte.Ticks);
            List<DataMonth> dmDatas = new List<DataMonth>();
            for (int i = BeginMonth; i <= EndMonth; i++)
            {
                DataMonth dm = new DataMonth() { Year = Year, Month = i };
                for (int j = 0; j < datas.Count; j++)
                {
                    var v = datas[j];
                    DateTime dt = new DateTime(v.Vt_in_time);
                    if (dt.Month == i)
                    {
                        if (v.Vt_identify_type == IdentifyType.Employee)
                        {
                            dm.InnerCount++;
                        }

                        if (v.Vt_identify_type == IdentifyType.IdCard)
                        {
                            dm.OutCount++;
                        }
                    }
                }
                dmDatas.Add(dm);
            }

            try
            {
                ReportDocument reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Doc\SimpleReport.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Templates\");
                reader.Close();

                ReportData data = new ReportData();

                // set constant document values
                data.ReportDocumentValues.Add("ReportBYear", Year); // print date is now
                data.ReportDocumentValues.Add("ReportBMonth", BeginMonth); // print date is now
                data.ReportDocumentValues.Add("ReportEMonth", EndMonth); // print date is now

                // sample table "Ean"
                DataTable table = new DataTable("Ean");
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("Month", typeof(string));
                table.Columns.Add("OutCount", typeof(string));
                table.Columns.Add("InnerCount", typeof(int));
                Random rnd = new Random(1234);
                int totalcount = 0;
                for (int i = 0; i < dmDatas.Count; i++)
                {
                    var dm = dmDatas[i];
                    // randomly create some articles
                    table.Rows.Add(new object[] { dm.Year, dm.Month, dm.OutCount, dm.InnerCount });
                    totalcount += dm.OutCount;
                    totalcount += dm.InnerCount;
                }
                data.DataTables.Add(table);

                data.ReportDocumentValues.Add("TotalCount", totalcount); // print date is now

                DateTime dateTimeStart = DateTime.Now; // start time measure here

                data.ReportDocumentValues.Add("CurrentTime", dateTimeStart.ToString("yyyy年MM月dd日 HH时mm分")); // print date is now

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                DataView = xps.GetFixedDocumentSequence();

                // show the elapsed time in window title
                //Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
            }
            catch (Exception ex)
            {
                // show exception
                //MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        public ICommand StartCmd
        {
            get { return _startCmd ?? (_startCmd = new DelegateCommand(StartComand)); }
        }

        public int EndMonth
        {
            get { return _endMonth; }
            set
            {
                _endMonth = value;
                NotifyChange("EndMonth");
            }
        }

        public int BeginMonth
        {
            get { return _beginMonth; }
            set
            {
                _beginMonth = value;
                NotifyChange("BeginMonth");

            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                NotifyChange("Year");

            }
        }

        public IDocumentPaginatorSource DataView
        {
            get { return _dataView; }
            set
            {
                _dataView = value;
                NotifyChange("DataView");
            }
        }

        public List<string> YearDatas
        {
            get { return _yearDatas; }
            set
            {
                _yearDatas = value;
                NotifyChange("YearDatas");
            }
        }

        public List<string> MonthDatas
        {
            get { return _monthDatas; }
            set
            {
                _monthDatas = value;
                NotifyChange("MonthDatas");

            }
        }
    }
}
