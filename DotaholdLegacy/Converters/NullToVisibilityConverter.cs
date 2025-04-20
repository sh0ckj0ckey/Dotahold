using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null)
                {
                    return value == null ? Visibility.Collapsed : Visibility.Visible;
                }

                if (parameter != null && parameter.ToString() == "-")
                {
                    return value == null ? Visibility.Visible : Visibility.Collapsed;
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