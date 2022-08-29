using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OpenDota_UWP.Helpers
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public class ConstantsCourier
    {
        private static Lazy<ConstantsCourier> _lazyVM = new Lazy<ConstantsCourier>(() => new ConstantsCourier());
        public static ConstantsCourier Instance => _lazyVM.Value;

        private Windows.Web.Http.HttpClient _constantsHttpClient = new Windows.Web.Http.HttpClient();

        private const string _heroesJsonFileName = "heroesjson";
        private const string _itemsJsonFileName = "itemsjson";
        private const string _buffsJsonFileName = "permanentbuffsjson";
        private const string _abilitiesJsonFileName = "abilitiesjson";

        private string _heroesBuildInJson = string.Empty;
        private string _itemsBuildInJson = string.Empty;
        private string _buffsBuildInJson = string.Empty;
        private string _abilitiesBuildInJson = string.Empty;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private Dictionary<string, string> _dictPermanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private Dictionary<string, string> _dictAbilitiesId = null;


        /// <summary>
        /// 获取英雄列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Models.DotaHeroModel>> GetHeroesConstant()
        {
            Dictionary<string, Models.DotaHeroModel> heroesConst = null;
            try
            {
                //if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                //{
                //    // if Time > 24h, then download new file
                //    var heroes = await DownloadConstant<Dictionary<string, Models.DotaHeroModel>>("heroes");
                //    return heroes;
                //}

                // 先去本地的Local文件夹找下载的最新的
                string json = string.Empty;
                try
                {
                    json = await StorageFilesCourier.ReadFileAsync(_heroesJsonFileName);
                }
                catch { json = string.Empty; }

                // 找不到就用内置的
                if (string.IsNullOrEmpty(json))
                {
                    try
                    {
                        json = await LoadBuildInJsonString(_heroesJsonFileName);
                    }
                    catch { json = string.Empty; }
                }

                // 内置的也找不到(理论上不会找不到，以防万一而已)
                if (string.IsNullOrEmpty(json))
                {
                    try
                    {
                        heroesConst = await DownloadConstant<Dictionary<string, Models.DotaHeroModel>>("heroes");
                    }
                    catch { }
                }
            }
            catch { }
            return heroesConst;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Models.DotaItemModel>> GetItemsConstant()
        {
            try
            {
                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var items = await DownloadConstant<Dictionary<string, Models.DotaItemModel>>("items");
                    return items;
                }
                else
                {
                    // return local file
                    return null;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取永久buff列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetPermanentBuffsConstant()
        {
            try
            {
                if (_dictPermanentBuffs != null) return _dictPermanentBuffs;

                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true/*time is ok*/ && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var buffs = await DownloadConstant<Dictionary<string, string>>("permanent_buffs");

                    if (buffs != null) _dictPermanentBuffs = buffs;

                    return _dictPermanentBuffs;
                }
                else
                {
                    // return local file
                    return null;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取技能ID列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAbilityIDsConstant()
        {
            try
            {
                if (_dictAbilitiesId != null) return _dictAbilitiesId;

                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true/*time is ok*/ && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var abilities = await DownloadConstant<Dictionary<string, string>>("ability_ids");

                    if (abilities != null) _dictAbilitiesId = abilities;

                    return _dictAbilitiesId;
                }
                else
                {
                    // return local file
                    return null;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 下载指定的Constant json文件，存储到本地，然后返回这个对象
        /// (根据需要的频率去调用更新)
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>下载的json反序列化对象</returns>
        private async Task<T> DownloadConstant<T>(string resource)
        {
            string url = "https://api.opendota.com/api/constants/" + resource;
            try
            {
                var response = await _constantsHttpClient.GetAsync(new Uri(url));
                string jsonMessage = await response.Content.ReadAsStringAsync();
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                var constantModel = JsonConvert.DeserializeObject<T>(jsonMessage, jsonSerializerSettings);

                if (constantModel != null)
                {
                    // await StorageFileHelper.WriteAsync(constantModel, resource + ".dota");
                    return constantModel;
                }
            }
            catch { }
            return default;
        }

        /// <summary>
        /// 读取内置的json
        /// </summary>
        /// <param name="jsonName"></param>
        /// <returns></returns>
        private async Task<string> LoadBuildInJsonString(string jsonName)
        {
            try
            {
                if (_heroesJsonFileName == jsonName)
                {
                    if (string.IsNullOrEmpty(_heroesBuildInJson))
                    {
                        _heroesBuildInJson = string.Empty;

                        string fname = @"ConstantsJsons\heroes.json";
                        StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                        StorageFile file = await installFolder.GetFileAsync(fname);
                        if (file != null/* && File.Exists(file.Path)*/)
                        {
                            IRandomAccessStream accessStream = await file.OpenReadAsync();
                            using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                            {
                                _heroesBuildInJson = streamReader.ReadToEnd();
                            }
                        }
                    }
                    return _heroesBuildInJson;
                }
                else if (_itemsJsonFileName == jsonName)
                {
                    if (string.IsNullOrEmpty(_itemsBuildInJson))
                    {
                        _itemsBuildInJson = "";
                    }
                    return _itemsBuildInJson;
                }
                else if (_buffsJsonFileName == jsonName)
                {
                    if (string.IsNullOrEmpty(_buffsBuildInJson))
                    {
                        _buffsBuildInJson = "";
                    }
                    return _buffsBuildInJson;
                }
                else if (_abilitiesJsonFileName == jsonName)
                {
                    if (string.IsNullOrEmpty(_abilitiesBuildInJson))
                    {
                        _abilitiesBuildInJson = "";
                    }
                    return _abilitiesBuildInJson;
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
