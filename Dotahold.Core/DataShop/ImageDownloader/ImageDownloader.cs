using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.DataShop.ImageDownloader
{
    internal static class ImageDownloader
    {
        private readonly static HttpClient _httpClient = new HttpClient();

        private readonly static Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 获取一张图片
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        internal static async Task<BitmapImage> LoadImageAsync(string uri, int width, int height, bool cache)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uri))
                {
                    return null;
                }

                string imgKey = $"{uri}_{width}_{height}";

                if (_imageCache.ContainsKey(uri))
                {
                    return _imageCache[uri];
                }

                BitmapImage bitmapImage = null;
                using (var memStream = await DownloadImage(uri, cache))
                {
                    var stream = memStream?.AsRandomAccessStream();
                    if (stream?.Size > 0)
                    {
                        bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(stream);
                    }
                }

                if (bitmapImage is null)
                {
                    return null;
                }

                bitmapImage.DecodePixelType = DecodePixelType.Logical;

                if (width > 0)
                {
                    bitmapImage.DecodePixelWidth = width;
                }

                if (height > 0)
                {
                    bitmapImage.DecodePixelHeight = height;
                }

                if (_imageCache.Count > 10240)
                {
                    _imageCache.Clear();
                }

                _imageCache[imgKey] = bitmapImage;

                return bitmapImage;
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
                return null;
            }
        }

        /// <summary>
        /// 带缓存的图像下载
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        private static async Task<MemoryStream> DownloadImage(string uri, bool cache)
        {
            try
            {
                string tmpFileName = $"dotahold_tmp_{GetStringMD5(uri)}";
                var cachedFile = await ImageCacheManager.GetCachedFileAsync(tmpFileName);

                if (cachedFile != null)
                {
                    using (var fileStream = await cachedFile.OpenStreamForReadAsync())
                    {
                        var memStream = new MemoryStream();
                        await fileStream.CopyToAsync(memStream);
                        memStream.Position = 0;
                        return memStream;
                    }
                }

                // 没有找到缓存文件
                using (var resStream = await (await GetImage(uri))?.Content?.ReadAsStreamAsync())
                {
                    if (resStream != null)
                    {
                        var memStream = new MemoryStream();
                        await resStream.CopyToAsync(memStream);
                        memStream.Position = 0;

                        if (cache)
                        {
                            var newCachedFile = await ImageCacheManager.CreateCacheFileAsync(tmpFileName);
                            if (newCachedFile != null)
                            {
                                using (var fileStream = await newCachedFile.Value.File?.OpenStreamForWriteAsync())
                                {
                                    await memStream.CopyToAsync(fileStream);
                                }

                                await ImageCacheManager.FinishCacheFileAsync(newCachedFile.Value, true);

                                memStream.Position = 0;
                            }
                        }

                        return memStream;
                    }
                    else
                    {
                        LogCourier.LogAsync($"Getting image {uri} failed, stream is null.", LogCourier.LogType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return null;
        }

        private static async Task<HttpResponseMessage> GetImage(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return null;
                var response = await _httpClient.GetAsync(new Uri(url));
                //response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
                return null;
            }
        }

        private static string GetStringMD5(string oriString)
        {
            try
            {
                if (oriString.Length == 0)
                {
                    return "";
                }
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] ret = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(oriString));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < ret.Length; ++i)
                {
                    sBuilder.Append(ret[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return oriString;
        }
    }
}
