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
    internal class LongToMinutesConverter : IValueConverter
    {
        private static string _DefaultString = "00:00";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return _DefaultString;
                string time = value.ToString();
                if (string.IsNullOrEmpty(time) || time == "0") return _DefaultString;

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
            return _DefaultString;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
