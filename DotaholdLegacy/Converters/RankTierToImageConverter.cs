using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Converters
{
    internal class RankTierToImageConverter : IValueConverter
    {
        private static List<string> RankTiers = new List<string> {
            "10", "11", "12", "13", "14", "15", "16", "17",
            "20", "21", "22", "23", "24", "25", "26", "27",
            "30", "31", "32", "33", "34", "35", "36", "37",
            "40", "41", "42", "43", "44", "45", "46", "47",
            "50", "51", "52", "53", "54", "55", "56", "57",
            "60", "61", "62", "63", "64", "65", "66", "67",
            "70", "71", "72", "73", "74", "75", "76", "77",
            "80", "81", "82", "83", "84", "00"};

        private static BitmapImage DefaultRankTier = new BitmapImage(new System.Uri("ms-appx:///Assets/RankMedal/SeasonalRank0-0.png"));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string rank = value.ToString();
                    if (!string.IsNullOrEmpty(rank) && rank.Length >= 2)
                    {
                        string tier = rank[0].ToString();
                        string stars = rank[1].ToString();
                        string contain = tier + stars;
                        if (RankTiers.Contains(contain))
                        {
                            string image = string.Format("ms-appx:///Assets/RankMedal/SeasonalRank{0}-{1}.png", tier, stars);
                            var img = new BitmapImage(new System.Uri(image));

                            if (parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
                            {
                                int width = 0;
                                if (int.TryParse(parameter.ToString(), out width))
                                {
                                    img.DecodePixelType = DecodePixelType.Logical;
                                    img.DecodePixelWidth = width;
                                }
                            }

                            return img;
                        }
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return DefaultRankTier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
