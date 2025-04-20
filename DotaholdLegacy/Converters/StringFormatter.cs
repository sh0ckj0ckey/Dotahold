using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class StringFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Retrieve the format string and use it to format the value.
                string formatString = parameter as string;
                if (!string.IsNullOrEmpty(formatString))
                {
                    return string.Format(formatString, value);
                }

                return value.ToString();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return "StringFormatter Error";
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
