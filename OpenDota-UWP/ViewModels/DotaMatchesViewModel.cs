using Newtonsoft.Json;
using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OpenDota_UWP.ViewModels
{
    public class DotaMatchesViewModel : ViewModelBase
    {
        private static Lazy<DotaMatchesViewModel> _lazyVM = new Lazy<DotaMatchesViewModel>(() => new DotaMatchesViewModel());
        public static DotaMatchesViewModel Instance => _lazyVM.Value;

        //用于保存用户的Steam64位ID，以"账号绑定"的形式
        public ApplicationDataContainer DotaSettings = ApplicationData.Current.LocalSettings;

        // 缓存玩家名字和头像
        private Dictionary<string, string> dictPlayersNameCache = new Dictionary<string, string>();
        private Dictionary<string, string> dictPlayersPhotoCache = new Dictionary<string, string>();

        private Windows.Web.Http.HttpClient playerInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient matchHttpClient = new Windows.Web.Http.HttpClient();

        public DotaMatchesViewModel()
        {
            InitialDotaMatches();
        }

        private async void InitialDotaMatches()
        {
            try
            {

            }
            catch { }
        }


        /// <summary>
        /// 获得用户的个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PlayerProfile> GetPlayerProfileAsync(string id)
        {
            try
            {
                string url = String.Format("https://api.opendota.com/api/players/{0}", id); //http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var player = JsonConvert.DeserializeObject<PlayerProfile>(jsonMessage);
                return player;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获得用户的胜局败局数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<DotaMatchWinLoseModel> GetPlayerWLAsync(string id)
        {
            try
            {
                string url = String.Format("https://api.opendota.com/api/players/{0}/wl", id);

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var wl = JsonConvert.DeserializeObject<DotaMatchWinLoseModel>(jsonMessage);
                return wl;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 绑定保存用户的SteamID
        /// </summary>
        /// <param name="input"></param>
        public void SetSteamID(string input)
        {
            try
            {
                //我的Steam64位ID:76561198194624815
                if (input.Length > 14)
                {
                    //这说明输入的是64位的,要先转换成32位
                    input = ConvertSteamID(Convert.ToDecimal(input));
                }
                DotaSettings.Values["SteamID"] = input;
            }
            catch { }
        }

        /// <summary>
        /// 读取保存的用户的SteamID
        /// </summary>
        /// <returns></returns>
        public string GetSteamID()
        {
            try
            {
                if (DotaSettings.Values["SteamID"] != null)
                {
                    return DotaSettings.Values["SteamID"].ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 根据Steam64位ID获得32位ID
        /// </summary>
        /// <param name="id_64"></param>
        /// <returns></returns>
        private string ConvertSteamID(decimal id_64)
        {
            try
            {
                return (id_64 - 76561197960265728).ToString();
            }
            catch { }
            return id_64.ToString();
        }
    }
}
