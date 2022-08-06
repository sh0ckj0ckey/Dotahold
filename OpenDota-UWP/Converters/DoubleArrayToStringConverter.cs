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
    internal class DoubleArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && value is double[] arr)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string split = " ";
                    if (parameter != null)
                        split = " " + parameter.ToString() + " ";

                    for (int i = 0; i < arr.Length; i++)
                    {
                        stringBuilder.Append(arr[i]);

                        if (i < arr.Length - 1)
                        {
                            stringBuilder.Append(split);
                        }
                    }

                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
