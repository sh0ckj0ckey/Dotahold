using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenDota_UWP.Converters
{
    public class StringLengthToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string v = value.ToString();
                    int len = v.Length;
                    if (len >= 14)
                    {
                        return 12.0;
                    }
                    else
                    {
                        return 14.0;
                    }
                }
            }
            catch { }
            return 14.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}