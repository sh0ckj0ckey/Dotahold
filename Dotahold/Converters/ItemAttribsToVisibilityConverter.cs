using System;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class ItemAttribsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is Attrib[] attribs && attribs?.Length > 0)
                {
                    foreach (Attrib attrib in attribs)
                    {
                        if (!string.IsNullOrWhiteSpace(attrib.display))
                        {
                            return Visibility.Visible;
                        }
                    }
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
