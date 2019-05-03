using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Reflection;

namespace MoSeqAcquire.Views.Converters
{
    public class EnumToImageConverter<T> : IValueConverter
    {
        protected Dictionary<T, String> iconMap;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.iconMap[((T)value)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
