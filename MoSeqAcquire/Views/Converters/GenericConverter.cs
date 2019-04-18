using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;


namespace MoSeqAcquire.Views.Converters
{
    public class GenericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.Convert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.Convert(value, targetType);
        }

        protected object Convert(object value, Type targetType)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                if (underlyingType.IsValueType && value == null)
                {
                    value = Activator.CreateInstance(underlyingType);
                }
                return System.Convert.ChangeType(value, underlyingType);
            }
            return System.Convert.ChangeType(value, targetType);
        }
    }
}
