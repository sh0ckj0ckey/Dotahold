using System;
using System.Globalization;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class LongToMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return "00:00";
                string time = value.ToString();
                if (string.IsNullOrEmpty(time) || time == "0") return "00:00";

                long totalSeconds = System.Convert.ToInt64(time);
                long minutes = totalSeconds / 60;
                long seconds = totalSeconds % 60;

                string min = minutes.ToString();
                string sec = seconds.ToString();
                StringBuilder stringBuilder = new StringBuilder();
                if (min.Length <= 1)
                {
                    stringBuilder.Append("0");
                }
                stringBuilder.Append(min);
                stringBuilder.Append(":");
                if (sec.Length <= 1)
                {
                    stringBuilder.Append("0");
                }
                stringBuilder.Append(sec);
                return stringBuilder.ToString();
            }
            catch { }
            return "00:00";
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
