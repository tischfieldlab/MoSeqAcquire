using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Reflection;

namespace MoSeqAcquire.Views.Converters
{
    public class DataTypeInheritsTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var baseType = parameter as Type;
            if (value == null)
            {
                return false;
            }
            var valueType = value.GetType();
            return baseType.IsAssignableFrom(valueType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
