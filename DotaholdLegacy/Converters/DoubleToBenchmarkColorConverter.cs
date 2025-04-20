using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class DoubleToBenchmarkColorConverter : IValueConverter
    {
        private SolidColorBrush Ping0Color = new SolidColorBrush(Colors.ForestGreen);
        private SolidColorBrush Ping1Color = new SolidColorBrush(Colors.DodgerBlue);
        private SolidColorBrush Ping2Color = new SolidColorBrush(Colors.Peru);
        private SolidColorBrush Ping3Color = new SolidColorBrush(Colors.Tomato);
        private SolidColorBrush Ping4Color = new SolidColorBrush(Colors.Crimson);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    double pct = 0; // 去掉%的值，例如80%则pct=80
                    if (double.TryParse(value.ToString(), out pct))
                    {
                        if (pct >= 80)
                        {
                            return Ping0Color;
                        }
                        else if (pct >= 60 && pct < 80)
                        {
                            return Ping1Color;
                        }
                        else if (pct >= 40 && pct < 60)
                        {
                            return Ping2Color;
                        }
                        else if (pct >= 20 && pct < 40)
                        {
                            return Ping3Color;
                        }
                        else
                        {
                            return Ping4Color;
                        }
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return Ping0Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}