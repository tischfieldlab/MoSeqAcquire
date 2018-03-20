using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Reflection;
using System.Globalization;

namespace MoSeqAcquire.Views.Converters
{
    public class EnumContainsConverter : IValueConverter
    {
        private int targetValue;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mask = (int)parameter;
            this.targetValue = (int)value;
            return ((mask & this.targetValue) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.targetValue ^= (int)parameter;
            return Enum.Parse(targetType, this.targetValue.ToString());
        } 
    }
}
