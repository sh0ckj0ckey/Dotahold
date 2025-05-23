﻿using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class LongToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    long timeStamp = 0;
                    if (long.TryParse(value.ToString(), out timeStamp))
                    {
                        DateTimeOffset dateTimeOffset = timeStamp.ToString().Length == 13 ? DateTimeOffset.FromUnixTimeMilliseconds(timeStamp) : DateTimeOffset.FromUnixTimeSeconds(timeStamp);
                        return dateTimeOffset.LocalDateTime.ToString("yyyy-MM-dd HH:mm");
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return "1970-01-01";
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}