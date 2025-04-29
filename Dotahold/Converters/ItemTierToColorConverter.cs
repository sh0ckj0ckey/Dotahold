using System;
using System.Collections.Generic;
using Dotahold.Data.DataShop;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal partial class ItemTierToColorConverter : IValueConverter
    {
        private static readonly Dictionary<int, SolidColorBrush> _tierColors = [];

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                int tier = int.TryParse(value?.ToString(), out int parsedTier) ? parsedTier : 0;

                switch (tier)
                {
                    case 1:
                        if (!_tierColors.TryGetValue(1, out _))
                        {
                            _tierColors[1] = new SolidColorBrush(Color.FromArgb(255, 191, 191, 191));
                        }
                        return _tierColors[1];
                    case 2:
                        if (!_tierColors.TryGetValue(2, out _))
                        {
                            _tierColors[2] = new SolidColorBrush(Color.FromArgb(255, 147, 228, 127));
                        }
                        return _tierColors[2];
                    case 3:
                        if (!_tierColors.TryGetValue(3, out _))
                        {
                            _tierColors[3] = new SolidColorBrush(Color.FromArgb(255, 128, 148, 252));
                        }
                        return _tierColors[3];
                    case 4:
                        if (!_tierColors.TryGetValue(4, out _))
                        {
                            _tierColors[4] = new SolidColorBrush(Color.FromArgb(255, 213, 124, 255));
                        }
                        return _tierColors[4];
                    case 5:
                        if (!_tierColors.TryGetValue(5, out _))
                        {
                            _tierColors[5] = new SolidColorBrush(Color.FromArgb(255, 255, 225, 150));
                        }
                        return _tierColors[5];
                    default:
                        if (!_tierColors.TryGetValue(0, out _))
                        {
                            _tierColors[0] = new SolidColorBrush(Colors.Transparent);
                        }
                        return _tierColors[0];
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
