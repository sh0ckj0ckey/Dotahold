using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

/// <summary>
/// https://github.com/tobiichiamane/pixivfs-uwp by tobiichiamane
/// </summary>
namespace Dotahold.Core.DataShop.ImageDownloader
{
    /// <summary>
    /// 记录临时的缓存文件及其期望的名称
    /// </summary>
    internal struct TempStorageFile
    {
        internal StorageFile File;
        internal string ExpectedName;
    }

    /// <summary>
    /// 图片缓存管理
    /// </summary>
    internal static class ImageCacheManager
    {
        /// <summary>
        /// 获取图片缓存目录，如果不存在会创建
        /// </summary>
        /// <returns></returns>
        internal static async Task<StorageFolder> GetCacheFolderAsync()
        {
            var tempFolder = ApplicationData.Current.TemporaryFolder;
            var cacheFolder = await tempFolder.TryGetItemAsync("Cache");
            if (cacheFolder is null)
            {
                return await tempFolder.CreateFolderAsync("Cache");
            }
            else
            {
                return cacheFolder as StorageFolder;
            }
        }

        /// <summary>
        /// 获取缓存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static async Task<StorageFile> GetCachedFileAsync(string fileName)
        {
            try
            {
                var cacheFolder = await GetCacheFolderAsync();
                if (await cacheFolder.TryGetItemAsync(fileName) == null)
                {
                    return null;
                }

                return await cacheFolder.GetFileAsync(fileName);
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
                return null;
            }
        }

        /// <summary>
        /// 删除缓存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static async Task DeleteCachedFileAsync(string fileName)
        {
            try
            {
                var file = await GetCachedFileAsync(fileName);
                await file?.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// 创建缓存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static async Task<TempStorageFile?> CreateCacheFileAsync(string fileName)
        {
            try
            {
                // 避免使用GUID作为文件名
                if (string.IsNullOrWhiteSpace(fileName) || Guid.TryParse(fileName, out _))
                {
                    return null;
                }

                Guid guid;
                do guid = Guid.NewGuid();
                while (await GetCachedFileAsync(guid.ToString()) != null);

                var cacheFolder = await GetCacheFolderAsync();
                var newFile = await cacheFolder.CreateFileAsync(guid.ToString());

                return new TempStorageFile
                {
                    ExpectedName = fileName,
                    File = newFile
                };
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return null;
        }

        /// <summary>
        /// 完成写入缓存文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="forceUpdate">是否覆盖已有的文件</param>
        /// <returns></returns>
        internal static async Task<bool> FinishCacheFileAsync(TempStorageFile file, bool forceUpdate = false)
        {
            try
            {
                if (await GetCachedFileAsync(file.ExpectedName) != null)
                {
                    if (forceUpdate)
                    {
                        await DeleteCachedFileAsync(file.ExpectedName);
                    }
                    else
                    {
                        return false;
                    }
                }

                await file.File.MoveAsync(await GetCacheFolderAsync(), file.ExpectedName, NameCollisionOption.ReplaceExisting);

                return true;
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
                return false;
            }
        }

        /// <summary>
        /// 获取缓存目录大小
        /// </summary>
        /// <returns></returns>
        internal static async Task<long> GetCacheSizeAsync()
        {
            try
            {
                var cacheFolder = await GetCacheFolderAsync();

                var getFileSizeTasks = from file
                                       in await cacheFolder.CreateFileQuery().GetFilesAsync()
                                       select file.GetBasicPropertiesAsync().AsTask();

                var fileSizes = await Task.WhenAll(getFileSizeTasks);

                return fileSizes.Sum(i => (long)i.Size);
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
                return 0;
            }
        }

        /// <summary>
        /// 清理整个缓存目录
        /// </summary>
        /// <returns></returns>
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return false;
        }

        /// <summary>
        /// 清理名称为GUID的临时文件
        /// </summary>
        /// <returns></returns>
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}