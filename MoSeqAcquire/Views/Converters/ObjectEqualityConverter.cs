using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Reflection;

namespace MoSeqAcquire.Views.Converters
{
    public class ObjectEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameter == null)
                return DependencyProperty.UnsetValue;

            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
