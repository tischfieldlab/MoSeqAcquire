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
            return value;
            //return this.Convert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //value = this.Convert(value, targetType);
            if (string.IsNullOrEmpty(value.ToString()))
                return null;

            return value;
        }

        protected object Convert(object value, Type targetType)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }
            if (targetType.IsValueType && value == null)
            {
                value = Activator.CreateInstance(targetType);
            }
            return System.Convert.ChangeType(value, targetType);
        }
    }
}
