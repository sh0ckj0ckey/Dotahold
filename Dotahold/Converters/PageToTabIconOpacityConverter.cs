using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class PageToTabIconOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value?.ToString()?.ToLower() == parameter?.ToString()?.ToLower() ? 1.0 : 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
