using System;
using System.Globalization;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenDota_UWP.Converters
{
    internal class LongToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    long timeStamp = 0;
                    if (long.TryParse(value.ToString(), out timeStamp))
                    {
                        DateTimeOffset dateTimeOffset = timeStamp.ToString().Length == 13 ? DateTimeOffset.FromUnixTimeMilliseconds(timeStamp) : DateTimeOffset.FromUnixTimeSeconds(timeStamp);
                        return dateTimeOffset.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            catch { }
            return "1970-01-01";
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}