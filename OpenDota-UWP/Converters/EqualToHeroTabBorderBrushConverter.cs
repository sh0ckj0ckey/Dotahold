using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class EqualToHeroTabBorderBrushConverter : IValueConverter
    {
        private static SolidColorBrush _solidColorBrushUnselected = new SolidColorBrush(Color.FromArgb(0x00, 0x80, 0x80, 0x80));
        private static SolidColorBrush _solidColorBrushSelected = new SolidColorBrush(Color.FromArgb(0x66, 0x80, 0x80, 0x80));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && parameter != null)
                {
                    var eq = value.ToString() == parameter.ToString();
                    if (eq)
                    {
                        return _solidColorBrushSelected;
                    }
                }
            }
            catch { }
            return _solidColorBrushUnselected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}