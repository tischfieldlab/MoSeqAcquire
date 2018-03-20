using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace MoSeqAcquire.Views.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = "";
            try
            {
                foreach (var itm in (value as List<String>))
                {
                    str += itm + "\n";
                }
            }
            catch
            {
                return str;
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as String).Split('\n').ToList<String>();
        }
    }
}
