using System;
using System.Collections.Generic;
using Dotahold.Data.DataShop;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Dotahold.Helpers
{
    public static class AbilityColorHelper
    {
        private static readonly Dictionary<string, LinearGradientBrush> _facetGradientBrushes = [];

        private static readonly Dictionary<int, SolidColorBrush> _damageTypeForegroundBrushes = [];

        /// <summary>
        /// 获取命石颜色名称
        /// </summary>
        /// <param name="facetColor"></param>
        /// <returns></returns>
        public static string GetFacetColorName(int facetColor)
        {
            return facetColor switch
            {
                0 => "Red",
                1 => "Yellow",
                2 => "Green",
                3 => "Blue",
                4 => "Purple",
                5 => "Gray",
                _ => ""
            };
        }

        /// <summary>
        /// 根据命石卡片的背景色名称获取渐变画刷
        /// </summary>
        /// <param name="gradientName"></param>
        /// <returns></returns>
        public static LinearGradientBrush GetFacetGradientBrush(string gradientName)
        {
            try
            {
                if (_facetGradientBrushes.TryGetValue(gradientName, out var brush))
                {
                    return brush;
                }

                LinearGradientBrush newBrush = gradientName switch
                {
                    "FacetColorRed0" => CreateBrush("#9F3C3C", "#4A2040"),
                    "FacetColorRed1" => CreateBrush("#954533", "#452732"),
                    "FacetColorRed2" => CreateBrush("#A3735E", "#4F2A25"),
                    "FacetColorYellow0" => CreateBrush("#C8A45C", "#6F3D21"),
                    "FacetColorYellow1" => CreateBrush("#C6A158", "#604928"),
                    "FacetColorYellow2" => CreateBrush("#CAC194", "#433828"),
                    "FacetColorYellow3" => CreateBrush("#C3A99A", "#4D352B"),
                    "FacetColorGreen0" => CreateBrush("#A2B23E", "#2D5A18"),
                    "FacetColorGreen1" => CreateBrush("#7EC2B2", "#29493A"),
                    "FacetColorGreen2" => CreateBrush("#538564", "#1C3D3F"),
                    "FacetColorGreen3" => CreateBrush("#9A9F6A", "#223824"),
                    "FacetColorGreen4" => CreateBrush("#9FAD8E", "#3F4129"),
                    "FacetColorBlue0" => CreateBrush("#727CB2", "#342D5B"),
                    "FacetColorBlue1" => CreateBrush("#547EA6", "#2A385E"),
                    "FacetColorBlue2" => CreateBrush("#6BAEBC", "#135459"),
                    "FacetColorBlue3" => CreateBrush("#94B5BA", "#385B59"),
                    "FacetColorPurple0" => CreateBrush("#B57789", "#412755"),
                    "FacetColorPurple1" => CreateBrush("#9C70A4", "#282752"),
                    "FacetColorPurple2" => CreateBrush("#675CAE", "#261C44"),
                    "FacetColorGray0" => CreateBrush("#565C61", "#1B1B21"),
                    "FacetColorGray1" => CreateBrush("#6A6D73", "#29272C"),
                    "FacetColorGray2" => CreateBrush("#95A9B1", "#3E464F"),
                    "FacetColorGray3" => CreateBrush("#ADB6BE", "#4E5557"),
                    _ => CreateBrush("#00000000", "#00000000"),
                };

                _facetGradientBrushes[gradientName] = newBrush;
                return newBrush;
            }
            catch (Exception ex)
            {
                LogCourier.Log($"GetGradientBrush({gradientName}) Error: {ex.Message}", LogCourier.LogType.Error);
                return CreateBrush("#00000000", "#00000000");
            }
        }

        /// <summary>
        /// 获取技能的伤害类型字体颜色
        /// </summary>
        /// <param name="damageType"></param>
        /// <returns></returns>
        public static SolidColorBrush GetDamageTypeColor(int damageType)
        {
            if (_damageTypeForegroundBrushes.TryGetValue(damageType, out var color))
            {
                return color;
            }

            SolidColorBrush newColor = damageType switch
            {
                1 => new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                2 => new SolidColorBrush(Color.FromArgb(255, 163, 220, 238)),
                4 => new SolidColorBrush(Color.FromArgb(255, 255, 165, 0)),
                8 => new SolidColorBrush(Color.FromArgb(255, 165, 15, 121)),
                _ => new SolidColorBrush(Color.FromArgb(255, 204, 204, 204)),
            };

            _damageTypeForegroundBrushes[damageType] = newColor;

            return newColor;
        }

        /// <summary>
        /// 创建线性渐变画刷
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        private static LinearGradientBrush CreateBrush(string color1, string color2)
        {
            return new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops =
                [
                    new GradientStop { Color = ConvertHexColor(color1), Offset = 0 },
                    new GradientStop { Color = ConvertHexColor(color2), Offset = 1 }
                ]
            };
        }

        /// <summary>
        /// 将 #RRGGBB 或 #AARRGGBB 格式的字符串转换为 Windows.UI.Color
        /// </summary>
        private static Color ConvertHexColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("hex is null or empty.");
            }

            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                byte r = Convert.ToByte(hex[..2], 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = Convert.ToByte(hex[..2], 16);
                byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                byte b = Convert.ToByte(hex.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                throw new FormatException("Invalid color format. Use #RRGGBB or #AARRGGBB.");
            }
        }
    }
}
