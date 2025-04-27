using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    internal class HeroModel
    {
        /// <summary>
        /// 默认英雄图片
        /// </summary>
        public static BitmapImage? DefaultHeroImageSource72 = null;

        public DotaHeroModel DotaHeroAttributes { get; private set; }

        public AsyncImage HeroImage { get; private set; }

        public AsyncImage HeroIcon { get; private set; }

        public HeroModel(DotaHeroModel hero)
        {
            DefaultHeroImageSource72 ??= new BitmapImage(new Uri("ms-appx:///Assets/img_placeholder.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 144,
            };

            this.DotaHeroAttributes = hero;
            this.HeroImage = new AsyncImage($"https://cdn.cloudflare.steamstatic.com{this.DotaHeroAttributes.img}", 0, 144, DefaultHeroImageSource72);
            this.HeroIcon = new AsyncImage($"https://cdn.cloudflare.steamstatic.com{this.DotaHeroAttributes.icon}", 0, 36);
        }
    }
}
