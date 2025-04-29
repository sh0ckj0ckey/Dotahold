using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (int.TryParse(value?.ToString(), out int number))
                {
                    if (parameter is not null && parameter.ToString() == "!")
                    {
                        return number > 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else
                    {
                        return number > 0 ? Visibility.Visible : Visibility.Collapsed;
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
