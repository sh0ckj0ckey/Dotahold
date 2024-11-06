using Dotahold.Core.DataShop;
using Dotahold.Core.Models;
using Dotahold.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
        // 用户信息
        private DotaMatchPlayerProfileModel _PlayerProfile = null;
        public DotaMatchPlayerProfileModel PlayerProfile
        {
            get { return _PlayerProfile; }
            set { Set("PlayerProfile", ref _PlayerProfile, value); }
        }

        // 用户ID
        private string _sSteamId = string.Empty;
        public string sSteamId
        {
            get { return _sSteamId; }
            set { Set("sSteamId", ref _sSteamId, value); }
        }

        // 绑定过的账号列表
        public ObservableCollection<DotaIdBindHistoryModel> vDotaIdHistory = new ObservableCollection<DotaIdBindHistoryModel>();

        /// <summary>
        /// 加载用户的个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async void GetPlayerProfileAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetPlayerProfile ---> " + DateTime.Now.Ticks);

                PlayerProfile = null;

                string url = string.Format("https://api.opendota.com/api/players/{0}", id); // http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}
                DotaMatchPlayerProfileModel profile = null;

                try
                {
                    profile = await GetResponseAsync<DotaMatchPlayerProfileModel>(url, _playerInfoHttpClient);
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                if (profile != null)
                {
                    if (profile.leaderboard_rank is int rank && rank > 0 && profile.rank_tier >= 80)
                    {
                        if (rank == 1)
                        {
                            profile.rank_tier = 84;
                        }
                        else if (rank <= 10)
                        {
                            profile.rank_tier = 83;
                        }
                        else if (rank <= 100)
                        {
                            profile.rank_tier = 82;
                        }
                        else if (rank <= 1000)
                        {
                            profile.rank_tier = 81;
                        }
                        else
                        {
                            profile.rank_tier = 80;
                        }
                    }
                }
                PlayerProfile = profile;

                if (PlayerProfile?.profile != null)
                {
                    await PlayerProfile?.profile?.LoadAvatarAsync(72);
                }

                if (PlayerProfile?.profile != null)
                {
                    var prof = PlayerProfile?.profile;
                    string steamId = this.sSteamId;
                    AddDotaIdHistory(prof.personaname, prof.avatarfull, steamId);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 绑定保存用户的SteamID
        /// </summary>
        /// <param name="input"></param>
        public void SetSteamID(string steamId)
        {
            try
            {
                // 我的Steam64位ID:76561198194624815
                if (steamId.Length > 14)
                {
                    // 说明输入的是64位的,要先转换成32位
                    decimal id64 = Convert.ToDecimal(steamId);
                    steamId = (id64 - 76561197960265728).ToString();
                }
                DotaViewModel.Instance.AppSettings.sSteamID = steamId;
                sSteamId = steamId;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 添加一条账号绑定记录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="img"></param>
        /// <param name="id"></param>
        private async void AddDotaIdHistory(string name, string img, string id)
        {
            try
            {
                DotaIdBindHistoryModel removing = null;
                foreach (var item in vDotaIdHistory)
                {
                    if (item.SteamId == id)
                    {
                        removing = item;
                        break;
                    }
                }
                if (removing != null && vDotaIdHistory.Contains(removing))
                {
                    vDotaIdHistory.Remove(removing);
                }

                while (vDotaIdHistory.Count > 2)
                {
                    vDotaIdHistory.RemoveAt(vDotaIdHistory.Count - 1);
                }
                DotaIdBindHistoryModel his = new DotaIdBindHistoryModel();
                his.PlayerName = name;
                his.SteamId = id;
                his.AvatarImage = img;
                vDotaIdHistory.Insert(0, his);

                SaveBindedDotaIdHistory();

                await his.LoadImageAsync(56);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private async void LoadBindedDotaIdHistory()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("dotaidbindhistory");
                var list = JsonConvert.DeserializeObject<ObservableCollection<DotaIdBindHistoryModel>>(json);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (item == null || string.IsNullOrEmpty(item.SteamId)) continue;
                        vDotaIdHistory.Add(item);
                    }

                    foreach (var item in vDotaIdHistory)
                    {
                        await item.LoadImageAsync(56);
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private async void SaveBindedDotaIdHistory()
        {
            try
            {
                var json = JsonConvert.SerializeObject(vDotaIdHistory);
                if (!string.IsNullOrEmpty(json))
                {
                    await StorageFilesCourier.WriteFileAsync("dotaidbindhistory", json);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

    }
}
