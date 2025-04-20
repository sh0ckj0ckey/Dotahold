using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return "NaN";
                string val = value.ToString();
                if (val.Contains("."))
                {
                    if (value is double v)
                    {
                        return v.ToString("f1");
                        // return (Math.Floor(100 * v) / 100).ToString();
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}