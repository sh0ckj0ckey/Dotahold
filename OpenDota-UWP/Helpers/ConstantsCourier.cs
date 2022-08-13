using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的"常量数据"
    /// </summary>
    public class ConstantsCourier
    {
        private static Lazy<ConstantsCourier> _lazyVM = new Lazy<ConstantsCourier>(() => new ConstantsCourier());
        public static ConstantsCourier Instance => _lazyVM.Value;

        private Windows.Web.Http.HttpClient constantsHttpClient = new Windows.Web.Http.HttpClient();

        /// <summary>
        /// 获取英雄列表
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Models.DotaHeroModel>> GetHeroesConstant()
        {
            try
            {
                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var heroes = await DownloadConstant<Dictionary<string, Models.DotaHeroModel>>("heroes");
                    return heroes;
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
                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var buffs = await DownloadConstant<Dictionary<string, string>>("permanent_buffs");
                    return buffs;
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
                if (ViewModels.DotaViewModel.Instance.bForceApiRequest || (true && !ViewModels.DotaViewModel.Instance.bDisableApiRequest))
                {
                    // if Time > 24h, then download new file
                    var abilities = await DownloadConstant<Dictionary<string, string>>("ability_ids");
                    return abilities;
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
                var response = await constantsHttpClient.GetAsync(new Uri(url));
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
    }
}
