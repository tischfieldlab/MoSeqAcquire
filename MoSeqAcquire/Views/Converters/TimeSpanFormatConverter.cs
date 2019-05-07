using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null && value is TimeSpan)
            {
                TimeSpan duration = (TimeSpan)value;
                string result = "";
                if(duration.Days > 0)
                {
                    result += string.Format("{0:0}:", duration.Days);
                }
                if(duration.Hours > 0 || duration.Days > 0)
                {
                    result += string.Format("{0:0}:", duration.Hours);
                }
                result += duration.ToString("mm\\:ss");
                return result;
            }
            return "0:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
