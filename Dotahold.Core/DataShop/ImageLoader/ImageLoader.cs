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
        private static HttpClient http = new HttpClient();

        private static Dictionary<string, BitmapImage> dictImageCache = new Dictionary<string, BitmapImage>();

        internal static async Task<BitmapImage> LoadImageAsync(string Uri, bool bCache)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Uri)) return null;

                if (dictImageCache.ContainsKey(Uri))
                {
                    return dictImageCache[Uri];
                }

                BitmapImage bm = null;
                using (var memStream = await DownloadImage(Uri, bCache))
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

                if (dictImageCache.Count > 3000)
                {
                    dictImageCache.Clear();
                }
                dictImageCache[Uri] = bm;

                return bm;
            }
            catch
            {
                try
                {
                    return null;
                }
                catch { }
                return null;
            }
        }

        // 带缓存的图像下载
        internal static async Task<MemoryStream> DownloadImage(string Uri, bool bCache)
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
                var response = await http.GetAsync(new Uri(url));
                //response.EnsureSuccessStatusCode();
                return response;
            }
            catch { return null; }
        }
    }
}
