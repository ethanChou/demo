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
        private int _endyear = 2017;
        private int _beginMonth = 1;
        private int _endMonth = 12;
        private ICommand _startCmd;
        private IDocumentPaginatorSource _dataView;
        private List<string> _yearDatas = new List<string>();
        private List<string> _monthDatas = new List<string>();
        private int _statisticIndex = 0;
        private bool _showDayConditon = true;
        private DateTime _beginDate = DateTime.Now.AddDays(-7);
        private DateTime _endDate = DateTime.Now;

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
            int max = Math.Max(5, DateTime.Now.Year - 2017 + 1);

            for (int i = 0; i < max; i++)
            {
                _yearDatas.Add(2017 + i + "");
            }
        }

        public class DataMonth
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int IdCount { get; set; }
            public int OutCount { get; set; }
            public int IntCount { get; set; }
            public int OtherCount { get; set; }
            public int Count { get; set; }
        }

        public class DataDay
        {
            public int Year { get; set; }

            public string Date { get; set; }

            public int IdCount { get; set; }
            public int OutCount { get; set; }
            public int IntCount { get; set; }
            public int OtherCount { get; set; }
            public int Count { get; set; }
        }

        private void StartComand(object arg)
        {
            GC.Collect();
            if (StatisticIndex == 0)
            {
                StatisticByMonth();
            }

            if (StatisticIndex == 1)
            {
                StatisticByDay();
            }

        }

        private void StatisticByDay()
        {
            int totalcount = 0;
            List<DataDay> datas = new List<DataDay>();

            for (DateTime i = BeginDate; i <= new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59); i = i.AddDays(1))
            {
                DataDay dd = new DataDay();
                var bt = new DateTime(i.Year, i.Month, i.Day, 0, 0, 0);
                var et = new DateTime(i.Year, i.Month, i.Day, 23, 59, 59);
                dd.Year = i.Year;
                dd.Date = i.ToString("MM-dd");
                dd.IdCount = GetIdCount(bt, et);
                dd.IntCount = GetInCount(bt, et);
                dd.OutCount = GetOutCount(bt, et);
                dd.OtherCount = GetOtherCount(bt, et);
                dd.Count = GetCount(bt, et);
                totalcount += dd.Count;

                datas.Add(dd);
            }

            try
            {
                ReportDocument reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Doc\SimpleReportByDay.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Doc\");
                reader.Close();

                ReportData data = new ReportData();

                // set constant document values
                //data.ReportDocumentValues.Add("ReportBYear", Year); // print date is now
                data.ReportDocumentValues.Add("ReportBMonth", BeginDate.ToString("yyyy-MM-dd")); // print date is now
                data.ReportDocumentValues.Add("ReportEMonth", EndDate.ToString("yyyy-MM-dd")); // print date is now

                // sample table "Ean"
                DataTable table = new DataTable("Ean");
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("Date", typeof(string));
                table.Columns.Add("IdCount", typeof(int));
                table.Columns.Add("OutCount", typeof(int));
                table.Columns.Add("IntCount", typeof(int));
                table.Columns.Add("OtherCount", typeof(int));
                table.Columns.Add("Count", typeof(int));


                Random rnd = new Random(1234);
              
                for (int i = 0; i < datas.Count; i++)
                {
                    var dm = datas[i];
                    // randomly create some articles
                    table.Rows.Add(new object[] { dm.Year, dm.Date, dm.IdCount, dm.OutCount, dm.IntCount, dm.OtherCount, dm.Count });

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
                throw ex;
                // show exception
                //MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void StatisticByMonth()
        {
            int totalcount = 0;
            DateTime dts = new DateTime(Year, BeginMonth, 1, 0, 0, 0);
            DateTime dte = new DateTime(Endyear, EndMonth, DateTime.DaysInMonth(Year,EndMonth), 23, 59, 59);
            List<DataMonth> datas = new List<DataMonth>();

            for (DateTime i = dts; i <= new DateTime(dte.Year, dte.Month, dte.Day, 23, 59, 59); i = i.AddMonths(1))
            {
                DataMonth dd = new DataMonth();
                var bt = new DateTime(i.Year, i.Month, 1, 0, 0, 0);
                var et = new DateTime(i.Year, i.Month, DateTime.DaysInMonth(i.Year, i.Month), 23, 59, 59);
                dd.Year = i.Year;
                dd.Month =i.Month;
                dd.IdCount = GetIdCount(bt, et);
                dd.IntCount = GetInCount(bt, et);
                dd.OutCount = GetOutCount(bt, et);
                dd.OtherCount = GetOtherCount(bt, et);
                dd.Count = GetCount(bt, et);
                totalcount += dd.Count;
                datas.Add(dd);
            }
            try
            {
                ReportDocument reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Doc\SimpleReportByMonth.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Doc\");
                reader.Close();

                ReportData data = new ReportData();

                // set constant document values
                data.ReportDocumentValues.Add("ReportBYear", Year); // print date is now
                data.ReportDocumentValues.Add("ReportBMonth", BeginMonth); // print date is now
                data.ReportDocumentValues.Add("ReportEMonth",EndMonth); // print date is now

                // sample table "Ean"
                DataTable table = new DataTable("Ean");
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("Month", typeof(string));
                table.Columns.Add("IdCount", typeof(int));
                table.Columns.Add("OutCount", typeof(int));
                table.Columns.Add("IntCount", typeof(int));
                table.Columns.Add("OtherCount", typeof(int));
                table.Columns.Add("Count", typeof(int));


                Random rnd = new Random(1234);
             
                for (int i = 0; i < datas.Count; i++)
                {
                    var dm = datas[i];
                    // randomly create some articles
                    table.Rows.Add(new object[] { dm.Year, dm.Month, dm.IdCount, dm.OutCount, dm.IntCount, dm.OtherCount, dm.Count });

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
                throw ex;
                // show exception
                //MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private int GetCount(DateTime bt, DateTime et)
        {
            string where = "where vt_in_time>=" + bt.Ticks + " and vt_in_time<=" + et.Ticks;
            string sql = "select count(*) from visitor " + where;
            DataCount dc = ThriftManager.GetCount(sql);
            if (dc.IsSeccessd) return dc.Count;

            return 0;
        }


        private int GetIdCount(DateTime bt, DateTime et)
        {
            string where = "where vt_identify_type=0 and vt_in_time>=" + bt.Ticks + " and vt_in_time<=" + et.Ticks;
            string sql = "select count(*) from visitor " + where;
            DataCount dc = ThriftManager.GetCount(sql);
            if (dc.IsSeccessd) return dc.Count;

            return 0;
        }

        private int GetOtherCount(DateTime bt, DateTime et)
        {
            string where = "where vt_identify_type=2 and vt_in_time>=" + bt.Ticks + " and vt_in_time<=" + et.Ticks;
            string sql = "select count(*) from visitor " + where;
            DataCount dc = ThriftManager.GetCount(sql);
            if (dc.IsSeccessd) return dc.Count;

            return 0;
        }

        private int GetOutCount(DateTime bt, DateTime et)
        {
            string idno = "\"" + ThriftManager.Adrelation.Lnl_id + "%\"";

            string where = "where vt_identify_type=1 and vt_identify_no not like " + idno + " and vt_in_time>=" + bt.Ticks + " and vt_in_time<=" + et.Ticks;
            string sql = "select count(*) from visitor " + where;
            DataCount dc = ThriftManager.GetCount(sql);
            if (dc.IsSeccessd) return dc.Count;

            return 0;
        }

        private int GetInCount(DateTime bt, DateTime et)
        {
            string idno = "\"" + ThriftManager.Adrelation.Lnl_id + "%\"";
            string where = "where vt_identify_type=1 and vt_identify_no like " + idno + " and vt_in_time>=" + bt.Ticks + " and vt_in_time<=" + et.Ticks;
            string sql = "select count(*) from visitor " + where;
            DataCount dc = ThriftManager.GetCount(sql);
            if (dc.IsSeccessd) return dc.Count;

            return 0;
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

        public int StatisticIndex
        {
            get { return _statisticIndex; }
            set
            {
                _statisticIndex = value;
                if (_statisticIndex == 0) ShowDayConditon = true;
                if (_statisticIndex == 1) ShowDayConditon = false;
                NotifyChange("StatisticIndex");
            }
        }

        public bool ShowDayConditon
        {
            get { return _showDayConditon; }
            set
            {
                _showDayConditon = value;
                NotifyChange("ShowDayConditon");

            }
        }

        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                NotifyChange("BeginDate");
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyChange("EndDate");
            }
        }

        public int Endyear
        {
            get { return _endyear; }
            set
            {
                _endyear = value;
                NotifyChange("Endyear");
                
            }
        }
    }
}
