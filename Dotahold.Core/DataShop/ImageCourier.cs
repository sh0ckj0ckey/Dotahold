using System.Threading.Tasks;
using Dotahold.Core.DataShop.ImageDownloader;
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
        /// <param name="uri"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetImageAsync(string uri, int width, int height, bool cache = true)
        {
            return await ImageDownloader.ImageDownloader.LoadImageAsync(uri, width, height, cache);
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
