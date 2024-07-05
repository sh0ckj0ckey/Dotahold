using Dotahold.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.DataShop.ImageLoader
{
    internal static class ImageLoader
    {
        private static HttpClient _http = new HttpClient();

        private static Dictionary<string, BitmapImage> _dictImageCache = new Dictionary<string, BitmapImage>();

        internal static async Task<BitmapImage> LoadImageAsync(string uri, int width, int height, bool cache)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uri)) return null;

                string imgKey = $"{uri}_{width}_{height}";

                if (_dictImageCache.ContainsKey(uri))
                {
                    return _dictImageCache[uri];
                }

                BitmapImage bm = null;
                using (var memStream = await DownloadImage(uri, cache))
                {
                    if (memStream == null)
                    {
                        return null;
                    }
                    var stream = memStream.AsRandomAccessStream();
                    if ((stream?.Size ?? 0) > 0)
                    {
                        bm = new BitmapImage();
                        await bm.SetSourceAsync(stream);
                    }
                }

                if (bm == null) return null;

                bm.DecodePixelType = DecodePixelType.Logical;
                if (width > 0)
                {
                    bm.DecodePixelWidth = width;
                }
                if (height > 0)
                {
                    bm.DecodePixelHeight = height;
                }

                if (_dictImageCache.Count > 5000)
                {
                    _dictImageCache.Clear();
                }
                _dictImageCache[imgKey] = bm;

                return bm;
            }
            catch
            {
                return null;
            }
        }

        // 带缓存的图像下载
        private static async Task<MemoryStream> DownloadImage(string Uri, bool bCache)
        {
            try
            {
                //string tmpFileName = System.Text.RegularExpressions.Regex.Replace(Uri, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");
                string tmpFileName = "dotahold_tmp_" + MD5Utils.StringMD5(Uri);
                var cachedFile = await ImageCacheManager.GetCachedFileAsync(tmpFileName);
                if (cachedFile == null)
                {
                    //没有对应的缓存文件
                    using (var resStream = await (await GetImage(Uri))?.Content?.ReadAsStreamAsync())
                    {
                        if (resStream != null)
                        {
                            var memStream = new MemoryStream();
                            await resStream.CopyToAsync(memStream);
                            memStream.Position = 0;

                            if (bCache)
                            {
                                var newCachedFile = await ImageCacheManager.CreateCacheFileAsync(tmpFileName);
                                if (newCachedFile == null) return memStream;
                                using (var fileStream = await newCachedFile.Value.File.OpenStreamForWriteAsync())
                                {
                                    await memStream.CopyToAsync(fileStream);
                                }
                                await ImageCacheManager.FinishCachedFileAsync(newCachedFile.Value, true);
                                memStream.Position = 0;
                            }

                            return memStream;
                        }
                    }
                }
                else
                {
                    //有缓存文件
                    using (var fileStream = await cachedFile.OpenStreamForReadAsync())
                    {
                        var memStream = new MemoryStream();
                        await fileStream.CopyToAsync(memStream);
                        memStream.Position = 0;
                        return memStream;
                    }
                }
            }
            catch { }
            return null;
        }

        private static async Task<HttpResponseMessage> GetImage(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return null;
                var response = await _http.GetAsync(new Uri(url));
                //response.EnsureSuccessStatusCode();
                return response;
            }
            catch { return null; }
        }
    }
}
