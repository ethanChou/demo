using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisitorManager.ViewModel
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ThriftCommon.Status vs = (ThriftCommon.Status)Enum.Parse(typeof(ThriftCommon.Status), value.ToString());
            if (vs == ThriftCommon.Status.Visiting)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x66, 0x40, 0x00));
            }
            if (vs == ThriftCommon.Status.Leave)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0x33, 0x33));
            }
            if (vs == ThriftCommon.Status.LostCard)
            {
                //return new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x53, 0x44));
                return new SolidColorBrush(Color.FromArgb(0xff, 0x42, 0x82, 0xc4));
            }
            if (vs == ThriftCommon.Status.NoComeBack)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x00, 0x00));
            }
            return new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
