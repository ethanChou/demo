using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VisitorManager.ViewModel
{
    public class VisitorStatusToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;

            ThriftCommon.Status vs = (ThriftCommon.Status)Enum.Parse(typeof(ThriftCommon.Status), value.ToString());
            int res = (int)vs;

            //正在访问和逾期未还才可用
            if (res == 1 || res == 4) return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
