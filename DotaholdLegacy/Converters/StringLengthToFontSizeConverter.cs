using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class StringLengthToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string v = value.ToString();
                    int len = v.Length;
                    if (len >= 14)
                    {
                        return 12.0;
                    }
                    else
                    {
                        return 14.0;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return 14.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}