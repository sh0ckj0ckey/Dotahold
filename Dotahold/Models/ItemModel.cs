using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    internal class ItemModel
    {
        /// <summary>
        /// 默认物品图片
        /// </summary>
        public static BitmapImage? DefaultItemImageSource42 = null;

        public DotaItemModel DotaItemAttributes { get; private set; }

        public AsyncImage ItemImage { get; private set; }

        public ItemModel(DotaItemModel item)
        {
            DefaultItemImageSource42 ??= new BitmapImage(new Uri("ms-appx:///Assets/img_placeholder.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 84
            };

            this.DotaItemAttributes = item;
            this.ItemImage = new AsyncImage($"{Dotahold.Data.DataShop.ConstantsCourier.ImageSourceDomain}{this.DotaItemAttributes.img}", 0, 84, DefaultItemImageSource42);
        }
    }
}
