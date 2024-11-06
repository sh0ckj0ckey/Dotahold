using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class DoubleToPingIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    double ping = -1;
                    if (double.TryParse(value.ToString(), out ping))
                    {
                        if (ping < 0)
                        {
                            return "\uE904";
                        }
                        else if (ping >= 0 && ping <= 35)
                        {
                            return "\uE908";
                        }
                        else if (ping > 35 && ping <= 70)
                        {
                            return "\uE907";
                        }
                        else if (ping > 70 && ping <= 110)
                        {
                            return "\uE906";
                        }
                        else
                        {
                            return "\uE905";
                        }
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