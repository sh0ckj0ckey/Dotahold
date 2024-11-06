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
    public class DotaHeroRankingModel
    {
        public string hero_id { get; set; }
        public List<RankingPlayer> rankings { get; set; }
    }

    public class RankingPlayer : ViewModelBase
    {
        public string account_id { get; set; }
        public string score { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string last_login { get; set; }
        public string rank_tier { get; set; }

        [JsonIgnore]
        public int iRank { get; set; }

        [JsonIgnore]
        private BitmapImage _ImageSource = ConstantsCourier.DefaultAvatarImageSource72;
        [JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            private set { Set("ImageSource", ref _ImageSource, value); }
        }
        [JsonIgnore]
        private bool _loadedImage = false;
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (_loadedImage || string.IsNullOrWhiteSpace(this.avatar)) return;
                var imageSource = await ImageCourier.GetImageAsync(this.avatar, decodeWidth, 0, false);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                    _loadedImage = true;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

}
