using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dotahold.Data.Models;
using Windows.Storage;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public static class ConstantsCourier
    {
        private static readonly Windows.Web.Http.HttpClient _constantsHttpClient = new();

        private static Dictionary<string, long>? _dictConstantsGottenDate = null;

        private const string _heroesJsonFileName = "heroes_json";

        private const string _itemsJsonFileName = "items_json";

        private const string _abilitiesJsonFileName = "abilities_json";

        private const string _buffsJsonFileName = "permanent_buffs_json";

        private const string _abilityIdsJsonFileName = "ability_ids_json";

        /// <summary>
        /// 英雄字典
        /// </summary>
        private static Dictionary<string, DotaHeroModel>? _heroes = null;

        /// <summary>
        /// 物品字典
        /// </summary>
        private static Dictionary<string, DotaItemModel>? _items = null;

        /// <summary>
        /// 技能字典
        /// </summary>
        private static Dictionary<string, DotaAibilitiesModel>? _abilities = null;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private static Dictionary<string, string>? _permanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private static Dictionary<string, string>? _abilityIds = null;

        /// <summary>
        /// 英雄和物品图片地址的域名
        /// </summary>
        public static string ImageSourceDomain { get; set; } = "https://cdn.akamai.steamstatic.com";

        static ConstantsCourier()
        {
            LoadConstantsGottenDate();
        }

        /// <summary>
        /// 获取英雄列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, DotaHeroModel>> GetHeroesConstant()
        {
            if (_heroes is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_heroesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _heroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_heroes is null)
            {
                try
                {
                    StorageFolder rootFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder dataFolder = await rootFolder.GetFolderAsync("Dotahold.Data");
                    StorageFolder dataShopFolder = await dataFolder.GetFolderAsync("DataShop");
                    StorageFolder constantsJsonsFolder = await dataShopFolder.GetFolderAsync("ConstantsJsons");
                    var json = await StorageFilesCourier.ReadFileAsync("heroes.json", constantsJsonsFolder);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _heroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_heroes is null || CheckNeed2UpdateJson("heroes"))
            {
                try
                {
                    if (_heroes is null)
                    {
                        var json = await GetConstant("heroes", _heroesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _heroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("heroes", _heroesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _heroes ??= [];

            return _heroes;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, DotaItemModel>> GetItemsConstant()
        {
            if (_items is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_itemsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _items = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_items is null)
            {
                try
                {
                    StorageFolder rootFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder dataFolder = await rootFolder.GetFolderAsync("Dotahold.Data");
                    StorageFolder dataShopFolder = await dataFolder.GetFolderAsync("DataShop");
                    StorageFolder constantsJsonsFolder = await dataShopFolder.GetFolderAsync("ConstantsJsons");
                    var json = await StorageFilesCourier.ReadFileAsync("items.json", constantsJsonsFolder);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _items = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_items is null || CheckNeed2UpdateJson("items"))
            {
                try
                {
                    if (_items is null)
                    {
                        var json = await GetConstant("items", _itemsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _items = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("items", _itemsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _items ??= [];

            return _items;
        }

        /// <summary>
        /// 获取技能列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, DotaAibilitiesModel>> GetAbilitiesConstant()
        {
            if (_abilities is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_abilitiesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _abilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_abilities is null)
            {
                try
                {
                    StorageFolder rootFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder dataFolder = await rootFolder.GetFolderAsync("Dotahold.Data");
                    StorageFolder dataShopFolder = await dataFolder.GetFolderAsync("DataShop");
                    StorageFolder constantsJsonsFolder = await dataShopFolder.GetFolderAsync("ConstantsJsons");
                    var json = await StorageFilesCourier.ReadFileAsync("hero_abilities.json", constantsJsonsFolder);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _abilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_abilities is null || CheckNeed2UpdateJson("hero_abilities"))
            {
                try
                {
                    if (_abilities is null)
                    {
                        var json = await GetConstant("hero_abilities", _abilitiesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _abilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("hero_abilities", _abilitiesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _abilities ??= [];

            return _abilities;
        }

        /// <summary>
        /// 获取永久buff列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetPermanentBuffsConstant()
        {
            if (_permanentBuffs is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_buffsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _permanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_permanentBuffs is null)
            {
                try
                {
                    StorageFolder rootFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder dataFolder = await rootFolder.GetFolderAsync("Dotahold.Data");
                    StorageFolder dataShopFolder = await dataFolder.GetFolderAsync("DataShop");
                    StorageFolder constantsJsonsFolder = await dataShopFolder.GetFolderAsync("ConstantsJsons");
                    var json = await StorageFilesCourier.ReadFileAsync("permanent_buffs.json", constantsJsonsFolder);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _permanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_permanentBuffs is null || CheckNeed2UpdateJson("permanent_buffs"))
            {
                try
                {
                    if (_permanentBuffs is null)
                    {
                        var json = await GetConstant("permanent_buffs", _buffsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _permanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = GetConstant("permanent_buffs", _buffsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _permanentBuffs ??= [];

            return _permanentBuffs;
        }

        /// <summary>
        /// 获取技能ID列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetAbilityIDsConstant()
        {
            if (_abilityIds is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_abilityIdsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _abilityIds = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_abilityIds is null)
            {
                try
                {
                    StorageFolder rootFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFolder dataFolder = await rootFolder.GetFolderAsync("Dotahold.Data");
                    StorageFolder dataShopFolder = await dataFolder.GetFolderAsync("DataShop");
                    StorageFolder constantsJsonsFolder = await dataShopFolder.GetFolderAsync("ConstantsJsons");
                    var json = await StorageFilesCourier.ReadFileAsync("ability_ids.json", constantsJsonsFolder);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _abilityIds = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_abilityIds is null || CheckNeed2UpdateJson("ability_ids"))
            {
                try
                {
                    if (_abilityIds is null)
                    {
                        var json = await GetConstant("ability_ids", _abilityIdsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _abilityIds = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = GetConstant("ability_ids", _abilityIdsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _abilityIds ??= [];

            return _abilityIds;
        }

        /// <summary>
        /// 下载指定的Constant json文件，存储到本地，然后返回
        /// </summary>
        /// <returns>下载的json</returns>
        private static async Task<string> GetConstant(string constantName, string jsonFileName)
        {
            string constantJson = string.Empty;
            string url = "https://api.opendota.com/api/constants/" + constantName;

            try
            {
                var response = await _constantsHttpClient.GetAsync(new Uri(url));
                constantJson = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(constantJson) && constantJson.Length > 256/*太短的话认为是请求失败*/)
                {
                    bool written = await StorageFilesCourier.WriteFileAsync(jsonFileName, constantJson);

                    if (written && _dictConstantsGottenDate is not null)
                    {
                        _dictConstantsGottenDate[constantName] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        SaveConstantsGottenDate();
                    }
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }

            return constantJson;
        }

        private static bool CheckNeed2UpdateJson(string constantName)
        {
            try
            {
                long nowDateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                if (_dictConstantsGottenDate?.ContainsKey(constantName) == true &&
                    nowDateTime - _dictConstantsGottenDate[constantName] <= 172800)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return true;
        }

        private static void LoadConstantsGottenDate()
        {
            try
            {
                //string json = await StorageFilesCourier.ReadFileAsync("constantsgottendate");
                string json = ApplicationData.Current.LocalSettings.Values["constants_update_date"] as string ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _dictConstantsGottenDate = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringInt64);
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }

            _dictConstantsGottenDate ??= [];
        }

        private static void SaveConstantsGottenDate()
        {
            try
            {
                var json = JsonSerializer.Serialize(_dictConstantsGottenDate, SourceGenerationContext.Default.DictionaryStringInt64);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    //await StorageFilesCourier.WriteFileAsync("constantsgottendate", json);
                    ApplicationData.Current.LocalSettings.Values["constants_update_date"] = json;
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        public static void ResetConstantsGottenDate()
        {
            try
            {
                _dictConstantsGottenDate?.Clear();
                SaveConstantsGottenDate();
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
