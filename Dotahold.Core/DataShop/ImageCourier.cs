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
    /// <summary>
    /// 这是一只信使，帮你运送你需要的下载图片
    /// </summary>
    public class ImageCourier
    {
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="Uri"></param>
        /// <param name="bCache"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetImageAsync(string Uri, bool bCache = true)
        {
            return await ImageLoader.ImageLoader.LoadImageAsync(Uri, bCache);
        }

        /// <summary>
        /// 获取缓存目录
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetCacheFolderAsync()
        {
            return await ImageCacheManager.GetCacheFolderAsync();
        }

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        public static async Task<long> GetCacheSizeAsync()
        {
            return await ImageCacheManager.GetCacheSizeAsync();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ClearCacheAsync()
        {
            return await ImageCacheManager.ClearCacheAsync();
        }
    }
}
