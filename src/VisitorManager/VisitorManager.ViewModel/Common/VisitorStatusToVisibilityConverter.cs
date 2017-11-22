using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VisitorManager.ViewModel
{
    public class VisitorStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            ThriftCommon.Status vs = (ThriftCommon.Status)Enum.Parse(typeof(ThriftCommon.Status), value.ToString());
            int res = (int)vs;

            //正在访问和逾期未还才可用
            if (res == 1 || res == 3) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
