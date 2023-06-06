using Dotahold.Core.DataShop.ImageLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.DataShop
{
    public class ImageCourier
    {
        public static async Task<BitmapImage> GetImageAsync(string Uri, bool bCache = true)
        {
            return await ImageLoader.ImageLoader.LoadImageAsync(Uri, bCache);
        }

        public static async Task<StorageFolder> GetCacheFolderAsync()
        {
            return await ImageCacheManager.GetCacheFolderAsync();
        }

        public static async Task<long> GetCacheSizeAsync()
        {
            return await ImageCacheManager.GetCacheSizeAsync();
        }

        public static async Task<bool> ClearCacheAsync()
        {
            return await ImageCacheManager.ClearCacheAsync();
        }
    }
}
