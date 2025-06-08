using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class HeroRankingModel
    {
        /// <summary>
        /// 默认玩家头像图片
        /// </summary>
        private static BitmapImage? _defaultAvatarImageSource32 = null;

        public DotaHeroRankingModel DotaHeroRanking { get; private set; }

        public int Rank { get; private set; }

        public AsyncImage AvatarImage { get; private set; }

        public HeroRankingModel(DotaHeroRankingModel ranking, int rank)
        {
            _defaultAvatarImageSource32 ??= new BitmapImage(new Uri("ms-appx:///Assets/Profile/img_default_avatar.jpeg"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 32,
            };

            this.DotaHeroRanking = ranking;
            this.Rank = rank;
            this.AvatarImage = new AsyncImage(this.DotaHeroRanking.avatar, 0, 32, _defaultAvatarImageSource32);
        }
    }
}
