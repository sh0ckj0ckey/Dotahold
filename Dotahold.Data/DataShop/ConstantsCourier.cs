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
        private static Dictionary<string, DotaHeroModel>? _dictHeroes = null;

        /// <summary>
        /// 物品字典
        /// </summary>
        private static Dictionary<string, DotaItemModel>? _dictItems = null;

        /// <summary>
        /// 技能字典
        /// </summary>
        private static Dictionary<string, DotaAibilitiesModel>? _dictAbilities = null;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private static Dictionary<string, string>? _dictPermanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private static Dictionary<string, string>? _dictAbilitiesId = null;

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
            if (_dictHeroes is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_heroesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictHeroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictHeroes is null)
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
                        _dictHeroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictHeroes is null || CheckNeed2UpdateJson("heroes"))
            {
                try
                {
                    if (_dictHeroes is null)
                    {
                        var json = await GetConstant("heroes", _heroesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictHeroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("heroes", _heroesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _dictHeroes ??= [];

            return _dictHeroes;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, DotaItemModel>> GetItemsConstant()
        {
            if (_dictItems is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_itemsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItems = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictItems is null)
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
                        _dictItems = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictItems is null || CheckNeed2UpdateJson("items"))
            {
                try
                {
                    if (_dictItems is null)
                    {
                        var json = await GetConstant("items", _itemsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictItems = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("items", _itemsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _dictItems ??= [];

            return _dictItems;
        }

        /// <summary>
        /// 获取技能列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, DotaAibilitiesModel>> GetAbilitiesConstant()
        {
            if (_dictAbilities is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_abilitiesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilities is null)
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
                        _dictAbilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilities is null || CheckNeed2UpdateJson("hero_abilities"))
            {
                try
                {
                    if (_dictAbilities is null)
                    {
                        var json = await GetConstant("hero_abilities", _abilitiesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictAbilities = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaAibilitiesModel);
                        }
                    }
                    else
                    {
                        _ = GetConstant("hero_abilities", _abilitiesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _dictAbilities ??= [];

            return _dictAbilities;
        }

        /// <summary>
        /// 获取永久buff列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetPermanentBuffsConstant()
        {
            if (_dictPermanentBuffs is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_buffsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictPermanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictPermanentBuffs is null)
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
                        _dictPermanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictPermanentBuffs is null || CheckNeed2UpdateJson("permanent_buffs"))
            {
                try
                {
                    if (_dictPermanentBuffs is null)
                    {
                        var json = await GetConstant("permanent_buffs", _buffsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictPermanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = GetConstant("permanent_buffs", _buffsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _dictPermanentBuffs ??= [];

            return _dictPermanentBuffs;
        }

        /// <summary>
        /// 获取技能ID列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetAbilityIDsConstant()
        {
            if (_dictAbilitiesId is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_abilityIdsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilitiesId is null)
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
                        _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilitiesId is null || CheckNeed2UpdateJson("ability_ids"))
            {
                try
                {
                    if (_dictAbilitiesId is null)
                    {
                        var json = await GetConstant("ability_ids", _abilityIdsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = GetConstant("ability_ids", _abilityIdsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            }

            _dictAbilitiesId ??= [];

            return _dictAbilitiesId;
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
