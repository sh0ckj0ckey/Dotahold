using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class StringToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return value?.ToString()?.ToUpper() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
