using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class EmptyStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter is not null && parameter.ToString() == "!")
                {
                    return string.IsNullOrWhiteSpace(value?.ToString());
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(value?.ToString());
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
