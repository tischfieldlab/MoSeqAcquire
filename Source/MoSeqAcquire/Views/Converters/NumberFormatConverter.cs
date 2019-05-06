using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (float)value;
            if (val >= 10000)
            {
                return val.ToString("E3");
            }
            else
            {
                return val.ToString("F3");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string itemContent = (string)value;
            float literal = 0;
            try
            {
                literal = float.Parse(itemContent);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Your number is too big!");
            }

            return literal;
        }
    }
}
