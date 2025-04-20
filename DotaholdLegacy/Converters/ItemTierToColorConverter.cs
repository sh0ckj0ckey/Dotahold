using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class ItemTierToColorConverter : IValueConverter
    {
        private static Dictionary<int, SolidColorBrush> _tierColors = new Dictionary<int, SolidColorBrush>();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string tier = value.ToString();
                    switch (tier)
                    {
                        case "1":
                            if (!_tierColors.ContainsKey(1))
                            {
                                _tierColors.Add(1, new SolidColorBrush(Color.FromArgb(255, 83, 83, 83)));
                            }
                            return _tierColors[1];
                        case "2":
                            if (!_tierColors.ContainsKey(2))
                            {
                                _tierColors.Add(2, new SolidColorBrush(Color.FromArgb(255, 0, 89, 0)));
                            }
                            return _tierColors[2];
                        case "3":
                            if (!_tierColors.ContainsKey(3))
                            {
                                _tierColors.Add(3, new SolidColorBrush(Color.FromArgb(255, 35, 35, 178)));
                            }
                            return _tierColors[3];
                        case "4":
                            if (!_tierColors.ContainsKey(4))
                            {
                                _tierColors.Add(4, new SolidColorBrush(Color.FromArgb(255, 102, 0, 102)));
                            }
                            return _tierColors[4];
                        case "5":
                            if (!_tierColors.ContainsKey(5))
                            {
                                _tierColors.Add(5, new SolidColorBrush(Color.FromArgb(255, 255, 165, 0)));
                            }
                            return _tierColors[5];
                        default:
                            if (!_tierColors.ContainsKey(0))
                            {
                                _tierColors.Add(0, new SolidColorBrush(Color.FromArgb(8, 8, 8, 8)));
                            }
                            return _tierColors[0];
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return new SolidColorBrush(Color.FromArgb(8, 8, 8, 8));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}