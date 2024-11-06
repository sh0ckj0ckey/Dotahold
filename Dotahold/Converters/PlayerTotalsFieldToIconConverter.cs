using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class PlayerTotalsFieldToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string field = value.ToString().ToLower();
                    switch (field)
                    {
                        case "kills":
                            return "\uE8F0";

                        case "deaths":
                            return "\uF78A";

                        case "assists":
                            return "\uE8E1";

                        case "gpm":
                            return "\uE9D9";

                        case "xpm":
                            return "\uE9D9";

                        case "last hits":
                            return "\uF138";

                        case "denies":
                            return "\uEDB1";

                        case "level":
                            return "\uEA41";

                        case "hero damage":
                            return "\uEA92";

                        case "tower damage":
                            return "\uECAD";

                        case "hero healing":
                            return "\uF10E";

                        case "kda":
                            return "\uE81E";

                        default:
                            return string.Empty;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}