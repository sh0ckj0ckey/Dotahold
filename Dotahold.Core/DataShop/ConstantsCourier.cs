using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public class ConstantsCourier
    {
        public static BitmapImage DefaultHeroImageSource72 = null;

        public static BitmapImage DefaultItemImageSource72 = null;

        public static BitmapImage DefaultAvatarImageSource72 = null;

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
        private Dictionary<string, Core.Models.DotaHeroModel> _dictHeroes = null;

        /// <summary>
        /// 物品字典
        /// </summary>
        private Dictionary<string, Core.Models.DotaItemModel> _dictItems = null;

        /// <summary>
        /// 永久buff字典
        /// </summary>
        private Dictionary<string, string> _dictPermanentBuffs = null;

        /// <summary>
        /// 技能ID与名称字典
        /// </summary>
        private Dictionary<string, string> _dictAbilitiesId = null;

        public ConstantsCourier()
        {
            try
            {
                LoadConstantsGottenDate();

                DefaultHeroImageSource72 = new BitmapImage(new Uri("ms-appx:///Assets/Icons/item_placeholder.png"))
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelHeight = 144
                };

                DefaultItemImageSource72 = new BitmapImage(new Uri("ms-appx:///Assets/Icons/item_placeholder.png"))
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelHeight = 144
                };

                DefaultAvatarImageSource72 = new BitmapImage(new Uri("ms-appx:///Assets/Icons/avatar_placeholder.jpeg"))
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelHeight = 144
                };
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 获取英雄列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Core.Models.DotaHeroModel>> GetHeroesConstant()
        {
            // 先去本地的Local文件夹找下载的最新的
            if (_dictHeroes == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_heroesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictHeroes = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaHeroModel>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 找不到就用内置的
            if (_dictHeroes == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\heroes.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictHeroes = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaHeroModel>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 内置的也找不到(不太可能)
            if (_dictHeroes == null || CheckNeed2UpdateJson("heroes"))
            {
                try
                {
                    if (_dictHeroes == null)
                    {
                        var json = await GetConstant("heroes", _heroesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictHeroes = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaHeroModel>>(json);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("heroes", _heroesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            return _dictHeroes;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Core.Models.DotaItemModel>> GetItemsConstant()
        {
            // 先去本地的Local文件夹找下载的最新的
            if (_dictItems == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_itemsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItems = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaItemModel>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 找不到就用内置的
            if (_dictItems == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\items.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictItems = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaItemModel>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 内置的也找不到(不太可能)
            if (_dictItems == null || CheckNeed2UpdateJson("items"))
            {
                try
                {
                    if (_dictItems == null)
                    {
                        var json = await GetConstant("items", _itemsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictItems = JsonSerializer.Deserialize<Dictionary<string, Core.Models.DotaItemModel>>(json);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("items", _itemsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            return _dictItems;
        }

        /// <summary>
        /// 获取永久buff列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetPermanentBuffsConstant()
        {
            // 先去本地的Local文件夹找下载的最新的
            if (_dictPermanentBuffs == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_buffsJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictPermanentBuffs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 找不到就用内置的
            if (_dictPermanentBuffs == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\permanent_buffs.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictPermanentBuffs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 内置的也找不到(不太可能)
            if (_dictPermanentBuffs == null || CheckNeed2UpdateJson("permanent_buffs"))
            {
                try
                {
                    if (_dictPermanentBuffs == null)
                    {
                        var json = await GetConstant("permanent_buffs", _buffsJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictPermanentBuffs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("permanent_buffs", _buffsJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            return _dictPermanentBuffs;
        }

        /// <summary>
        /// 获取技能ID列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAbilityIDsConstant()
        {
            // 先去本地的Local文件夹找下载的最新的
            if (_dictAbilitiesId == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(_abilitiesJsonFileName);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilitiesId = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 找不到就用内置的
            if (_dictAbilitiesId == null)
            {
                try
                {
                    var json = await StorageFilesCourier.ReadFileAsync(@"\ConstantsJsons\ability_ids.json", Windows.ApplicationModel.Package.Current.InstalledLocation);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _dictAbilitiesId = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            // 内置的也找不到(不太可能)
            if (_dictAbilitiesId == null || CheckNeed2UpdateJson("ability_ids"))
            {
                try
                {
                    if (_dictAbilitiesId == null)
                    {
                        var json = await GetConstant("ability_ids", _abilitiesJsonFileName);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            _dictAbilitiesId = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                        }
                    }
                    else
                    {
                        _ = await GetConstant("ability_ids", _abilitiesJsonFileName);
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            }

            return _dictAbilitiesId;
        }

        /// <summary>
        /// 下载指定的Constant json文件，存储到本地，然后返回
        /// </summary>
        /// <returns>下载的json</returns>
        private async Task<string> GetConstant(string constantName, string jsonFileName)
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

                    if (written)
                    {
                        _dictConstantsGottenDate[constantName] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        SaveConstantsGottenDate();
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

            return constantJson;
        }


        private bool CheckNeed2UpdateJson(string constantName)
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

        private async void LoadConstantsGottenDate()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("constantsgottendate");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _dictConstantsGottenDate = JsonSerializer.Deserialize<Dictionary<string, long>>(json);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

            try
            {
                if (_dictConstantsGottenDate is null)
                {
                    _dictConstantsGottenDate = new Dictionary<string, long>();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private async void SaveConstantsGottenDate()
        {
            try
            {
                var json = JsonSerializer.Serialize(_dictConstantsGottenDate);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    await StorageFilesCourier.WriteFileAsync("constantsgottendate", json);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        public void ResetConstantsGottenDate()
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
