using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public static class ConstantsCourier
    {
        private static readonly Windows.Web.Http.HttpClient _constantsHttpClient = new();

        private static Dictionary<string, long>? _dictConstantsGottenDate = null;

        private const string _heroesJsonFileName = "heroesjson";

        private const string _itemsJsonFileName = "itemsjson";

        private const string _buffsJsonFileName = "permanentbuffsjson";

        private const string _abilitiesJsonFileName = "abilitiesjson";

        private const string _itemColorsJsonFileName = "itemcolorsjson";

        /// <summary>
        /// 英雄字典
        /// </summary>
        private static Dictionary<string, Models.DotaHeroModel>? _dictHeroes = null;

        /// <summary>
        /// 物品字典
        /// </summary>
        private static Dictionary<string, Models.DotaItemModel>? _dictItems = null;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private static Dictionary<string, string>? _dictPermanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private static Dictionary<string, string>? _dictAbilitiesId = null;

        /// <summary>
        /// 物品种类与颜色字典
        /// </summary>
        private static Dictionary<string, string>? _dictItemColors = null;

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
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictHeroes is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\heroes.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictHeroes = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaHeroModel);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                        _ = await GetConstant("heroes", _heroesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            _dictHeroes ??= new Dictionary<string, DotaHeroModel>();

            return _dictHeroes;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Models.DotaItemModel>> GetItemsConstant()
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
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictItems is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\items.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItems = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringDotaItemModel);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                        _ = await GetConstant("items", _itemsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            _dictItems ??= new Dictionary<string, Models.DotaItemModel>();

            return _dictItems;
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
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictPermanentBuffs is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\permanent_buffs.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictPermanentBuffs = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                        _ = await GetConstant("permanent_buffs", _buffsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            _dictPermanentBuffs ??= new Dictionary<string, string>();

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
                    var json = await StorageFilesCourier.ReadFileAsync(_abilitiesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilitiesId is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\ability_ids.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictAbilitiesId is null || CheckNeed2UpdateJson("ability_ids"))
            {
                try
                {
                    if (_dictAbilitiesId is null)
                    {
                        var json = await GetConstant("ability_ids", _abilitiesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictAbilitiesId = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("ability_ids", _abilitiesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            _dictAbilitiesId ??= new Dictionary<string, string>();

            return _dictAbilitiesId;
        }

        /// <summary>
        /// 获取物品分类和颜色列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetItemColorsConstant()
        {
            if (_dictItemColors is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_itemColorsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItemColors = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictItemColors is null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\item_colors.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItemColors = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            if (_dictItemColors is null || CheckNeed2UpdateJson("item_colors"))
            {
                try
                {
                    if (_dictItemColors is null)
                    {
                        var json = await GetConstant("item_colors", _itemColorsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictItemColors = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringString);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("item_colors", _itemColorsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            _dictItemColors ??= new Dictionary<string, string>();

            return _dictItemColors;
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }

            return true;
        }

        private static async void LoadConstantsGottenDate()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("constantsgottendate");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _dictConstantsGottenDate = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DictionaryStringInt64);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

            _dictConstantsGottenDate ??= new Dictionary<string, long>();
        }

        private static async void SaveConstantsGottenDate()
        {
            try
            {
                var json = JsonSerializer.Serialize(_dictConstantsGottenDate, SourceGenerationContext.Default.DictionaryStringInt64);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    await StorageFilesCourier.WriteFileAsync("constantsgottendate", json);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        public static void ResetConstantsGottenDate()
        {
            try
            {
                _dictConstantsGottenDate?.Clear();
                SaveConstantsGottenDate();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
