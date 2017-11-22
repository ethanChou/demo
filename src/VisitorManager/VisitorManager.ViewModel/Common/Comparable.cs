using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class TimeComparer : IComparer<Visitor>
    {
        private readonly bool _isAsc;//升序排列

        public TimeComparer(bool isAsc)
        {
            _isAsc = isAsc;
        }
        public int Compare(Visitor x, Visitor y)
        {
            if (x == null || y == null) return 0;

            if (x.Vt_in_time > y.Vt_in_time)
            {
                return 1*(_isAsc ? 1 : -1);
            }
            else if (x.Vt_in_time == y.Vt_in_time)
            {
                return 0;
            }
            else
            {
                return -1 * (_isAsc ? 1 : -1);
            }
        }
    }

    public class NameComparer : IComparer<Visitor>
    {
        private readonly bool _isAsc;//升序排列
        public NameComparer(bool isAsc)
        {
            _isAsc = isAsc;
        }
        public int Compare(Visitor x, Visitor y)
        {
            if (x == null || y == null)
                throw new ArgumentException("Parameters can't be null");
            CultureInfo culture = new CultureInfo("zh-cn");
            return String.Compare(x.Vt_name, y.Vt_name, true, culture) * (_isAsc ? 1 : -1);
        }
    }

    public class CardIdCompare : IComparer<Visitor>
    {
        public int Compare(Visitor x, Visitor y)
        {
            throw new NotImplementedException();
        }
    }

}
