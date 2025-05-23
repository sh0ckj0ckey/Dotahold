﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return bool.Parse(value?.ToString() ?? "False") ? Visibility.Visible : Visibility.Collapsed;
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return !bool.Parse(value?.ToString() ?? "True") ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}