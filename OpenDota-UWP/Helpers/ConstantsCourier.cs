using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Dotahold.Helpers
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public class ConstantsCourier
    {
        private static Lazy<ConstantsCourier> _lazyVM = new Lazy<ConstantsCourier>(() => new ConstantsCourier());
        public static ConstantsCourier Instance => _lazyVM.Value;

        private Windows.Web.Http.HttpClient _constantsHttpClient = new Windows.Web.Http.HttpClient();

        // 记录每个constant最近一次保存到本地的时间
        private Dictionary<string, long> _dictConstantsGottenDate = null;

        private const string _heroesJsonFileName = "heroesjson";
        private const string _itemsJsonFileName = "itemsjson";
        private const string _buffsJsonFileName = "permanentbuffsjson";
        private const string _abilitiesJsonFileName = "abilitiesjson";

        /// <summary>
        /// 英雄字典
        /// </summary>
        private Dictionary<string, Models.DotaHeroModel> _dictHeroes = null;

        /// <summary>
        /// 物品字典
        /// </summary>
        private Dictionary<string, Models.DotaItemModel> _dictItems = null;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private Dictionary<string, string> _dictPermanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private Dictionary<string, string> _dictAbilitiesId = null;


        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        public ConstantsCourier()
        {
            try
            {
                LoadConstantsGottenDate();
            }
            catch { }
        }

        /// <summary>
        /// 获取英雄列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Models.DotaHeroModel>> GetHeroesConstant()
        {
            try
            {
                // 先去本地的Local文件夹找下载的最新的
                if (_dictHeroes == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(_heroesJsonFileName);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictHeroes = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaHeroModel>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 找不到就用内置的
                if (_dictHeroes == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\heroes.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictHeroes = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaHeroModel>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 内置的也找不到(不太可能)
                if (_dictHeroes == null || Need2UpdateJson("heroes"))
                {
                    try
                    {
                        if (_dictHeroes == null)
                        {
                            var json = await GetConstant("heroes", _heroesJsonFileName);
                            if (!string.IsNullOrEmpty(json))
                            {
                                _dictHeroes = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaHeroModel>>(json, _jsonSerializerSettings);
                            }
                        }
                        else
                        {
                            await GetConstant("heroes", _heroesJsonFileName);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return _dictHeroes;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Models.DotaItemModel>> GetItemsConstant()
        {
            try
            {
                // 先去本地的Local文件夹找下载的最新的
                if (_dictItems == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(_itemsJsonFileName);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictItems = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaItemModel>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 找不到就用内置的
                if (_dictItems == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\items.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictItems = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaItemModel>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 内置的也找不到(不太可能)
                if (_dictItems == null || Need2UpdateJson("items"))
                {
                    try
                    {
                        if (_dictItems == null)
                        {
                            var json = await GetConstant("items", _itemsJsonFileName);
                            if (!string.IsNullOrEmpty(json))
                            {
                                _dictItems = JsonConvert.DeserializeObject<Dictionary<string, Models.DotaItemModel>>(json, _jsonSerializerSettings);
                            }
                        }
                        else
                        {
                            await GetConstant("items", _itemsJsonFileName);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return _dictItems;
        }

        /// <summary>
        /// 获取永久buff列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetPermanentBuffsConstant()
        {
            try
            {
                // 先去本地的Local文件夹找下载的最新的
                if (_dictPermanentBuffs == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(_buffsJsonFileName);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictPermanentBuffs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 找不到就用内置的
                if (_dictPermanentBuffs == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\permanent_buffs.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictPermanentBuffs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 内置的也找不到(不太可能)
                if (_dictPermanentBuffs == null || Need2UpdateJson("permanent_buffs"))
                {
                    try
                    {
                        if (_dictPermanentBuffs == null)
                        {
                            var json = await GetConstant("permanent_buffs", _buffsJsonFileName);
                            if (!string.IsNullOrEmpty(json))
                            {
                                _dictPermanentBuffs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                            }
                        }
                        else
                        {
                            await GetConstant("permanent_buffs", _buffsJsonFileName);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return _dictPermanentBuffs;
        }

        /// <summary>
        /// 获取技能ID列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAbilityIDsConstant()
        {
            try
            {
                // 先去本地的Local文件夹找下载的最新的
                if (_dictAbilitiesId == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(_abilitiesJsonFileName);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictAbilitiesId = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 找不到就用内置的
                if (_dictAbilitiesId == null)
                {
                    try
                    {
                        var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\ability_ids.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _dictAbilitiesId = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                        }
                    }
                    catch { }
                }

                // 内置的也找不到(不太可能)
                if (_dictAbilitiesId == null || Need2UpdateJson("ability_ids"))
                {
                    try
                    {
                        if (_dictAbilitiesId == null)
                        {
                            var json = await GetConstant("ability_ids", _abilitiesJsonFileName);
                            if (!string.IsNullOrEmpty(json))
                            {
                                _dictAbilitiesId = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _jsonSerializerSettings);
                            }
                        }
                        else
                        {
                            await GetConstant("ability_ids", _abilitiesJsonFileName);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return _dictAbilitiesId;
        }

        /// <summary>
        /// 下载指定的Constant json文件，存储到本地，然后返回
        /// </summary>
        /// <returns>下载的json</returns>
        private async Task<string> GetConstant(string constantName, string jsonFileName)
        {
            string url = "https://api.opendota.com/api/constants/" + constantName;
            try
            {
                var response = await _constantsHttpClient.GetAsync(new Uri(url));
                string jsonMessage = await response.Content.ReadAsStringAsync();

                try
                {
                    if (!string.IsNullOrEmpty(jsonMessage) && jsonMessage.Length > 96/*太短的话通常是请求失败*/)
                    {
                        bool written = await StorageFilesCourier.WriteFileAsync(jsonFileName, jsonMessage);

                        if (written)
                        {
                            _dictConstantsGottenDate[constantName] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            SaveConstantsGottenDate();
                        }
                    }
                }
                catch { }

                return jsonMessage;
            }
            catch { }
            return string.Empty;
        }


        private bool Need2UpdateJson(string constantName)
        {
            try
            {
                long nowDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (_dictConstantsGottenDate != null && _dictConstantsGottenDate.ContainsKey(constantName) && ((nowDate - _dictConstantsGottenDate[constantName]) <= 172800))
                    return false;
            }
            catch { }
            return true;
        }

        private async void LoadConstantsGottenDate()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("constantsgottendate");
                if (!string.IsNullOrEmpty(json))
                {
                    _dictConstantsGottenDate = JsonConvert.DeserializeObject<Dictionary<string, long>>(json);
                }
            }
            catch { }
            try
            {
                if (_dictConstantsGottenDate == null)
                {
                    _dictConstantsGottenDate = new Dictionary<string, long>();
                }
            }
            catch { }
        }

        private async void SaveConstantsGottenDate()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_dictConstantsGottenDate);
                if (!string.IsNullOrEmpty(json))
                {
                    await StorageFilesCourier.WriteFileAsync("constantsgottendate", json);
                }
            }
            catch { }
        }

        public void ResetConstantsGottenDate()
        {
            try
            {
                _dictConstantsGottenDate?.Clear();
                SaveConstantsGottenDate();
            }
            catch { }
        }
    }
}
