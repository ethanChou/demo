using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VisitorManager.Model;

namespace VisitorManager.ViewModel.Common
{
    public class VisitorStatusToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;

            VisitorStatus vs = (VisitorStatus)Enum.Parse(typeof(VisitorStatus), value.ToString());
            int res = (int)vs;

            if (res == 1 || res == 3) return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
