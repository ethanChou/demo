using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ThriftCommon;

namespace VisitorManager.ViewModel
{
    public class CardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "未知";


            ThriftCommon.IdentifyType res = (ThriftCommon.IdentifyType)Enum.Parse(typeof(ThriftCommon.IdentifyType), value.ToString());
         
            if ((int)res == 0) return "二代身份证";
            if ((int)res == 1) return "正式卡";
            if ((int)res == 2) return "其他证件";

            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PassCardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "临时卡";
            int res = 0;
            int.TryParse(value.ToString(), out res);
            if (res == 0) return "临时卡";
            if (res == 1) return "正式卡";

            return "临时卡";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TelephoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";

            return string.Format(" {0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
