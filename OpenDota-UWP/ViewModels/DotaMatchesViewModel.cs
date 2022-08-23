using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json;
using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using SkiaSharp;
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

        // 绑定过的账号列表
        public ObservableCollection<DotaIdBindHistoryModel> vDotaIdHistory = new ObservableCollection<DotaIdBindHistoryModel>();

        private Windows.Web.Http.HttpClient playerInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient matchHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient matchInfoHttpClient = new Windows.Web.Http.HttpClient();

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

        // 所有的比赛
        private List<DotaRecentMatchModel> vAllMatchesList = new List<DotaRecentMatchModel>();
        public ObservableCollection<DotaRecentMatchModel> vAllMatches = new ObservableCollection<DotaRecentMatchModel>();

        // 最常用的10个英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayed10Heroes = new ObservableCollection<DotaMatchHeroPlayedModel>();

        // 所有的最常用英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayedHeroes = new ObservableCollection<DotaMatchHeroPlayedModel>();

        // 玩家的统计数据
        public ObservableCollection<DotaMatchPlayerTotalModel> vPlayerTotals = new ObservableCollection<DotaMatchPlayerTotalModel>();

        // 是否正在加载常用英雄
        private bool _bLoadingPlayed = false;
        public bool bLoadingPlayed
        {
            get { return _bLoadingPlayed; }
            set { Set("bLoadingPlayed", ref _bLoadingPlayed, value); }
        }
        // 是否正在加载所有比赛
        private bool _bLoadingAllMatches = false;
        public bool bLoadingAllMatches
        {
            get { return _bLoadingAllMatches; }
            set { Set("bLoadingAllMatches", ref _bLoadingAllMatches, value); }
        }
        // 是否已经加载完所有的比赛
        private bool _bLoadedAllMatches = false;
        public bool bLoadedAllMatches
        {
            get { return _bLoadedAllMatches; }
            set { Set("bLoadedAllMatches", ref _bLoadedAllMatches, value); }
        }

        // 是否正在加载指定比赛
        private bool _bLoadingOneMatchInfo = false;
        public bool bLoadingOneMatchInfo
        {
            get { return _bLoadingOneMatchInfo; }
            set { Set("bLoadingOneMatchInfo", ref _bLoadingOneMatchInfo, value); }
        }

        // 是否正在加载英雄和物品
        private bool _bLoadingHeroesAndItems = false;
        public bool bLoadingHeroesAndItems
        {
            get { return _bLoadingHeroesAndItems; }
            set { Set("bLoadingHeroesAndItems", ref _bLoadingHeroesAndItems, value); }
        }

        // 是否正在搜索比赛编号
        private bool _bSearchingByMatchId = false;
        public bool bSearchingByMatchId
        {
            get { return _bSearchingByMatchId; }
            set { Set("bSearchingByMatchId", ref _bSearchingByMatchId, value); }
        }

        // 刷新胜负场次的饼状图
        public Action<double, double> ActUpdatePieChart = null;

        // 是否已经拉取过所有比赛的列表
        private bool _bGottenAllMatchesList = false;

        // 当前正在查看的比赛编号
        public long CurrentMatchId { get; set; } = 0;

        // 当前正在查看的比赛信息
        private DotaMatchInfoModel _CurrentMatchInfo = null;
        public DotaMatchInfoModel CurrentMatchInfo
        {
            get { return _CurrentMatchInfo; }
            set { Set("CurrentMatchInfo", ref _CurrentMatchInfo, value); }
        }

        // 请求过的比赛缓存起来
        private Dictionary<long, DotaMatchInfoModel> _MatchesInfoCache = new Dictionary<long, DotaMatchInfoModel>();

        #region 折线图

        // 天辉优势走势图(金钱和经验)
        private LiveChartsCore.ISeries[] _RadiantAdvantageSeries = new LiveChartsCore.ISeries[]
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                GeometryStroke = new SolidColorPaint(SKColors.Gold, 2),
                GeometrySize = 2,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Gold, 2),
                Name = "Radiant Gold Adv"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                GeometryStroke = new SolidColorPaint(SKColors.MediumOrchid, 2),
                GeometrySize = 2,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.MediumOrchid, 2),
                Name = "Radiant XP Adv"
            }
        };
        public LiveChartsCore.ISeries[] RadiantAdvantageSeries
        {
            get { return _RadiantAdvantageSeries; }
            set { Set("RadiantAdvantageSeries", ref _RadiantAdvantageSeries, value); }
        }

        // 玩家经济/经验走势
        private LiveChartsCore.ISeries[] _PlayersGoldOrXPSeries = new LiveChartsCore.ISeries[]
        {
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(51,117,255,255), 2),
                Name = "Player1"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(102,255,191,255), 2),
                Name = "Player2"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(191,0,191,255), 2),
                Name = "Player3"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(243,240,11,255), 2),
                Name = "Player4"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(255,107,0,255), 2),
                Name = "Player5"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(254,134,194,255), 2),
                Name = "Player6"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(161,180,71,255), 2),
                Name = "Player7"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(101,217,247,255), 2),
                Name = "Player8"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(0,131,33,255), 2),
                Name = "Player9"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double>(),
                Fill = null,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(new SKColor(164,105,0,255), 2),
                Name = "Player10"
            },
        };
        public LiveChartsCore.ISeries[] PlayersGoldOrXPSeries
        {
            get { return _PlayersGoldOrXPSeries; }
            set { Set("PlayersGoldOrXPSeries", ref _PlayersGoldOrXPSeries, value); }
        }

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.LightGray,
                    StrokeThickness = 2
                }
            }
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = SKColors.LightGray,
                    StrokeThickness = 2,
                    PathEffect = new LiveChartsCore.SkiaSharpView.Painting.Effects.DashEffect(new float[] { 3, 3 })
                }
            }
        };

        public RectangularSection[] RadiantAdvSections { get; set; } =
        {
            new RectangularSection
            {
                Yi = 0,
                Yj = 500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(68, 112, 78, 255)
                }
            },
            new RectangularSection
            {
                Yi = 0,
                Yj = -500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(145, 56, 63, 255)
                }
            }
        };

        public RectangularSection[] PlayersGoldXPSections { get; set; } =
        {
            new RectangularSection
            {
                Yi = 0,
                Yj = 500000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(34, 35, 50, 180)
                }
            },
            new RectangularSection
            {
                Yi = 0,
                Yj = -5000,
                Fill = new SolidColorPaint
                {
                    Color = new SKColor(34, 35, 50, 180)
                }
            }
        };

        public DrawMarginFrame Frame { get; set; } = new DrawMarginFrame()
        {
            Fill = new SolidColorPaint
            {
                Color = new SKColor(0, 0, 0, 30)
            },
            Stroke = new SolidColorPaint
            {
                Color = new SKColor(80, 80, 80),
                StrokeThickness = 2
            }
        };

        // 是否有天辉优势走势图数据
        private bool _bHaveRadiantAdv = false;
        public bool bHaveRadiantAdv
        {
            get { return _bHaveRadiantAdv; }
            set { Set("bHaveRadiantAdv", ref _bHaveRadiantAdv, value); }
        }

        // 是否有玩家走势
        private bool _bHavePlayersSeries = false;
        public bool bHavePlayersSeries
        {
            get { return _bHavePlayersSeries; }
            set { Set("bHavePlayersSeries", ref _bHavePlayersSeries, value); }
        }

        // 当前查看的玩家走势类型（true-经济/false-经验）
        private bool _bSelectedPlayersSeriesType = true;
        public bool bSelectedPlayersSeriesType
        {
            get { return _bSelectedPlayersSeriesType; }
            set { Set("bSelectedPlayersSeriesType", ref _bSelectedPlayersSeriesType, value); }
        }

        #endregion

        public DotaMatchesViewModel()
        {
            InitialDotaMatches();
        }

        public async void InitialDotaMatches()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to load Matches ---> " + DateTime.Now.Ticks);

                _bGottenAllMatchesList = false;

                sSteamId = GetSteamID();

                bLoadingHeroesAndItems = true;
                bool triedLoadHeroes = await DotaHeroesViewModel.Instance.LoadDotaHeroes();
                bool triedLoadItems = await DotaItemsViewModel.Instance.LoadDotaItems();

                // 先等获取完英雄和物品列表
                if (!string.IsNullOrEmpty(sSteamId))
                {
                    if (triedLoadHeroes && triedLoadItems)
                    {
                        System.Diagnostics.Debug.WriteLine("Loading Matches ---> " + DateTime.Now.Ticks);

                        bLoadingHeroesAndItems = false;

                        // 玩家信息
                        GetPlayerProfileAsync(sSteamId);

                        // 胜率
                        GetPlayerWLAsync(sSteamId);

                        // 统计数据
                        GetTotalAsync(sSteamId);

                        // 处理最近的比赛
                        GetRecentMatchAsync(sSteamId);

                        // 常用英雄
                        GetHeroesPlayedAsync(sSteamId);

                        // 在线玩家数
                        GetNumberOfCurrentPlayersAsync();
                    }
                }
            }
            catch { }
            finally { bLoadingHeroesAndItems = false; }
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

        /// <summary>
        /// 网络请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        private async Task<T> GetResponseAsync<T>(string url, Windows.Web.Http.HttpClient httpClient)
        {
            try
            {
                var response = await httpClient.GetAsync(new Uri(url));
                var jsonMessage = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(jsonMessage);
                return result;
            }
            catch { }
            return default;
        }


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
                    profile = await GetResponseAsync<DotaMatchPlayerProfileModel>(url, playerInfoHttpClient);
                }
                catch { }

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
                    await PlayerProfile?.profile?.LoadIconAsync(72);
                }

                if (PlayerProfile?.profile != null)
                {
                    var prof = PlayerProfile?.profile;
                    string steamId = this.sSteamId;
                    AddDotaIdHistory(prof.personaname, prof.avatarfull, steamId);
                }
            }
            catch { }
        }

        /// <summary>
        /// 获得用户的胜局败局数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async void GetPlayerWLAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetPlayerWL ---> " + DateTime.Now.Ticks);

                PlayerWinLose = null;

                string url = string.Format("https://api.opendota.com/api/players/{0}/wl", id);
                DotaMatchWinLoseModel wl = null;

                try
                {
                    wl = await GetResponseAsync<DotaMatchWinLoseModel>(url, playerInfoHttpClient);
                }
                catch { }

                if (wl != null && (wl.win + wl.lose) > 0)
                {
                    double rate = wl.win / (wl.win + wl.lose);
                    wl.winRate = (Math.Floor(10000 * rate) / 100).ToString() + "%";

                    PlayerWinLose = wl;
                    ActUpdatePieChart?.Invoke(PlayerWinLose.win, PlayerWinLose.lose);
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取比赛总数据统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async void GetTotalAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetTotal ---> " + DateTime.Now.Ticks);

                vPlayerTotals.Clear();

                string url = string.Format("https://api.opendota.com/api/players/{0}/totals", id);
                List<DotaMatchPlayerTotalModel> totals = null;

                try
                {
                    totals = await GetResponseAsync<List<DotaMatchPlayerTotalModel>>(url, playerInfoHttpClient);
                }
                catch { }

                if (totals != null && totals.Count > 0)
                {
                    Dictionary<string, string> addingKeys = new Dictionary<string, string>()
                    {
                        {"kills", "Kills"}, {"deaths", "Deaths"}, {"assists", "Assists"}, {"gold_per_min", "GPM"}, {"xp_per_min", "XPM"},
                        {"last_hits", "Last Hits"}, {"denies", "Denies"}, {"level", "Level"}, {"hero_damage", "Hero Damage"}, {"tower_damage", "Tower Damage"},
                        {"hero_healing", "Hero Healing"}
                    };

                    double kills = -1, deaths = -1, assists = -1;

                    foreach (var item in totals)
                    {
                        if (addingKeys.ContainsKey(item.field))
                        {
                            item.field = addingKeys[item.field];
                            item.n = Math.Floor((item.sum / item.n) * 10) / 10;
                            vPlayerTotals.Add(item);

                            if (item.field == "Kills") kills = item.n;
                            if (item.field == "Deaths") deaths = item.n;
                            if (item.field == "Assists") assists = item.n;
                        }
                    }
                    if (kills >= 0 && deaths >= 0 && assists >= 0)
                    {
                        double kda = 0;
                        if (deaths <= 0) deaths = 1;
                        kda = Math.Floor(((kills + assists) / deaths) * 100) / 100;

                        vPlayerTotals.Insert(0, new DotaMatchPlayerTotalModel() { field = "KDA", n = kda });
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取最近20场比赛
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async void GetRecentMatchAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetRecentMatch ---> " + DateTime.Now.Ticks);

                vRecentMatchesForFlip.Clear();
                vRecentMatches.Clear();

                string url = string.Format("https://api.opendota.com/api/players/{0}/recentMatches", id);
                List<DotaRecentMatchModel> recentMatches = null;

                try
                {
                    recentMatches = await GetResponseAsync<List<DotaRecentMatchModel>>(url, matchHttpClient);
                }
                catch { }

                if (recentMatches != null)
                {
                    foreach (var item in recentMatches)
                    {
                        if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(item.hero_id.ToString()) == true)
                        {
                            item.sHeroCoverImage = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/crops/{0}.png",
                                DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].name.Replace("npc_dota_hero_", ""));
                            item.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].localized_name;
                            item.sHeroHorizonImage = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].img;
                            item.bWin = null;
                            if (item.player_slot != null && item.radiant_win != null)
                            {
                                if (item.player_slot < 128)// 天辉
                                    item.bWin = item.radiant_win;
                                else if (item.player_slot >= 128)// 夜魇
                                    item.bWin = !item.radiant_win;
                            }
                            vRecentMatches.Add(item);
                            if (vRecentMatchesForFlip.Count < 5)
                                vRecentMatchesForFlip.Add(item);
                        }
                    }

                    foreach (var item in vRecentMatches)
                    {
                        await item.LoadHorizonImageAsync(64);
                    }
                    foreach (var item in vRecentMatchesForFlip)
                    {
                        await item.LoadCoverImageAsync(220);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取玩家常用英雄数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async void GetHeroesPlayedAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetHeroesPlayed ---> " + DateTime.Now.Ticks);

                bLoadingPlayed = true;
                vMostPlayed10Heroes.Clear();
                vMostPlayedHeroes.Clear();

                string url = string.Format("https://api.opendota.com/api/players/{0}/heroes", id);
                List<DotaMatchHeroPlayedModel> heroes = null;

                try
                {
                    heroes = await GetResponseAsync<List<DotaMatchHeroPlayedModel>>(url, matchHttpClient);
                }
                catch { }

                if (heroes != null)
                {
                    foreach (var item in heroes)
                    {
                        if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(item.hero_id.ToString()) == true)
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
                    foreach (var item in vMostPlayedHeroes)
                    {
                        await item.LoadImageAsync(36);
                    }
                }
            }
            catch { }
            finally { bLoadingPlayed = false; }
        }

        /// <summary>
        /// 获取当前在线人数
        /// </summary>
        /// <returns></returns>
        private async void GetNumberOfCurrentPlayersAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to GetNumberOfCurrentPlayers ---> " + DateTime.Now.Ticks);

                string url = "http://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1?appid=570&format=json";
                DotaOnlinePlayersModel online = null;

                try
                {
                    online = await GetResponseAsync<DotaOnlinePlayersModel>(url, playerInfoHttpClient);
                }
                catch { }

                if (online?.response?.result == 1)
                {
                    sOnlilnePlayersCount = online.response.player_count.ToString();
                }
            }
            catch { }
        }

        /// <summary>
        /// 加载所有比赛的列表
        /// </summary>
        public async void GetAllMatchesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(sSteamId)) return;
                if (_bGottenAllMatchesList) return;

                bLoadingAllMatches = true;
                bLoadedAllMatches = true;
                vAllMatchesList.Clear();
                vAllMatches.Clear();

                List<DotaRecentMatchModel> matches = null;

                try
                {
                    string url = string.Format("https://api.opendota.com/api/players/{0}/matches", sSteamId);
                    matches = await GetResponseAsync<List<DotaRecentMatchModel>>(url, matchHttpClient);
                }
                catch { }

                if (matches == null) return;

                _bGottenAllMatchesList = true;

                foreach (var item in matches)
                {
                    if (DotaHeroesViewModel.Instance.dictAllHeroes.ContainsKey(item.hero_id.ToString()))
                    {
                        item.sHeroCoverImage = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/crops/{0}.png",
                            DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].name.Replace("npc_dota_hero_", ""));
                        item.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].localized_name;
                        item.sHeroHorizonImage = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].img;
                        item.bWin = null;
                        if (item.player_slot != null && item.radiant_win != null)
                        {
                            if (item.player_slot < 128)// 天辉
                                item.bWin = item.radiant_win;
                            else if (item.player_slot >= 128)// 夜魇
                                item.bWin = !item.radiant_win;
                        }

                        vAllMatchesList.Add(item);
                        if (vAllMatches.Count < 40)
                        {
                            double kda = 0;
                            if (item.kills != null && item.assists != null && item.deaths != null)
                            {
                                if (item.deaths <= 0)
                                    kda = (double)item.kills + (double)item.assists;
                                else
                                    kda = ((double)item.kills + (double)item.assists) / (double)item.deaths;
                            }
                            item.sKda = kda.ToString("f2");
                            vAllMatches.Add(item);
                        }
                    }
                }

                if (vAllMatchesList.Count <= vAllMatches.Count)
                {
                    bLoadedAllMatches = true;
                }
                else
                {
                    bLoadedAllMatches = false;
                }

                foreach (var item in vAllMatches)
                {
                    await item.LoadHorizonImageAsync(64);
                }
            }
            catch { }
            finally { bLoadingAllMatches = false; }
        }

        /// <summary>
        /// 从所有比赛中再取出20条显示
        /// </summary>
        public async void IncreaseFromAllMatches()
        {
            try
            {
                // 未绑定账号或者还没有拉到列表时就不处理
                if (string.IsNullOrEmpty(sSteamId)) return;
                if (!_bGottenAllMatchesList) return;

                int index = vAllMatches.Count;
                for (int i = index; i < index + 30 && i < vAllMatchesList.Count; i++)
                {
                    if (i >= vAllMatchesList.Count) break;

                    var item = vAllMatchesList[i];

                    if (DotaHeroesViewModel.Instance.dictAllHeroes.ContainsKey(item.hero_id.ToString()))
                    {
                        item.sHeroCoverImage = string.Format("https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/crops/{0}.png",
                            DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].name.Replace("npc_dota_hero_", ""));
                        item.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].localized_name;
                        item.sHeroHorizonImage = DotaHeroesViewModel.Instance.dictAllHeroes[item.hero_id.ToString()].img;
                        item.bWin = null;
                        if (item.player_slot != null && item.radiant_win != null)
                        {
                            if (item.player_slot < 128)// 天辉
                                item.bWin = item.radiant_win;
                            else if (item.player_slot >= 128)// 夜魇
                                item.bWin = !item.radiant_win;
                        }

                        double kda = 0;
                        if (item.kills != null && item.assists != null && item.deaths != null)
                        {
                            if (item.deaths <= 0)
                                kda = (double)item.kills + (double)item.assists;
                            else
                                kda = ((double)item.kills + (double)item.assists) / (double)item.deaths;
                        }
                        item.sKda = kda.ToString("f2");

                        await item.LoadHorizonImageAsync(64);

                        vAllMatches.Add(item);
                    }
                }

                if (vAllMatchesList.Count <= vAllMatches.Count)
                {
                    bLoadedAllMatches = true;
                }
                else
                {
                    bLoadedAllMatches = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// 加载指定比赛
        /// </summary>
        public async void GetMatchInfoAsync(long matchId)
        {
            try
            {
                if (matchId == 0 || (CurrentMatchId == matchId && CurrentMatchInfo != null)) return;

                CurrentMatchId = matchId;
                CurrentMatchInfo = null;
                bLoadingOneMatchInfo = true;

                bHaveRadiantAdv = false;
                bHavePlayersSeries = false;
                bSelectedPlayersSeriesType = true;

                string url = string.Format("https://api.opendota.com/api/matches/{0}", matchId);    //e.g.3792271763
                DotaMatchInfoModel matchInfo = null;

                try
                {
                    if (_MatchesInfoCache.ContainsKey(matchId))
                    {
                        matchInfo = _MatchesInfoCache[matchId];
                    }
                    else
                    {
                        matchInfo = await GetResponseAsync<DotaMatchInfoModel>(url, matchInfoHttpClient);
                        if (matchInfo != null)
                        {
                            _MatchesInfoCache[matchId] = matchInfo;
                            if (_MatchesInfoCache.Count > 100)
                            {
                                var removing = _MatchesInfoCache.ElementAt(0);
                                _MatchesInfoCache.Remove(removing.Key);
                            }
                        }
                    }
                    #region
                    //    "players": [
                    //      {
                    //        "match_id": 6706352286,
                    //        "player_slot": 0,
                    //        "ability_upgrades_arr": [
                    //          5162, 5163, 5163, 5162, 5163, 5165, 5163, 5162, 5162, 768, 5164, 5165,
                    //          5164, 5164, 6412, 5164, 5165, 6505
                    //        ],
                    //        "account_id": 198161112,
                    //        "additional_units": [
                    //          {
                    //            "unitname": "spirit_bear",
                    //            "item_0": 168,
                    //            "item_1": 50,
                    //            "item_2": 172,
                    //            "item_3": 116,
                    //            "item_4": 143,
                    //            "item_5": 112,
                    //            "backpack_0": 0,
                    //            "backpack_1": 0,
                    //            "backpack_2": 0,
                    //            "item_neutral": 0
                    //          }
                    //        ],
                    //        "assists": 23,
                    //        "backpack_0": 0,
                    //        "backpack_1": 188,
                    //        "backpack_2": 0,
                    //        "backpack_3": null,
                    //        "camps_stacked": 3,
                    //        "creeps_stacked": 14,
                    //        "deaths": 11,
                    //        "denies": 5,
                    //        "gold": 3322,
                    //        "gold_per_min": 373,
                    //        "gold_spent": 15625,
                    //        "gold_t": [
                    //          0, 219, 319, 454, 808, 983, 1129, 1359, 1740, 1919, 2158, 2329, 2512,
                    //          2854, 3069, 3220, 4172, 4528, 4837, 5294, 5646, 6307, 6591, 6920, 7159,
                    //          7449, 7561, 7741, 8038, 8966, 9511, 9878, 10143, 10775, 11164, 11709,
                    //          12066, 13015, 13215, 13579, 14030, 15157, 15367, 15640, 15760, 15989,
                    //          16266, 16780, 17115, 17374, 18161, 19126
                    //        ],
                    //        "hero_damage": 30193,
                    //        "hero_healing": 18628,
                    //        "hero_id": 37,
                    //        "item_0": 908,
                    //        "item_1": 190,
                    //        "item_2": 254,
                    //        "item_3": 269,
                    //        "item_4": 218,
                    //        "item_5": 29,
                    //        "item_neutral": 676,
                    //        "kills": 3,
                    //        "last_hits": 142,
                    //        "level": 24,
                    //        "net_worth": 16447,
                    //        "obs_placed": 10,
                    //        "party_id": 0,
                    //        "party_size": 10,
                    //        "permanent_buffs": [
                    //          { "permanent_buff": 6, "stack_count": 2 },
                    //          { "permanent_buff": 12, "stack_count": 0 }
                    //        ],
                    //        "pings": 34,
                    //        "purchase_log": [
                    //          { "time": -89, "key": "tango" },
                    //          { "time": -89, "key": "magic_stick" },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": -89, "key": "enchanted_mango", "charges": 2 },
                    //          { "time": -89, "key": "branches" },
                    //          { "time": 4, "key": "ward_sentry" },
                    //          { "time": 4, "key": "ward_sentry" },
                    //          { "time": 225, "key": "boots" },
                    //          { "time": 226, "key": "clarity" },
                    //          { "time": 226, "key": "clarity" },
                    //          { "time": 385, "key": "sobi_mask" },
                    //          { "time": 385, "key": "ring_of_basilius" },
                    //          { "time": 387, "key": "ward_sentry" },
                    //          { "time": 483, "key": "ring_of_protection" },
                    //          { "time": 483, "key": "buckler" },
                    //          { "time": 609, "key": "tpscroll" },
                    //          { "time": 611, "key": "magic_wand" },
                    //          { "time": 613, "key": "ward_observer" },
                    //          { "time": 613, "key": "ward_sentry" },
                    //          { "time": 614, "key": "smoke_of_deceit" },
                    //          { "time": 720, "key": "blades_of_attack" },
                    //          { "time": 813, "key": "tpscroll" },
                    //          { "time": 857, "key": "ward_observer" },
                    //          { "time": 857, "key": "ward_observer" },
                    //          { "time": 857, "key": "ward_sentry" },
                    //          { "time": 857, "key": "ward_sentry" },
                    //          { "time": 954, "key": "lifesteal" },
                    //          { "time": 976, "key": "vladmir" },
                    //          { "time": 992, "key": "tpscroll" },
                    //          { "time": 1035, "key": "ward_sentry" },
                    //          { "time": 1035, "key": "ward_sentry" },
                    //          { "time": 1100, "key": "tpscroll" },
                    //          { "time": 1102, "key": "smoke_of_deceit" },
                    //          { "time": 1102, "key": "ward_sentry" },
                    //          { "time": 1205, "key": "tpscroll" },
                    //          { "time": 1213, "key": "tome_of_knowledge" },
                    //          { "time": 1237, "key": "wraith_pact" },
                    //          { "time": 1237, "key": "point_booster" },
                    //          { "time": 1317, "key": "ward_sentry" },
                    //          { "time": 1317, "key": "ward_observer" },
                    //          { "time": 1317, "key": "dust" },
                    //          { "time": 1318, "key": "smoke_of_deceit" },
                    //          { "time": 1318, "key": "ward_sentry" },
                    //          { "time": 1420, "key": "ward_observer" },
                    //          { "time": 1420, "key": "ward_sentry" },
                    //          { "time": 1666, "key": "aghanims_shard" },
                    //          { "time": 1672, "key": "ward_observer" },
                    //          { "time": 1672, "key": "ward_sentry" },
                    //          { "time": 1672, "key": "ward_sentry" },
                    //          { "time": 1752, "key": "ward_sentry" },
                    //          { "time": 1752, "key": "dust" },
                    //          { "time": 1753, "key": "smoke_of_deceit" },
                    //          { "time": 1753, "key": "ward_sentry" },
                    //          { "time": 1857, "key": "tome_of_knowledge" },
                    //          { "time": 1891, "key": "sobi_mask" },
                    //          { "time": 1891, "key": "ring_of_basilius" },
                    //          { "time": 1892, "key": "crown" },
                    //          { "time": 1901, "key": "veil_of_discord" },
                    //          { "time": 1985, "key": "ward_observer" },
                    //          { "time": 1986, "key": "ward_sentry" },
                    //          { "time": 2225, "key": "shadow_amulet" },
                    //          { "time": 2225, "key": "cloak" },
                    //          { "time": 2225, "key": "glimmer_cape" },
                    //          { "time": 2234, "key": "ward_sentry" },
                    //          { "time": 2340, "key": "ward_sentry" },
                    //          { "time": 2340, "key": "ward_observer" },
                    //          { "time": 2341, "key": "ward_sentry" },
                    //          { "time": 2502, "key": "ward_observer" },
                    //          { "time": 2503, "key": "ward_sentry" },
                    //          { "time": 2503, "key": "ward_sentry" },
                    //          { "time": 2541, "key": "ring_of_regen" },
                    //          { "time": 2541, "key": "headdress" },
                    //          { "time": 2541, "key": "fluffy_hat" },
                    //          { "time": 2549, "key": "energy_booster" },
                    //          { "time": 2553, "key": "holy_locket" },
                    //          { "time": 2715, "key": "ward_observer" },
                    //          { "time": 2715, "key": "ward_sentry" },
                    //          { "time": 2926, "key": "ward_observer" },
                    //          { "time": 2926, "key": "ward_observer" },
                    //          { "time": 2926, "key": "ward_sentry" },
                    //          { "time": 2926, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" },
                    //          { "time": 2927, "key": "ward_sentry" }
                    //        ],
                    //        "randomed": false,
                    //        "roshans_killed": 0,
                    //        "runes_log": [
                    //          { "time": 0, "key": 5 },
                    //          { "time": 361, "key": 5 },
                    //          { "time": 1468, "key": 5 },
                    //          { "time": 2708, "key": 5 }
                    //        ],
                    //        "tower_damage": 932,
                    //        "towers_killed": 1,
                    //        "xp_per_min": 543,
                    //        "xp_t": [
                    //          0, 118, 356, 444, 642, 866, 991, 1121, 1410, 1751, 2291, 2555, 2932,
                    //          3177, 3692, 4052, 4492, 6046, 6876, 7595, 7959, 8878, 9125, 9410, 9526,
                    //          10159, 10209, 10594, 10813, 12551, 13163, 13379, 14755, 14831, 15130,
                    //          16991, 18015, 18087, 18839, 19448, 20880, 22020, 22102, 22423, 22890,
                    //          23326, 23708, 24192, 25297, 25453, 25551, 27835
                    //        ],
                    //        "personaname": "euphoria",
                    //        "name": "detachment",
                    //        "radiant_win": false,
                    //        "isRadiant": true,
                    //        "total_gold": 19184,
                    //        "total_xp": 27928,
                    //        "neutral_kills": 26,
                    //        "tower_kills": 1,
                    //        "courier_kills": 0,
                    //        "observer_kills": 3,
                    //        "sentry_kills": 3,
                    //        "roshan_kills": 0,
                    //        "buyback_count": 0,
                    //        "rank_tier": 80,
                    //        "benchmarks": {
                    //          "gold_per_min": { "raw": 373, "pct": 0.8095238095238095 },
                    //          "xp_per_min": { "raw": 543, "pct": 0.8571428571428571 },
                    //          "kills_per_min": {
                    //            "raw": 0.05832793259883344,
                    //            "pct": 0.19047619047619047
                    //          },
                    //          "last_hits_per_min": {
                    //            "raw": 2.7608554763447826,
                    //            "pct": 0.8571428571428571
                    //          },
                    //          "hero_damage_per_min": {
                    //            "raw": 587.0317563188594,
                    //            "pct": 0.9047619047619048
                    //          },
                    //          "hero_healing_per_min": {
                    //            "raw": 362.17757615035646,
                    //            "pct": 0.8095238095238095
                    //          },
                    //          "tower_damage": { "raw": 932, "pct": 0.5714285714285714 },
                    //          "stuns_per_min": {
                    //            "raw": 0.33382604018146467,
                    //            "pct": 0.47619047619047616
                    //          },
                    //          "lhten": { "raw": 12, "pct": 0.8571428571428571 }
                    //        }
                    //      }
                    //    ]
                    //}
                    #endregion
                }
                catch { }

                if (matchInfo != null && matchInfo.match_id == CurrentMatchId)
                {
                    // BanPick列表
                    try
                    {
                        if (matchInfo.picks_bans == null)
                        {
                            matchInfo.picks_bans = new List<Picks_Bans>();
                        }
                        foreach (var bp in matchInfo.picks_bans)
                        {
                            if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(bp.hero_id.ToString()) == true)
                            {
                                bp.sHeroImage = DotaHeroesViewModel.Instance.dictAllHeroes[bp.hero_id.ToString()].img;
                                bp.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[bp.hero_id.ToString()].localized_name;
                                await bp.LoadImageAsync(86);
                            }
                        }
                    }
                    catch { }

                    // 玩家列表
                    try
                    {
                        // 用于整理玩家开黑编号
                        Dictionary<long, int> playersPartyDict = new Dictionary<long, int>();
                        int partyId = 0;

                        for (int i = 0; i < matchInfo.players.Count; i++)
                        {
                            try
                            {
                                var player = matchInfo.players[i];

                                // 玩家使用的英雄
                                try
                                {
                                    if (DotaHeroesViewModel.Instance.dictAllHeroes?.ContainsKey(player.hero_id.ToString()) == true)
                                    {
                                        player.sHeroName = DotaHeroesViewModel.Instance.dictAllHeroes[player.hero_id.ToString()].localized_name;
                                        player.sHeroImage = DotaHeroesViewModel.Instance.dictAllHeroes[player.hero_id.ToString()].img;
                                    }
                                }
                                catch { }

                                // 玩家的物品
                                try
                                {
                                    player.sItem0 = GetItemImgById(player.item_0?.ToString());
                                    player.sItem1 = GetItemImgById(player.item_1?.ToString());
                                    player.sItem2 = GetItemImgById(player.item_2?.ToString());
                                    player.sItem3 = GetItemImgById(player.item_3?.ToString());
                                    player.sItem4 = GetItemImgById(player.item_4?.ToString());
                                    player.sItem5 = GetItemImgById(player.item_5?.ToString());
                                    player.sItemB0 = GetItemImgById(player.backpack_0?.ToString());
                                    player.sItemB1 = GetItemImgById(player.backpack_1?.ToString());
                                    player.sItemB2 = GetItemImgById(player.backpack_2?.ToString());
                                    player.sItemN = GetItemImgById(player.item_neutral?.ToString());
                                    player.sNameItem0 = GetItemNameById(player.item_0?.ToString());
                                    player.sNameItem1 = GetItemNameById(player.item_1?.ToString());
                                    player.sNameItem2 = GetItemNameById(player.item_2?.ToString());
                                    player.sNameItem3 = GetItemNameById(player.item_3?.ToString());
                                    player.sNameItem4 = GetItemNameById(player.item_4?.ToString());
                                    player.sNameItem5 = GetItemNameById(player.item_5?.ToString());
                                    player.sNameItemB0 = GetItemNameById(player.backpack_0?.ToString());
                                    player.sNameItemB1 = GetItemNameById(player.backpack_1?.ToString());
                                    player.sNameItemB2 = GetItemNameById(player.backpack_2?.ToString());
                                    player.sNameItemN = GetItemNameById(player.item_neutral?.ToString());
                                }
                                catch { }

                                // 判断是天辉还是夜魇
                                try
                                {
                                    if (player.isRadiant == null)
                                    {
                                        player.isRadiant = player.player_slot < 128 ? true : false;
                                    }
                                }
                                catch { }

                                // 判断是否是当前绑定账号的玩家
                                try
                                {
                                    if (player.account_id.ToString() == sSteamId)
                                    {
                                        player.bIsCurrentPlayer = true;
                                    }
                                }
                                catch { }

                                // 神杖和魔晶
                                try
                                {
                                    player.bHaveAghanimScepter = false;
                                    player.bHaveAghanimShard = false;
                                    if (player?.permanent_buffs != null)
                                    {
                                        foreach (var buff in player.permanent_buffs)
                                        {
                                            if (buff.permanent_buff == 2)
                                            {
                                                player.bHaveAghanimScepter = true;
                                            }
                                            if (buff.permanent_buff == 12)
                                            {
                                                player.bHaveAghanimShard = true;
                                            }
                                        }
                                    }
                                }
                                catch { }

                                // 玩家的开黑编号
                                try
                                {
                                    player.iPartyId = 0;
                                    if (player.party_id != null && player.party_size != null && player.party_size > 1 && player.party_size < 10)
                                    {
                                        long id = (long)player.party_id;
                                        if (!playersPartyDict.ContainsKey(id))
                                        {
                                            partyId++;
                                            playersPartyDict.Add(id, partyId);
                                        }
                                        player.iPartyId = playersPartyDict[id];
                                    }
                                }
                                catch { }

                                // 额外单位，目前只处理德鲁伊(hero_id=80)的熊灵
                                try
                                {
                                    player.SpiritBear = null;
                                    if (player.hero_id == 80 && player.additional_units != null && player.additional_units.Length > 0)
                                    {
                                        foreach (var item in player.additional_units)
                                        {
                                            if (item.unitname.Contains("spirit_bear"))
                                            {
                                                player.SpiritBear = item;
                                                break;
                                            }
                                        }

                                        if (player.SpiritBear != null)
                                        {
                                            player.SpiritBear.sItem0 = GetItemImgById(player.SpiritBear.item_0?.ToString());
                                            player.SpiritBear.sItem1 = GetItemImgById(player.SpiritBear.item_1?.ToString());
                                            player.SpiritBear.sItem2 = GetItemImgById(player.SpiritBear.item_2?.ToString());
                                            player.SpiritBear.sItem3 = GetItemImgById(player.SpiritBear.item_3?.ToString());
                                            player.SpiritBear.sItem4 = GetItemImgById(player.SpiritBear.item_4?.ToString());
                                            player.SpiritBear.sItem5 = GetItemImgById(player.SpiritBear.item_5?.ToString());
                                            player.SpiritBear.sItemB0 = GetItemImgById(player.SpiritBear.backpack_0?.ToString());
                                            player.SpiritBear.sItemB1 = GetItemImgById(player.SpiritBear.backpack_1?.ToString());
                                            player.SpiritBear.sItemB2 = GetItemImgById(player.SpiritBear.backpack_2?.ToString());
                                            player.SpiritBear.sItemN = GetItemImgById(player.SpiritBear.item_neutral?.ToString());
                                            player.SpiritBear.sNameItem0 = GetItemNameById(player.SpiritBear.item_0?.ToString());
                                            player.SpiritBear.sNameItem1 = GetItemNameById(player.SpiritBear.item_1?.ToString());
                                            player.SpiritBear.sNameItem2 = GetItemNameById(player.SpiritBear.item_2?.ToString());
                                            player.SpiritBear.sNameItem3 = GetItemNameById(player.SpiritBear.item_3?.ToString());
                                            player.SpiritBear.sNameItem4 = GetItemNameById(player.SpiritBear.item_4?.ToString());
                                            player.SpiritBear.sNameItem5 = GetItemNameById(player.SpiritBear.item_5?.ToString());
                                            player.SpiritBear.sNameItemB0 = GetItemNameById(player.SpiritBear.backpack_0?.ToString());
                                            player.SpiritBear.sNameItemB1 = GetItemNameById(player.SpiritBear.backpack_1?.ToString());
                                            player.SpiritBear.sNameItemB2 = GetItemNameById(player.SpiritBear.backpack_2?.ToString());
                                            player.SpiritBear.sNameItemN = GetItemNameById(player.SpiritBear.item_neutral?.ToString());
                                        }
                                    }
                                }
                                catch { }

                                // 玩家的经济经验走势
                                try
                                {
                                    bHavePlayersSeries = SwitchPlayersGoldOrXPSeries(true, matchInfo);
                                }
                                catch { }
                            }
                            catch { }
                        }

                        foreach (var player in matchInfo.players)
                        {
                            try
                            {
                                await player.LoadImageAsync(86);
                                await player.LoadItemsImageAsync(44, 35, 134);
                                if (player.SpiritBear != null)
                                {
                                    await player.SpiritBear?.LoadItemsImageAsync(44, 35, 134);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }

                    // 天辉优势图
                    try
                    {
                        if (matchInfo?.radiant_gold_adv != null && matchInfo?.radiant_xp_adv != null)
                        {
                            if (RadiantAdvantageSeries[0].Values is ObservableCollection<double> v1 && v1 != null)
                            {
                                v1.Clear();
                                foreach (var item in matchInfo.radiant_gold_adv)
                                {
                                    v1.Add(item);
                                }
                            }
                            else
                            {
                                RadiantAdvantageSeries[0].Values = new ObservableCollection<double>(matchInfo.radiant_gold_adv);
                            }

                            if (RadiantAdvantageSeries[1].Values is ObservableCollection<double> v2 && v2 != null)
                            {
                                v2.Clear();
                                foreach (var item in matchInfo.radiant_xp_adv)
                                {
                                    v2.Add(item);
                                }
                            }
                            else
                            {
                                RadiantAdvantageSeries[1].Values = new ObservableCollection<double>(matchInfo.radiant_xp_adv);
                            }

                            bHaveRadiantAdv = true;
                        }
                    }
                    catch { }

                    // 双方职业战队
                    try
                    {
                        if (matchInfo.radiant_team != null)
                        {
                            if (string.IsNullOrEmpty(matchInfo.radiant_team.name))
                            {
                                matchInfo.radiant_team.name = matchInfo.radiant_team.team_id;
                            }
                        }
                        if (matchInfo.dire_team != null)
                        {
                            if (string.IsNullOrEmpty(matchInfo.dire_team.name))
                            {
                                matchInfo.dire_team.name = matchInfo.dire_team.team_id;
                            }
                        }
                    }
                    catch { }

                    CurrentMatchInfo = matchInfo;
                }
            }
            catch { }
            finally { bLoadingOneMatchInfo = false; }
        }

        /// <summary>
        /// 请求更新数据
        /// </summary>
        /// <param name="id"></param>
        public async void PostRefreshAsync(string id)
        {
            try
            {
                string url = string.Format("https://api.opendota.com/api/players/{0}/refresh", id);
                await playerInfoHttpClient.PostAsync(new Uri(url), null);
            }
            catch { }
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
                await his.LoadImageAsync(56);
            }
            catch { }
        }

        /// <summary>
        /// 从物品字典中查找物品，返回其图片地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetItemImgById(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (DotaItemsViewModel.Instance.dictIdToAllItems?.ContainsKey(id) == true)
                    {
                        return DotaItemsViewModel.Instance.dictIdToAllItems[id].img;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 从物品字典中查找物品，返回其名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetItemNameById(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (DotaItemsViewModel.Instance.dictIdToAllItems?.ContainsKey(id) == true)
                    {
                        return DotaItemsViewModel.Instance.dictIdToAllItems[id].dname;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 加载玩家的经济或者经验走势
        /// </summary>
        /// <param name="isGold"></param>
        /// <returns></returns>
        public bool SwitchPlayersGoldOrXPSeries(bool isGold, DotaMatchInfoModel matchInfo)
        {
            try
            {
                bSelectedPlayersSeriesType = isGold;

                bool havePlayersSeries = false;
                for (int i = 0; i < matchInfo.players.Count && i < PlayersGoldOrXPSeries.Length; i++)
                {
                    var player = matchInfo.players[i];
                    if (player == null) continue;

                    List<double> list = isGold ? player.gold_t : player.xp_t;

                    if (PlayersGoldOrXPSeries[i].Values is ObservableCollection<double> v && v != null)
                    {
                        v.Clear();
                        if (list != null && list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                v.Add(item);
                            }
                            havePlayersSeries = true;
                        }
                    }
                    else
                    {
                        if (list != null && list.Count > 0)
                        {
                            PlayersGoldOrXPSeries[i].Values = new ObservableCollection<double>(list);
                            havePlayersSeries = true;
                        }
                        else
                        {
                            PlayersGoldOrXPSeries[i].Values = new ObservableCollection<double>();
                        }
                    }
                    PlayersGoldOrXPSeries[i].Name = player.sHeroName;
                }
                return havePlayersSeries;
            }
            catch { }
            return false;
        }
    }
}
