﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VisitorManager.ViewModel
{
    public class ImageListBoxItemHeightConvert : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (parameter == null)
            {
                parameter = 4;
            }
            if (value.ToString() == "0")
            {
                return 0;
            }
            int p = 4;
            int.TryParse(parameter.ToString(), out p);
            var v = (((double)(value) - p * 2 * 13) / p);
            return v;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
