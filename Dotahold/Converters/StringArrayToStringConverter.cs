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
                    string separator = "\r\n";

                    if (parameter is string param && !string.IsNullOrWhiteSpace(param))
                    {
                        separator = $" {param} ";
                    }

                    return string.Join(separator, stringArray);
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
