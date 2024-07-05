using Dotahold.Core.DataShop;
using Dotahold.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
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

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroCoverImage { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sWinRate { get; set; } = string.Empty;

        // 封面图片(英雄小头像)
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        [Newtonsoft.Json.JsonIgnore]
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
            catch { }
        }
    }
}
