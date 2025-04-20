using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Data.Models
{
    public class DotaHeroModel : ObservableObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string localized_name { get; set; }
        public string primary_attr { get; set; }
        public string attack_type { get; set; }
        public string[] roles { get; set; }
        public string img { get; set; }
        public string icon { get; set; }
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


        [JsonIgnore] private bool _loadedImage = false;

        [JsonIgnore] private BitmapImage _imageSource = ConstantsCourier.DefaultHeroImageSource72;

        [JsonIgnore] private BitmapImage _iconSource = null;

        /// <summary>
        /// 英雄图片
        /// </summary>
        [JsonIgnore]
        public BitmapImage ImageSource
        {
            get => _imageSource;
            private set => SetProperty(ref _imageSource, value);
        }

        /// <summary>
        /// 英雄小头像
        /// </summary>
        [JsonIgnore]
        public BitmapImage IconSource
        {
            get => _iconSource;
            private set => SetProperty(ref _iconSource, value);
        }

        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (_loadedImage || string.IsNullOrWhiteSpace(this.img))
                {
                    return;
                }

                var imageSource = await ImageCourier.GetImageAsync(this.img, decodeWidth, 0);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                    _loadedImage = true;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        public async Task LoadIconAsync(int decodeWidth)
        {
            try
            {
                if (this.IconSource != null || string.IsNullOrWhiteSpace(this.icon))
                {
                    return;
                }

                var iconSource = await ImageCourier.GetImageAsync(this.icon, decodeWidth, 0);
                if (iconSource != null)
                {
                    this.IconSource = iconSource;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
