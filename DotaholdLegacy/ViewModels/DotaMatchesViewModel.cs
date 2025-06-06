﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
        private static Lazy<DotaMatchesViewModel> _lazyVM = new Lazy<DotaMatchesViewModel>(() => new DotaMatchesViewModel());
        public static DotaMatchesViewModel Instance => _lazyVM.Value;

        private Windows.Web.Http.HttpClient _playerInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient _matchHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient _matchInfoHttpClient = new Windows.Web.Http.HttpClient();

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
        private List<DotaRecentMatchModel> _vAllMatchesList = new List<DotaRecentMatchModel>();
        public ObservableCollection<DotaRecentMatchModel> vAllMatches = new ObservableCollection<DotaRecentMatchModel>();

        // 玩家的统计数据
        public ObservableCollection<DotaMatchPlayerTotalModel> vPlayerTotals = new ObservableCollection<DotaMatchPlayerTotalModel>();

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

        // 刷新胜负场次图
        public Action<double, double> ActUpdateWinRateCapsule = null;

        // 是否已经拉取过所有比赛的列表
        private bool _bGottenAllMatchesList = false;

        public DotaMatchesViewModel()
        {
            InitialDotaMatches();

            LoadBindedDotaIdHistory();
        }

        public async void InitialDotaMatches()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Going to load Matches ---> " + DateTime.Now.Ticks);

                _bGottenAllMatchesList = false;

                vOneHeroMatches.Clear();
                CurrentHeroForPlayedMatches = null;

                sSteamId = DotaViewModel.Instance.AppSettings.sSteamID;

                bLoadingHeroesAndItems = true;
                bool triedLoadHeroes = await DotaHeroesViewModel.Instance.LoadDotaHeroes();
                bool triedLoadItems = await DotaItemsViewModel.Instance.LoadDotaItems();

                // 先等获取完英雄和物品列表
                if (!string.IsNullOrWhiteSpace(sSteamId))
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            finally { bLoadingHeroesAndItems = false; }
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
                    recentMatches = await GetResponseAsync<List<DotaRecentMatchModel>>(url, _matchHttpClient);
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 加载所有比赛的列表
        /// </summary>
        public async Task<List<DotaRecentMatchModel>> GetAllMatchesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(sSteamId)) return null;
                if (_bGottenAllMatchesList) return _vAllMatchesList;

                bLoadingAllMatches = true;
                bLoadedAllMatches = true;
                _vAllMatchesList.Clear();
                vAllMatches.Clear();

                List<DotaRecentMatchModel> matches = null;

                try
                {
                    string url = string.Format("https://api.opendota.com/api/players/{0}/matches", sSteamId);
                    matches = await GetResponseAsync<List<DotaRecentMatchModel>>(url, _matchHttpClient);
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

                if (matches == null) return _vAllMatchesList;

                _bGottenAllMatchesList = true;

                foreach (var item in matches)
                {
                    if (DotaHeroesViewModel.Instance.dictAllHeroes.ContainsKey(item.hero_id?.ToString()))
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

                        _vAllMatchesList.Add(item);
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

                if (_vAllMatchesList.Count <= vAllMatches.Count)
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

                return _vAllMatchesList;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            finally { bLoadingAllMatches = false; }
            return _vAllMatchesList;
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
                for (int i = index; i < index + 30 && i < _vAllMatchesList.Count; i++)
                {
                    try
                    {
                        if (i >= _vAllMatchesList.Count) break;

                        var item = _vAllMatchesList[i];

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
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                }

                if (_vAllMatchesList.Count <= vAllMatches.Count)
                {
                    bLoadedAllMatches = true;
                }
                else
                {
                    bLoadedAllMatches = false;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
                await _playerInfoHttpClient.PostAsync(new Uri(url), null);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return string.Empty;
        }

        /// <summary>
        /// 从物品字典中查找物品，返回其图片地址
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetItemImgByName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (DotaItemsViewModel.Instance.dictNameToAllItems?.ContainsKey(name) == true)
                    {
                        return DotaItemsViewModel.Instance.dictNameToAllItems[name].img;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return string.Empty;
        }

        #region Heroes Played

        // 最常用的10个英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayed10Heroes = new ObservableCollection<DotaMatchHeroPlayedModel>();

        // 所有的最常用英雄
        public ObservableCollection<DotaMatchHeroPlayedModel> vMostPlayedHeroes = new ObservableCollection<DotaMatchHeroPlayedModel>();

        // 当前指定的英雄的比赛记录
        public ObservableCollection<DotaRecentMatchModel> vOneHeroMatches = new ObservableCollection<DotaRecentMatchModel>();

        // 是否正在加载常用英雄
        private bool _bLoadingPlayed = false;
        public bool bLoadingPlayed
        {
            get { return _bLoadingPlayed; }
            set { Set("bLoadingPlayed", ref _bLoadingPlayed, value); }
        }

        // 当前查看比赛记录的英雄
        private DotaMatchHeroPlayedModel _currentHeroForPlayedMatches = null;
        public DotaMatchHeroPlayedModel CurrentHeroForPlayedMatches
        {
            get { return _currentHeroForPlayedMatches; }
            set { Set("CurrentHeroForPlayedMatches", ref _currentHeroForPlayedMatches, value); }
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
                    heroes = await GetResponseAsync<List<DotaMatchHeroPlayedModel>>(url, _matchHttpClient);
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            finally { bLoadingPlayed = false; }
        }

        /// <summary>
        /// 获取指定英雄的比赛记录
        /// </summary>
        /// <param name="hero"></param>
        public async void GetHeroMatchesFormAllAsync(DotaMatchHeroPlayedModel hero)
        {
            try
            {
                if (hero == null || string.IsNullOrEmpty(hero.hero_id) || hero == CurrentHeroForPlayedMatches) return;

                var allMatches = await GetAllMatchesAsync();

                CurrentHeroForPlayedMatches = hero;
                vOneHeroMatches.Clear();

                if (allMatches != null && allMatches.Count > 0)
                {
                    foreach (var item in allMatches)
                    {
                        try
                        {
                            if (item.hero_id?.ToString() == CurrentHeroForPlayedMatches.hero_id)
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
                                vOneHeroMatches.Add(item);
                            }
                        }
                        catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                    }

                    foreach (var item in vOneHeroMatches)
                    {
                        try
                        {
                            await item.LoadHorizonImageAsync(64);
                        }
                        catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion
    }
}
