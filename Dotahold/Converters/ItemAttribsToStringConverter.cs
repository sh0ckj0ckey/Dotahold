using System;
using System.Text;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal partial class ItemAttribsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is Attrib[] attribs && attribs?.Length > 0)
                {
                    var attribsStringBuider = new StringBuilder();
                    foreach (Attrib attrib in attribs)
                    {
                        if (!string.IsNullOrWhiteSpace(attrib.display))
                        {
                            if (attribsStringBuider.Length > 0)
                            {
                                attribsStringBuider.Append("\r\n");
                            }
                            attribsStringBuider.Append(attrib.display.Replace("{value}", attrib.value));
                        }
                    }

                    return attribsStringBuider.ToString();
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
