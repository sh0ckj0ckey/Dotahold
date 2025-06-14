using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    public partial class DotaMatchesViewModel : ViewModelBase
    {
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

        // 是否正在搜索比赛编号
        private bool _bSearchingByMatchId = false;
        public bool bSearchingByMatchId
        {
            get { return _bSearchingByMatchId; }
            set { Set("bSearchingByMatchId", ref _bSearchingByMatchId, value); }
        }

        // 是否已经拉取过所有比赛的列表
        private bool _bGottenAllMatchesList = false;

        // 加载所有比赛的列表
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

        #region Heroes Played

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
