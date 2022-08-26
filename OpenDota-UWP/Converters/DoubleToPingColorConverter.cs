using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenDota_UWP.Converters
{
    internal class DoubleToPingColorConverter : IValueConverter
    {
        private SolidColorBrush Ping0Color = new SolidColorBrush(new Color() { R = 0, G = 99, B = 177, A = 255 });
        private SolidColorBrush Ping1Color = new SolidColorBrush(new Color() { R = 0, G = 128, B = 0, A = 255 });
        private SolidColorBrush Ping2Color = new SolidColorBrush(new Color() { R = 60, G = 179, B = 113, A = 255 });
        private SolidColorBrush Ping3Color = new SolidColorBrush(new Color() { R = 255, G = 127, B = 80, A = 255 });
        private SolidColorBrush Ping4Color = new SolidColorBrush(new Color() { R = 220, G = 20, B = 60, A = 255 });

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
                            return Ping0Color;
                        }
                        else if (ping >= 0 && ping <= 35)
                        {
                            return Ping1Color;
                        }
                        else if (ping > 35 && ping <= 70)
                        {
                            return Ping2Color;
                        }
                        else if (ping > 70 && ping <= 110)
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
            catch { }
            return Ping0Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}