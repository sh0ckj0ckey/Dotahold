using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    internal class ItemModel
    {
        /// <summary>
        /// 默认物品图片
        /// </summary>
        public static BitmapImage? DefaultItemImageSource72 = null;

        public DotaItemModel DotaItemAttributes { get; private set; }

        public AsyncImage ItemImage { get; private set; }

        public ItemModel(DotaItemModel item)
        {
            DefaultItemImageSource72 ??= new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon_placeholder.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 144
            };

            this.DotaItemAttributes = item;
            this.ItemImage = new AsyncImage($"https://steamcdn-a.akamaihd.net{this.DotaItemAttributes.img}", 0, 144, DefaultItemImageSource72);
        }
    }
}
