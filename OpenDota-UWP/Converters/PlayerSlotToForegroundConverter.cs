using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenDota_UWP.Converters
{
    internal class PlayerSlotToForegroundConverter : IValueConverter
    {
        private SolidColorBrush Slot0Color = new SolidColorBrush(new Color() { R = 51, G = 117, B = 255 });
        private SolidColorBrush Slot1Color = new SolidColorBrush(new Color() { R = 102, G = 255, B = 191 });
        private SolidColorBrush Slot2Color = new SolidColorBrush(new Color() { R = 191, G = 0, B = 191 });
        private SolidColorBrush Slot3Color = new SolidColorBrush(new Color() { R = 243, G = 240, B = 11 });
        private SolidColorBrush Slot4Color = new SolidColorBrush(new Color() { R = 255, G = 107, B = 0 });

        private SolidColorBrush Slot128Color = new SolidColorBrush(new Color() { R = 254, G = 134, B = 194 });
        private SolidColorBrush Slot129Color = new SolidColorBrush(new Color() { R = 161, G = 180, B = 71 });
        private SolidColorBrush Slot130Color = new SolidColorBrush(new Color() { R = 101, G = 217, B = 247 });
        private SolidColorBrush Slot131Color = new SolidColorBrush(new Color() { R = 0, G = 131, B = 33 });
        private SolidColorBrush Slot132Color = new SolidColorBrush(new Color() { R = 164, G = 105, B = 0 });

        private SolidColorBrush SlotXColor = new SolidColorBrush(new Color() { R = 99, G = 99, B = 99 });

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string slot = value.ToString();
                switch (slot)
                {
                    case "0":
                        return Slot0Color;
                    case "1":
                        return Slot1Color;
                    case "2":
                        return Slot2Color;
                    case "3":
                        return Slot3Color;
                    case "4":
                        return Slot4Color;
                    case "128":
                        return Slot128Color;
                    case "129":
                        return Slot129Color;
                    case "130":
                        return Slot130Color;
                    case "131":
                        return Slot131Color;
                    case "132":
                        return Slot132Color;
                    default:
                        return SlotXColor;
                }
            }
            catch { }
            return SlotXColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
