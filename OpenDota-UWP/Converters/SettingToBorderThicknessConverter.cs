using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class SettingToBorderThicknessConverter : IValueConverter
    {
        private static Thickness _offThickness = new Thickness(0, 0, 0, 0);
        private static Thickness _onThickness = new Thickness(1, 0, 0, 0);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    return bool.Parse(value?.ToString() ?? "False") ? _onThickness : _offThickness;
                }
            }
            catch { }
            return _offThickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}