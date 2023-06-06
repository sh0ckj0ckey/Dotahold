using Dotahold.Core.DataShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.Models
{
    public class DotaHeroModel : ViewModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string localized_name { get; set; }
        public string primary_attr { get; set; }
        public string attack_type { get; set; }
        public string[] roles { get; set; }
        public string img { get; set; } = "ms-appx:///Assets/Icons/item_placeholder.png";
        public string icon { get; set; } = "ms-appx:///Assets/Icons/item_placeholder.png";
        public double base_health { get; set; }
        public double base_health_regen { get; set; }
        public double base_mana { get; set; }
        public double base_mana_regen { get; set; }
        public double base_armor { get; set; }
        public double base_mr { get; set; }
        public double base_attack_min { get; set; }
        public double base_attack_max { get; set; }
        public double base_str { get; set; }
        public double base_agi { get; set; }
        public double base_int { get; set; }
        public double str_gain { get; set; }
        public double agi_gain { get; set; }
        public double int_gain { get; set; }
        public double attack_range { get; set; }
        public double projectile_speed { get; set; }
        public double attack_rate { get; set; }
        public double move_speed { get; set; }
        public double turn_rate { get; set; }
        public bool? cm_enabled { get; set; }
        public double legs { get; set; }

        // 英雄图片
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage _ImageSource = ConstantsCourier.DefaultHeroImageSource72;
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                var imageSource = await ImageCourier.GetImageAsync(img);
                if (imageSource != null)
                {
                    ImageSource = imageSource;
                    ImageSource.DecodePixelType = DecodePixelType.Logical;
                    ImageSource.DecodePixelWidth = decodeWidth;
                }
            }
            catch { }
        }

        // 英雄小头像
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage _IconSource = null;
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage IconSource
        {
            get { return _IconSource; }
            set { Set("IconSource", ref _IconSource, value); }
        }
        public async Task LoadIconAsync(int decodeWidth)
        {
            try
            {
                IconSource = await ImageCourier.GetImageAsync(icon);
                IconSource.DecodePixelType = DecodePixelType.Logical;
                IconSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }
    }

}
