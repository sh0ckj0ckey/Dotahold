using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace Dotahold.Converters
{
    internal partial class EqualToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return value?.ToString()?.ToLower() == parameter?.ToString()?.ToLower() ? Visibility.Visible : Visibility.Collapsed;
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
