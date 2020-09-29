using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenDota_UWP.Helpers
{
    public static class ImageLoader
    {
        private static HttpClient http = new HttpClient();

        public static async Task<BitmapImage> LoadImageAsync(string Uri)
        {
            try
            {
                var bm = new BitmapImage();
                using (var memStream = await DownloadImage(Uri))
                {
                    if (memStream == null)
                    {
                        return new BitmapImage(new System.Uri("ms-appx:///Assets/Icons/loadwrong.png"));
                    }
                    await bm.SetSourceAsync(memStream.AsRandomAccessStream());
                }
                return bm;
            }
            catch
            {
                return new BitmapImage(new System.Uri("ms-appx:///Assets/Icons/loadwrong.png"));
            }
        }


        //带缓存的图像下载
        public static async Task<MemoryStream> DownloadImage(string Uri)
        {
            try
            {
                string tmpFileName = System.Text.RegularExpressions.Regex.Replace(Uri, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");
                var cachedFile = await CacheManager.GetCachedFileAsync(tmpFileName);
                if (cachedFile == null)
                {
                    //没有对应的缓存文件
                    var img = await GetImage(Uri);
                    if (img == null)
                    {
                        return null;
                    }
                    using (var resStream = await (await GetImage(Uri)).Content.ReadAsStreamAsync())
                    {
                        var memStream = new MemoryStream();
                        await resStream.CopyToAsync(memStream);
                        memStream.Position = 0;
                        var newCachedFile = await CacheManager.CreateCacheFileAsync(tmpFileName);
                        using (var fileStream = await newCachedFile.Value.File.OpenStreamForWriteAsync())
                        {
                            await memStream.CopyToAsync(fileStream);
                        }
                        await CacheManager.FinishCachedFileAsync(newCachedFile.Value, true);
                        memStream.Position = 0;
                        return memStream;
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
            catch { return null; }
        }

        private static async Task<HttpResponseMessage> GetImage(string url)
        {
            try
            {
                var response = await http.GetAsync(new Uri(url));
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch { return null; }
        }
    }
}
