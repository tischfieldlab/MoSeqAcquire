using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param;
            try{
                param = Double.Parse((string)parameter);
            }catch{
                param = 1;
            }
            double realvalue;
            if (value is string)
            {
                realvalue = double.Parse((string)value);
            }
            else
            {
                realvalue = (double)value;
            }
            return (double)realvalue * param;
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
                param = 1;
            }
            double realvalue;
            if (value is string)
            {
                realvalue = double.Parse((string)value);
            }
            else
            {
                realvalue = (double)value;
            }
            return (double)realvalue / param;
        }
    }
}
