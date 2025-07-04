﻿using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class HeroModel
    {
        /// <summary>
        /// 默认英雄图片
        /// </summary>
        private static BitmapImage? _defaultHeroImageSource144 = null;

        /// <summary>
        /// 默认英雄图标
        /// </summary>
        private static BitmapImage? _defaultHeroIconSource36 = null;

        public DotaHeroModel DotaHeroAttributes { get; private set; }

        public AsyncImage HeroImage { get; private set; }

        public AsyncImage HeroIcon { get; private set; }

        public HeroModel(DotaHeroModel hero)
        {
            _defaultHeroImageSource144 ??= new BitmapImage(new Uri("ms-appx:///Assets/img_placeholder.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 144,
            };

            _defaultHeroIconSource36 ??= new BitmapImage(new Uri("ms-appx:///Assets/icon_dota2_color.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 36,
            };

            this.DotaHeroAttributes = hero;
            this.HeroImage = new AsyncImage($"{Dotahold.Data.DataShop.ConstantsCourier.ImageSourceDomain}{this.DotaHeroAttributes.img}", 0, 144, _defaultHeroImageSource144);
            this.HeroIcon = new AsyncImage($"{Dotahold.Data.DataShop.ConstantsCourier.ImageSourceDomain}{this.DotaHeroAttributes.icon}", 0, 36, _defaultHeroIconSource36);
        }
    }
}
