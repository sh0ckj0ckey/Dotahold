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
        public static BitmapImage? DefaultAvatarImageSource32 = null;

        public DotaHeroRankingModel DotaHeroRanking { get; private set; }

        public AsyncImage AvatarImage { get; private set; }

        public HeroRankingModel(DotaHeroRankingModel ranking)
        {
            DefaultAvatarImageSource32 ??= new BitmapImage(new Uri("ms-appx:///Assets/Profile/img_default_avatar.jpeg"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 32,
            };

            this.DotaHeroRanking = ranking;
            this.AvatarImage = new AsyncImage(this.DotaHeroRanking.avatar, 0, 32, DefaultAvatarImageSource32);
        }
    }
}
