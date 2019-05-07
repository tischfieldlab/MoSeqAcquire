using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;


namespace MoSeqAcquire.Views.Converters
{
    public class OrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == -1) { return "Pre"; }
            if ((int)value == int.MaxValue) { return "Post"; }
            return (String)value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Equals("Pre")) { return -1; }
            if (((string)value).Equals("Post")) { return int.MaxValue; }
            return int.Parse((string)value);
        }
    }
}
