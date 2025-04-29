using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is string[] stringArray)
                {
                    if (parameter is not null && parameter.ToString() == "!")
                    {
                        return stringArray.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else
                    {
                        return stringArray.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
                    }
                }

                if (parameter is not null && parameter.ToString() == "!")
                {
                    return string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
