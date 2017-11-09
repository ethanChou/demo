using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VisitorManager.ViewModel
{
    public class VisitorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ThriftCommon.Status vs = (ThriftCommon.Status)Enum.Parse(typeof(ThriftCommon.Status), value.ToString());
            int res = (int)vs;
           
            if (res == 1) return "正在访问";
            if (res == 2) return "已经离开";
            if (res == 3) return "逾期未还";
            if (res == 4) return "卡丢失";

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
