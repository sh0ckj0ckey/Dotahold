using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is string[] stringArray)
                {
                    string separator = parameter as string ?? "\r\n";
                    return string.Join(separator, stringArray);
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
