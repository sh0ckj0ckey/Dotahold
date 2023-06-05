using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Helpers
{
    public static class ImageLoader
    {
        private static HttpClient http = new HttpClient();

        private static Dictionary<string, BitmapImage> dictImageCache = new Dictionary<string, BitmapImage>();

        public static async Task<BitmapImage> LoadImageAsync(string Uri, string defaultImg = "ms-appx:///Assets/Icons/item_placeholder.png")
        {
            try
            {
                if (dictImageCache.ContainsKey(Uri))
                {
                    return dictImageCache[Uri];
                }

                BitmapImage bm = null;
                using (var memStream = await DownloadImage(Uri))
                {
                    if (memStream == null)
                    {
                        return string.IsNullOrEmpty(defaultImg) ? null : new BitmapImage(new System.Uri(defaultImg));
                    }
                    bm = new BitmapImage();
                    await bm.SetSourceAsync(memStream.AsRandomAccessStream());
                }

                dictImageCache[Uri] = bm;

                return bm;
            }
            catch
            {
                try
                {
                    return new BitmapImage(new System.Uri(defaultImg));
                }
                catch { }
                return null;
            }
        }


        //带缓存的图像下载
        public static async Task<MemoryStream> DownloadImage(string Uri)
        {
            try
            {
                string tmpFileName = System.Text.RegularExpressions.Regex.Replace(Uri, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");
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
                            var newCachedFile = await ImageCacheManager.CreateCacheFileAsync(tmpFileName);
                            if (newCachedFile == null) return null;
                            using (var fileStream = await newCachedFile.Value.File.OpenStreamForWriteAsync())
                            {
                                await memStream.CopyToAsync(fileStream);
                            }
                            await ImageCacheManager.FinishCachedFileAsync(newCachedFile.Value, true);
                            memStream.Position = 0;
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
