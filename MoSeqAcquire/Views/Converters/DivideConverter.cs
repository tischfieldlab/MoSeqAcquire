using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param;
            try{
                param = Double.Parse((string)parameter);
            }catch{
                param = 1;
            }
            double val = double.Parse(value.ToString());
            return (double)(val / param);
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
            double val = double.Parse(value.ToString());
            return (double)(val * param);
        }
    }
}
