using OpenDota_UWP.Helpers;
using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenDota_UWP.Converters
{
    internal class StringToRuneConverter : IValueConverter
    {
        private BitmapImage DoubleRune = null;
        private BitmapImage HasteRune = null;
        private BitmapImage IllusionRune = null;
        private BitmapImage InvisibilityRune = null;
        private BitmapImage RegenerationRune = null;
        private BitmapImage BountyRune = null;
        private BitmapImage ArcaneRune = null;
        private BitmapImage WaterRune = null;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string rune = value.ToString();
                    switch (rune)
                    {
                        case "0":   // 双倍
                            if (DoubleRune == null) DoubleRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_0.png"));
                            return DoubleRune;

                        case "1":   // 极速
                            if (HasteRune == null) HasteRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_1.png"));
                            return HasteRune;

                        case "2":   // 分身
                            if (IllusionRune == null) IllusionRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_2.png"));
                            return IllusionRune;

                        case "3":   // 隐身
                            if (InvisibilityRune == null) InvisibilityRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_3.png"));
                            return InvisibilityRune;

                        case "4":   // 恢复
                            if (RegenerationRune == null) RegenerationRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_4.png"));
                            return RegenerationRune;

                        case "5":   // 赏金
                            if (BountyRune == null) BountyRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_5.png"));
                            return BountyRune;

                        case "6":   // 奥术
                            if (ArcaneRune == null) ArcaneRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_6.png"));
                            return ArcaneRune;

                        case "7":   // 圣水
                            if (WaterRune == null) WaterRune = new BitmapImage(new Uri("ms-appx:///Assets/Icons/Match/Runes/img_rune_7.png"));
                            return WaterRune;

                        default:
                            //var image = await ImageLoader.LoadImageAsync(string.Format("ms-appx:///Assets/Icons/Match/Runes/img_rune_{0}.png", rune), "");
                            var image = new BitmapImage();
                            image.DecodePixelType = DecodePixelType.Logical;
                            image.DecodePixelWidth = 32;
                            image.UriSource = new Uri(string.Format("ms-appx:///Assets/Icons/Match/Runes/img_rune_{0}.png", rune));
                            return image;
                    }
                }
            }
            catch { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}