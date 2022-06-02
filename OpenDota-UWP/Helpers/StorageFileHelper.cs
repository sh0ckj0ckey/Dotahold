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
    public class StorageFileHelper
    {
        //存储数据的文件夹名称
        private const string FolderName = "DataCache";

        //存储数据的文件夹对象(单例，见下面的GetDataFolder方法)
        private static IStorageFolder DataFolder = null;

        //获取存储数据的文件夹的对象
        private static async Task<IStorageFolder> GetDataFolder()
        {
            if (DataFolder == null)
            {
                DataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
            }
            return DataFolder;
        }

        //写入本地文件夹根目录的文件
        public static async Task WriteFileAsync(string fileName, string content)
        {
            IStorageFolder applicationFolder = await GetDataFolder();
            IStorageFile storageFile = await applicationFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        //读取本地文件夹根目录的文件
        public static async Task<string> ReadFileAsync(string fileName)
        {
            try
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                IStorageFile storageFile = await applicationFolder.GetFileAsync(fileName);
                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //LogHelper.Log(e.Message);
            }
            return "";
        }

        //把实体类对象序列化成XML格式存储到文件里面
        public static async Task WriteAsync<T>(T data, string filename)
        {
            IStorageFolder applicationFolder = await GetDataFolder();
            StorageFile file = await applicationFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(outStream.AsStreamForWrite(), data);
                    await outStream.FlushAsync();
                }
            }
        }

        //反序列化XML文件
        public static async Task<T> ReadAsync<T>(string filename)
        {
            //获取实体类类型实例化一个对象
            T sessionState_ = default(T);
            IStorageFolder applicationFolder = await GetDataFolder();
            StorageFile file = await applicationFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            if (file == null)
            {
                return sessionState_;
            }
            try
            {
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    sessionState_ = (T)serializer.ReadObject(inStream.AsStreamForRead());
                }
            }
            catch (Exception)
            { }
            return sessionState_;
        }
    }
}
