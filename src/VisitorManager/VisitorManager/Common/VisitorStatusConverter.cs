using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VisitorManager.Model;

namespace VisitorManager.ViewModel.Common
{
    public class VisitorStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "未知";
            VisitorStatus vs = (VisitorStatus)Enum.Parse(typeof(VisitorStatus), value.ToString());
            int res = (int)vs;
           
            if (res == 1) return "正在访问";
            if (res == 2) return "已经离开";
            if (res == 3) return "逾期未还";
            if (res == 4) return "暂存待办";
            if (res == 5) return "等待进入";

            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
