using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

/// <summary>
/// https://github.com/tobiichiamane/pixivfs-uwp by tobiichiamane
/// </summary>
namespace Dotahold.Core.DataShop.ImageLoader
{
    //记录临时的缓存文件及其期望的名称
    internal struct TempStorageFile
    {
        internal StorageFile File;
        internal string ExpectedName;
    }

    //用来管理缓存
    internal static class ImageCacheManager
    {
        //临时目录
        private static StorageFolder tmpFolder = ApplicationData.Current.TemporaryFolder;

        // 获取临时目录，确保目录存在
        internal static async Task<StorageFolder> GetCacheFolderAsync()
        {
            var cacheFolder = await tmpFolder.TryGetItemAsync("Cache");
            if (cacheFolder == null) return await tmpFolder.CreateFolderAsync("Cache");
            else return cacheFolder as StorageFolder;
        }

        // 获取缓存目录大小
        internal static async Task<long> GetCacheSizeAsync()
        {
            try
            {
                var tar = await GetCacheFolderAsync();
                var size = await GetFolderSizeAsync(tar);
                return size;
            }
            catch { }
            return 0;
        }

        // 获取指定目录大小
        private static async Task<long> GetFolderSizeAsync(StorageFolder target)
        {
            try
            {
                var getFileSizeTasks = from file
                                       in await target.CreateFileQuery().GetFilesAsync()
                                       select file.GetBasicPropertiesAsync().AsTask();
                var fileSizes = await Task.WhenAll(getFileSizeTasks);
                return fileSizes.Sum(i => (long)i.Size);
            }
            catch { }
            return -1;
        }

        //得到缓存文件
        internal static async Task<StorageFile> GetCachedFileAsync(string Filename)
        {
            try
            {
                var cacheFolder = await GetCacheFolderAsync();
                if (await cacheFolder.TryGetItemAsync(Filename) == null) return null;
                return await cacheFolder.GetFileAsync(Filename);
            }
            catch { }
            return null;
        }

        //删除缓存文件
        internal static async Task<bool> DeleteCachedFileAsync(string Filename)
        {
            try
            {
                var file = await GetCachedFileAsync(Filename);
                if (file == null) return false;
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch { }
            return false;
        }

        // 创建缓存文件
        // 文件将以随机的Guid命名，完成写入时再命名为期望的文件名
        // 此举有助于防止写入一半时程序非法终止而产生损坏的缓存文件
        internal static async Task<TempStorageFile?> CreateCacheFileAsync(string Filename)
        {
            try
            {
                Guid guid;
                var test = Guid.TryParse(Filename, out guid);
                // 避免使用GUID作为文件名
                if (test) return null;
                do guid = Guid.NewGuid();
                while (await GetCachedFileAsync(guid.ToString()) != null);
                var cacheFolder = await GetCacheFolderAsync();
                var newFile = await cacheFolder.CreateFileAsync(guid.ToString());
                TempStorageFile result = new TempStorageFile();
                result.ExpectedName = Filename;
                result.File = newFile;
                return result;
            }
            catch { }
            return null;
        }

        // 完成写入缓存文件
        // ForceUpdate: 是否覆盖已有的文件
        internal static async Task<bool> FinishCachedFileAsync(TempStorageFile File, bool ForceUpdate = false)
        {
            try
            {
                if (await GetCachedFileAsync(File.ExpectedName) != null)
                {
                    if (ForceUpdate) await DeleteCachedFileAsync(File.ExpectedName);
                    else return false;
                }
                await File.File.MoveAsync(await GetCacheFolderAsync(), File.ExpectedName, NameCollisionOption.ReplaceExisting);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 清理缓存
        internal static async Task<bool> ClearCacheAsync()
        {
            try
            {
                var cacheFolder = await GetCacheFolderAsync();
                //var files = (await cacheFolder.GetFilesAsync()).Where(p => p.DisplayName.StartsWith("http"));
                //foreach (var file in files)
                //{
                //    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                //}
                await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch { }
            return false;
        }

        // 清理名称为GUID的临时文件
        internal static async Task ClearTempFilesAsync()
        {
            try
            {
                var deleteTempFilesTasks = from file
                                           in await (await GetCacheFolderAsync()).CreateFileQuery().GetFilesAsync()
                                           where Guid.TryParse(file.Name, out _)
                                           select file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
                await Task.WhenAll(deleteTempFilesTasks);
            }
            catch { }
        }
    }
}