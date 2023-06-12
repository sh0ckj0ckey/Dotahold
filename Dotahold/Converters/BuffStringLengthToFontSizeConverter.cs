using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace Dotahold.Converters
{
    internal class BuffStringLengthToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string v = value.ToString();
                    int len = v.Length;
                    if (len <= 4)
                    {
                        return 17.0;
                    }
                    else
                    {
                        return 11.0;
                    }
                }
            }
            catch { }
            return 17.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}