using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenDota_UWP.Models
{
    public class DotaMatchHeroPlayedModel : ViewModels.ViewModelBase
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
        public string sHeroCoverImage { get; set; } = "ms-appx:///Assets/Icons/item_placeholder.png";

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sWinRate { get; set; } = string.Empty;

        // 封面图片(英雄小头像)
        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (ImageSource != null) return;
                ImageSource = await ImageLoader.LoadImageAsync(sHeroCoverImage);
                ImageSource.DecodePixelType = DecodePixelType.Logical;
                ImageSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }
    }
}
