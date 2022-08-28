using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenDota_UWP.Models
{
    public class DotaHeroRankingModel
    {
        public string hero_id { get; set; }
        public List<RankingPlayer> rankings { get; set; }
    }

    public class RankingPlayer : ViewModels.ViewModelBase
    {
        public string account_id { get; set; }
        public string score { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public string avatar { get; set; } = "ms-appx:///Assets/Icons/avatar_placeholder.jpeg";
        public string last_login { get; set; }
        public string rank_tier { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int iRank { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
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
                ImageSource = await ImageLoader.LoadImageAsync(avatar, "ms-appx:///Assets/Icons/avatar_placeholder.jpeg");
                ImageSource.DecodePixelType = DecodePixelType.Logical;
                ImageSource.DecodePixelWidth = decodeWidth;
            }
            catch { }
        }
    }

}
