﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace MoSeqAcquire.Views.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null){
               return Visibility.Collapsed; 
            }
            if(!(bool)value) { 
                return Visibility.Collapsed; 
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null){
                return false; 
            }
            if((Visibility)value == Visibility.Collapsed) { 
                return false; 
            }
            return true;
        }
    }
}
