using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter is not null && parameter.ToString() == "!")
                {
                    return value is null;
                }
                else
                {
                    return value is not null;
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
