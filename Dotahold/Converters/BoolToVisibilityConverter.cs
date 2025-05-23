﻿using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (bool.TryParse(value?.ToString(), out bool val))
                {
                    if (parameter is not null && parameter.ToString() == "!")
                    {
                        return !val ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        return val ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
