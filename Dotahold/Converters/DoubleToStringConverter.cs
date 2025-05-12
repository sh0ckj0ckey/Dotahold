using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is null) return "NaN";

                if (value?.ToString()?.Contains('.') == true)
                {
                    if (double.TryParse(value?.ToString(), out double val))
                    {
                        // return (Math.Floor(100 * v) / 100).ToString();
                        return val.ToString("f1");
                    }
                }
                else
                {
                    return value?.ToString() ?? "NaN";
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return value ?? "NaN";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
