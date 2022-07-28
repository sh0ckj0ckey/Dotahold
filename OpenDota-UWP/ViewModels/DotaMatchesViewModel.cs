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

        // 用户ID
        private string _sSteamId = string.Empty;
        public string sSteamId
        {
            get { return _sSteamId; }
            set { Set("sSteamId", ref _sSteamId, value); }
        }

        // 缓存玩家名字和头像
        private Dictionary<string, string> dictPlayersNameCache = new Dictionary<string, string>();
        private Dictionary<string, string> dictPlayersPhotoCache = new Dictionary<string, string>();

        private Windows.Web.Http.HttpClient playerInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient matchHttpClient = new Windows.Web.Http.HttpClient();

        // 用户信息
        private DotaMatchPlayerProfileModel _PlayerProfile = null;
        public DotaMatchPlayerProfileModel PlayerProfile
        {
            get { return _PlayerProfile; }
            set { Set("PlayerProfile", ref _PlayerProfile, value); }
        }

        // 用户胜负场数
        private DotaMatchWinLoseModel _PlayerWinLose = null;
        public DotaMatchWinLoseModel PlayerWinLose
        {
            get { return _PlayerWinLose; }
            set { Set("PlayerWinLose", ref _PlayerWinLose, value); }
        }

        // 用户统计数据
        private List<DotaMatchPlayerTotalModel> _PlayerTotals = null;
        public List<DotaMatchPlayerTotalModel> PlayerTotals
        {
            get { return _PlayerTotals; }
            set { Set("PlayerTotals", ref _PlayerTotals, value); }
        }

        // 在线玩家数量
        private string _sOnlilnePlayersCount = string.Empty;
        public string sOnlilnePlayersCount
        {
            get { return _sOnlilnePlayersCount; }
            set { Set("sOnlilnePlayersCount", ref _sOnlilnePlayersCount, value); }
        }

        // 最近的5场比赛
        public ObservableCollection<DotaRecentMatchModel> vRecentMatchesForFlip = new ObservableCollection<DotaRecentMatchModel>();

        // 最近的比赛
        public ObservableCollection<DotaRecentMatchModel> vRecentMatches = new ObservableCollection<DotaRecentMatchModel>();

        // 最常用的10个英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayed10Heroes = new ObservableCollection<DotaMatchHeroPlayedModel>();

        // 所有的最常用英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayedHeroes = new ObservableCollection<DotaMatchHeroPlayedModel>();


        // 刷新胜负场次的饼状图
        public Action<double, double> ActUpdatePieChart = null;

        public DotaMatchesViewModel()
        {
            InitialDotaMatches();
        }

        public async void InitialDotaMatches()
        {
            try
            {
                sSteamId = GetSteamID();

                if (!string.IsNullOrEmpty(sSteamId))
                {
                    // 玩家信息
                    var profile = await GetPlayerProfileAsync(sSteamId);
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

                    // 胜率
                    var wl = await GetPlayerWLAsync(sSteamId);
                    if (wl != null && (wl.win + wl.lose) > 0)
                    {
                        double rate = wl.win / (wl.win + wl.lose);
                        wl.winRate = (Math.Floor(10000 * rate) / 100).ToString() + "%";
                    }
                    PlayerWinLose = wl;
                    if (PlayerWinLose != null)
                        ActUpdatePieChart?.Invoke(PlayerWinLose.win, PlayerWinLose.lose);

                    // 统计数据
                    var total = await GetTotalAsync(sSteamId);
                    PlayerTotals = total;

                    // 处理最近的比赛
                    var recentMatches = await GetRecentMatchAsync(sSteamId);
                    vRecentMatchesForFlip.Clear();
                    vRecentMatches.Clear();
                    if (recentMatches != null)
                    {
                        foreach (var item in recentMatches)
                        {
                            if (DotaHeroesViewModel.Instance.dictAllHeroes.ContainsKey(item.hero_id.ToString()))
                            {
                                item.sHeroCoverImage = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/crops/{0}.png",
                                    DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].name.Replace("npc_dota_hero_", ""));
                                item.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].localized_name;

                                item.bWin = null;
                                if (item.player_slot != null && item.radiant_win != null)
                                {
                                    if (item.player_slot < 128)         // 天辉
                                        item.bWin = item.radiant_win;
                                    else if (item.player_slot >= 128)   // 夜魇
                                        item.bWin = !item.radiant_win;
                                }

                                vRecentMatches.Add(item);
                                if (vRecentMatchesForFlip.Count < 5)
                                    vRecentMatchesForFlip.Add(item);
                            }
                        }
                    }

                    // 常用英雄
                    var heroes = await GetHeroesPlayedAsync(sSteamId);
                    vMostPlayed10Heroes.Clear();
                    vMostPlayedHeroes.Clear();
                    if (heroes != null)
                    {
                        foreach (var item in heroes)
                        {
                            if (DotaHeroesViewModel.Instance.dictAllHeroes.ContainsKey(item.hero_id.ToString()))
                            {
                                item.sHeroCoverImage = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/icons/{0}.png",
                                    DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].name.Replace("npc_dota_hero_", ""));
                                item.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].localized_name;

                                double rate = 0;
                                if (item.games > 0)
                                    rate = (item.win ?? 0) / (item.games ?? 1);
                                else
                                    rate = 1;
                                item.sWinRate = (Math.Floor(1000 * rate) / 10).ToString() + "%";

                                vMostPlayedHeroes.Add(item);
                                if (vMostPlayed10Heroes.Count < 10)
                                    vMostPlayed10Heroes.Add(item);
                            }
                        }
                    }
                }

                sOnlilnePlayersCount = await GetNumberOfCurrentPlayersAsync();
            }
            catch { }
        }

        /// <summary>
        /// 获得用户的个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<DotaMatchPlayerProfileModel> GetPlayerProfileAsync(string id)
        {
            try
            {
                string url = string.Format("https://api.opendota.com/api/players/{0}", id); //http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var player = JsonConvert.DeserializeObject<DotaMatchPlayerProfileModel>(jsonMessage);
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
                string url = string.Format("https://api.opendota.com/api/players/{0}/wl", id);

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var wl = JsonConvert.DeserializeObject<DotaMatchWinLoseModel>(jsonMessage);
                return wl;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取比赛总数据统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<DotaMatchPlayerTotalModel>> GetTotalAsync(string id)
        {
            try
            {
                string url = string.Format("https://api.opendota.com/api/players/{0}/totals", id);

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var totals = JsonConvert.DeserializeObject<List<DotaMatchPlayerTotalModel>>(jsonMessage);

                if (totals != null && totals.Count > 0)
                {

                }
                //KDA
                //total[3] = ((Convert.ToDouble(killsMatch.Groups[1].Value) + Convert.ToDouble(assistsMatch.Groups[1].Value)) / (Convert.ToDouble(deadMatch.Groups[1].Value))).ToString("f2");

                return totals;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取最近20场比赛
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<DotaRecentMatchModel>> GetRecentMatchAsync(string id)
        {
            try
            {
                string url = String.Format("https://api.opendota.com/api/players/{0}/recentMatches", id);

                var response = await matchHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var matches = JsonConvert.DeserializeObject<List<DotaRecentMatchModel>>(jsonMessage);
                return matches;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取玩家常用英雄数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<DotaMatchHeroPlayedModel>> GetHeroesPlayedAsync(string id)
        {
            try
            {
                string url = string.Format("https://api.opendota.com/api/players/{0}/heroes", id);

                var response = await matchHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var heroes = JsonConvert.DeserializeObject<List<DotaMatchHeroPlayedModel>>(jsonMessage);

                return heroes;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取当前在线人数
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetNumberOfCurrentPlayersAsync()
        {
            try
            {
                string url = "http://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1?appid=570&format=json";

                var response = await playerInfoHttpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();

                var online = JsonConvert.DeserializeObject<DotaOnlinePlayersModel>(jsonMessage);

                if (online?.response?.result == 1)
                {
                    return online.response.player_count.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 请求更新数据
        /// </summary>
        /// <param name="id"></param>
        public async void PostRefreshAsync(string id)
        {
            try
            {
                string url = String.Format("https://api.opendota.com/api/players/{0}/refresh", id);
                await playerInfoHttpClient.PostAsync(new Uri(url), null);
            }
            catch { }
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
                App.AppSettingContainer.Values["SteamID"] = steamId;
                sSteamId = steamId;
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
                if (App.AppSettingContainer?.Values["SteamID"] != null)
                {
                    return App.AppSettingContainer?.Values["SteamID"].ToString();
                }
            }
            catch { }
            return string.Empty;
        }

    }
}
