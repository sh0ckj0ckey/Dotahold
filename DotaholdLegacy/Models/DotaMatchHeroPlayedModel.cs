﻿using System;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaMatchHeroPlayedModel : ViewModelBase
    {
        public string hero_id { get; set; }
        public double? last_played { get; set; }
        public double? games { get; set; }
        public double? win { get; set; }
        public double? with_games { get; set; }
        public double? with_win { get; set; }
        public double? against_games { get; set; }
        public double? against_win { get; set; }

        [JsonIgnore]
        public string sHeroCoverImage { get; set; }

        [JsonIgnore]
        public string sHeroName { get; set; } = string.Empty;

        [JsonIgnore]
        public string sWinRate { get; set; } = string.Empty;

        // 封面图片(英雄小头像)
        [JsonIgnore]
        private BitmapImage _ImageSource = null;
        [JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            private set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (this.ImageSource != null || string.IsNullOrWhiteSpace(this.sHeroCoverImage)) return;

                var imageSource = await ImageCourier.GetImageAsync(this.sHeroCoverImage, decodeWidth, 0);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
