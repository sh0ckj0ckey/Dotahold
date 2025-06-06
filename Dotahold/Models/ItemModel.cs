﻿using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class ItemModel
    {
        /// <summary>
        /// 默认物品图片
        /// </summary>
        private static BitmapImage? _defaultItemImageSource84 = null;

        public DotaItemModel DotaItemAttributes { get; private set; }

        public AsyncImage ItemImage { get; private set; }

        public ItemModel(DotaItemModel item)
        {
            _defaultItemImageSource84 ??= new BitmapImage(new Uri("ms-appx:///Assets/img_placeholder.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 84
            };

            this.DotaItemAttributes = item;
            this.ItemImage = new AsyncImage($"{Dotahold.Data.DataShop.ConstantsCourier.ImageSourceDomain}{this.DotaItemAttributes.img}", 0, 84, _defaultItemImageSource84);
        }
    }
}
