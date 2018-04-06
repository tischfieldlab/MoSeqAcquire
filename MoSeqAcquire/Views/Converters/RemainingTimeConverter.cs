﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoSeqAcquire.Views.Converters
{
    public class RemainingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String result = String.Empty;
            if (value is TimeSpan)
            {
                TimeSpan timeSpan = (TimeSpan)value;
                if (timeSpan.Equals(TimeSpan.MaxValue))
                {
                    result = "calculating.....";
                }
                else if (timeSpan != TimeSpan.Zero)
                {
                    if (parameter == null || !(parameter is string))
                        result = String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                                    timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)Math.Round(timeSpan.Milliseconds / 100.0));
                    else
                    {
                        /*DateTime time = DateTime.Today;
                        time = time.Add(timeSpan);
                        result = time.ToString((string)parameter);*/
                        result = timeSpan.ToString((string)parameter);
                    }
                }
            }
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
