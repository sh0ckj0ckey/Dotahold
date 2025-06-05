using System;
using System.Collections.Generic;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Converters
{
    internal partial class PlayerRankToMedalImageConverter : IValueConverter
    {
        private static readonly HashSet<string> _rankMedals = [
            "10", "11", "12", "13", "14", "15", "16", "17",
            "20", "21", "22", "23", "24", "25", "26", "27",
            "30", "31", "32", "33", "34", "35", "36", "37",
            "40", "41", "42", "43", "44", "45", "46", "47",
            "50", "51", "52", "53", "54", "55", "56", "57",
            "60", "61", "62", "63", "64", "65", "66", "67",
            "70", "71", "72", "73", "74", "75", "76", "77",
            "80", "81", "82", "83", "84", "00"];

        private static BitmapImage? _defaultRankMedal = null;

        /// <summary>
        /// Image Decode Width to Rank Tier to Medal Image
        /// </summary>
        private static readonly Dictionary<int, Dictionary<string, BitmapImage>> _rankMedalImages = [];

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var rank = value?.ToString();
                if (!string.IsNullOrWhiteSpace(rank) && rank.Length >= 2)
                {
                    var tier = rank[0];
                    var stars = rank[1];
                    string medalKey = $"{tier}{stars}";

                    if (!_rankMedals.Contains(medalKey))
                    {
                        tier = '0';
                        stars = '0';
                        medalKey = "00";
                    }

                    int decodePixelWidth = 128;
                    if (parameter is not null && int.TryParse(parameter.ToString(), out int decodeWidth))
                    {
                        decodePixelWidth = decodeWidth;
                    }

                    if (!_rankMedalImages.TryGetValue(decodePixelWidth, out var tierImages))
                    {
                        tierImages = [];
                        _rankMedalImages[decodePixelWidth] = tierImages;
                    }

                    if (!tierImages.TryGetValue(medalKey, out var medalImage))
                    {
                        medalImage = new(new Uri($"ms-appx:///Assets/RankMedals/SeasonalRank{tier}-{stars}.png"))
                        {
                            DecodePixelType = DecodePixelType.Logical,
                            DecodePixelWidth = decodePixelWidth
                        };

                        tierImages[medalKey] = medalImage;
                    }

                    return medalImage;
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            _defaultRankMedal ??= new(new Uri("ms-appx:///Assets/RankMedals/SeasonalRank0-0.png"));
            return _defaultRankMedal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
