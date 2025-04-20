using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Data.DataShop.ImageDownloader
{
    internal static class ImageDownloader
    {
        private readonly static HttpClient _httpClient = new();

        private readonly static Dictionary<string, BitmapImage> _imageCache = [];

        /// <summary>
        /// 获取一张图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="shouldCache"></param>
        /// <returns></returns>
        internal static async Task<BitmapImage?> LoadImageAsync(string url, int width, int height, bool shouldCache)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new Exception("Invalid URL");
                }

                string imgKey = $"{url}_{width}_{height}";

                if (_imageCache.TryGetValue(url, out BitmapImage? value))
                {
                    return value;
                }

                using var memStream = await GetImage(url, shouldCache);
                var stream = memStream?.AsRandomAccessStream();
                if (stream?.Size > 0)
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);

                    bitmapImage.DecodePixelType = DecodePixelType.Logical;

                    if (width > 0)
                    {
                        bitmapImage.DecodePixelWidth = width;
                    }

                    if (height > 0)
                    {
                        bitmapImage.DecodePixelHeight = height;
                    }

                    _imageCache[imgKey] = bitmapImage;

                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return null;
        }

        /// <summary>
        /// 带缓存的图像下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="shouldCache"></param>
        /// <returns></returns>
        private static async Task<MemoryStream?> GetImage(string url, bool shouldCache)
        {
            try
            {
                string tmpFileName = $"dotahold_tmp_{GetStringMD5(url)}";
                var cachedFile = await ImageCacheManager.TryGetCachedFileAsync(tmpFileName);

                if (cachedFile is not null)
                {
                    using var fileStream = await cachedFile.OpenStreamForReadAsync();
                    var memStream = new MemoryStream();
                    await fileStream.CopyToAsync(memStream);
                    memStream.Position = 0;
                    return memStream;
                }

                using var resStream = await DownloadImage(url);
                if (resStream is not null)
                {
                    var memStream = new MemoryStream();
                    await resStream.CopyToAsync(memStream);
                    memStream.Position = 0;

                    if (shouldCache)
                    {
                        var newCachedFile = await ImageCacheManager.CreateCacheFileAsync(tmpFileName);
                        if (newCachedFile.Value.File is not null)
                        {
                            using var fileStream = await newCachedFile.Value.File.OpenStreamForWriteAsync();
                            await memStream.CopyToAsync(fileStream);
                            memStream.Position = 0;

                            await ImageCacheManager.FinishCacheFileAsync(newCachedFile.Value, true);
                        }
                    }

                    return memStream;
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return null;
        }

        private static async Task<Stream?> DownloadImage(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new Exception("Invalid URL");
                }

                var response = await _httpClient.GetAsync(new Uri(url));
                // response.EnsureSuccessStatusCode();

                if (response?.Content is not null)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync($"Donwload image failed, Url is {url}, {ex.Message}", LogCourier.LogType.Error);
            }

            return null;
        }

        private static string GetStringMD5(string originalString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originalString))
                {
                    return "";
                }

                byte[] data = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.Default.GetBytes(originalString));

                StringBuilder sBuilder = new();
                for (int i = 0; i < data.Length; ++i)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return originalString;
        }
    }
}
