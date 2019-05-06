using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    public class AdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param;
            try{
                param = Double.Parse((string)parameter);
            }catch{
                param = 0;
            }
            return (double)value + param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param;
            try
            {
                param = Double.Parse((string)parameter);
            }
            catch
            {
                param = 0;
            }
            return (double)value - param;
        }
    }
}
