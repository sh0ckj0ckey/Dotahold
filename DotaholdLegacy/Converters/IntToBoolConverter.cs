using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return int.Parse(value.ToString()) > 0;
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return int.Parse(value?.ToString() ?? "0") <= 0;
                }

                if (parameter != null && value != null)
                {
                    return parameter.ToString() == value.ToString();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}