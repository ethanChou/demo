using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VisitorManager.ViewModel
{
    public class IDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            var nodes = ThriftManager.All;

            if (parameter == null)
            {
                return "";
            }
            int type = int.Parse(parameter.ToString());
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == value.ToString() && nodes[i].Type == type)
                {
                    return nodes[i].Name;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
