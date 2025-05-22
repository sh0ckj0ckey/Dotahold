using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的本地文件
    /// </summary>
    public static class StorageFilesCourier
    {
        /// <summary>
        /// 存储数据的文件夹名称
        /// </summary>
        private const string DataFolderName = "DotaholdData";

        /// <summary>
        /// 存储数据的文件夹对象
        /// </summary>
        private static StorageFolder? DataFolder = null;

        /// <summary>
        /// 获取存储数据的文件夹的对象
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetDataFolder()
        {
            DataFolder ??= await ApplicationData.Current.LocalFolder.CreateFolderAsync(DataFolderName, CreationCollisionOption.OpenIfExists);
            return DataFolder;
        }

        /// <summary>
        /// 读取本地文件夹根目录的文件，默认从OpenDotaData目录读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileAsync(string fileName, StorageFolder? applicationFolder = null)
        {
            string text = string.Empty;

            try
            {
                applicationFolder ??= await GetDataFolder();

                if (await applicationFolder.TryGetItemAsync(fileName) is not null)
                {
                    var storageFile = await applicationFolder.GetFileAsync(fileName);

                    if (storageFile is not null)
                    {
                        IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                        using StreamReader streamReader = new(accessStream.AsStreamForRead((int)accessStream.Size));
                        text = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return text;
        }

        /// <summary>
        /// 写入本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<bool> WriteFileAsync(string fileName, string content, StorageFolder? applicationFolder = null)
        {
            try
            {
                applicationFolder ??= await GetDataFolder();

                var storageFile = await applicationFolder.CreateFileAsync(fileName + "Tmp", CreationCollisionOption.ReplaceExisting);

                int retryAttempts = 3;
                const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
                const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);

                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        await FileIO.WriteTextAsync(storageFile, content);
                        await storageFile.RenameAsync(fileName, NameCollisionOption.ReplaceExisting);
                        return true;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                    catch (Exception ex)
                    {
                        LogCourier.Log(ex.Message, LogCourier.LogType.Error);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return false;
        }
    }
}
