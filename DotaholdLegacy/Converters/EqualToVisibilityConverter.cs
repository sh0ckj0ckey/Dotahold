using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class EqualToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && parameter != null)
                {
                    return value.ToString().ToLower() == parameter.ToString().ToLower() ? Visibility.Visible : Visibility.Collapsed;
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
