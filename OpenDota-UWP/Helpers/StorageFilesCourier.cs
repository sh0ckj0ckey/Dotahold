using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OpenDota_UWP.Helpers
{
    public class StorageFilesCourier
    {
        //存储数据的文件夹名称
        private const string DataFolderName = "OpenDotaData";

        //存储数据的文件夹对象(单例，见下面的GetDataFolder方法)
        private static IStorageFolder DataFolder = null;

        /// <summary>
        /// 获取存储数据的文件夹的对象
        /// </summary>
        /// <returns></returns>
        public static async Task<IStorageFolder> GetDataFolder()
        {
            if (DataFolder == null)
            {
                DataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DataFolderName, CreationCollisionOption.OpenIfExists);
            }
            return DataFolder;
        }

        /// <summary>
        /// 读取本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileAsync(string fileName)
        {
            string text = string.Empty;
            try
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                IStorageFile storageFile = await applicationFolder.GetFileAsync(fileName);
                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                {
                    text = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //text = "文件读取错误：" + e.Message;
            }
            return text;
        }

        /// <summary>
        /// 写入本地文件夹根目录的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> WriteFileAsync(string fileName, string content)
        {
            try
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                IStorageFile storageFile = await applicationFolder.CreateFileAsync("SavingTempFile.pswd", CreationCollisionOption.ReplaceExisting);

                Int32 retryAttempts = 3;
                const Int32 ERROR_ACCESS_DENIED = unchecked((Int32)0x80070005);
                const Int32 ERROR_SHARING_VIOLATION = unchecked((Int32)0x80070020);

                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        await FileIO.WriteTextAsync(storageFile, content);
                        await storageFile.RenameAsync(fileName, NameCollisionOption.ReplaceExisting);
                        return string.Empty;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) || (ex.HResult == ERROR_SHARING_VIOLATION))
                    {
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception e) { return "写入失败：" + e.Message; }
                }
                return "写入失败：文件访问被拒绝或者文件被占用";
            }
            catch (Exception e) { return "写入失败：" + e.Message; }
        }
    }
}
