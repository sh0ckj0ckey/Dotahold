using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Dotahold.Data.Models;

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
                    StringBuilder attribsStringBuider = new StringBuilder();
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
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
