using Newtonsoft.Json;
using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.ViewModels
{
    public class DotaHeroesViewModel : ViewModelBase
    {
        private static Lazy<DotaHeroesViewModel> _lazyVM = new Lazy<DotaHeroesViewModel>(() => new DotaHeroesViewModel());
        public static DotaHeroesViewModel Instance => _lazyVM.Value;

        // 所有英雄
        public Dictionary<string, Models.DotaHeroModel> mapAllHeroes { get; set; } = new Dictionary<string, Models.DotaHeroModel>();

        // 列表展示的英雄
        public ObservableCollection<Models.DotaHeroModel> vStrHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();
        public ObservableCollection<Models.DotaHeroModel> vAgiHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();
        public ObservableCollection<Models.DotaHeroModel> vIntHeroesList { get; set; } = new ObservableCollection<Models.DotaHeroModel>();

        // 缓存拉取过的英雄详情
        private Dictionary<int, Models.DotaHeroInfoModel> dictHeroInfos { get; set; } = new Dictionary<int, Models.DotaHeroInfoModel>();

        // 缓存拉取过的英雄排行榜
        private Dictionary<int, Models.DotaHeroRankingModel> dictHeroRankings { get; set; } = new Dictionary<int, Models.DotaHeroRankingModel>();


        // 是否正在加载英雄列表
        private bool _bLoadingHeroes = false;
        public bool bLoadingHeroes
        {
            get { return _bLoadingHeroes; }
            set { Set("bLoadingHeroes", ref _bLoadingHeroes, value); }
        }

        // 英雄列表加载是否失败
        private bool _bFailedHeroes = false;
        public bool bFailedHeroes
        {
            get { return _bFailedHeroes; }
            set { Set("bFailedHeroes", ref _bFailedHeroes, value); }
        }


        // 是否正在加载英雄详情
        private bool _bLoadingHeroInfo = false;
        public bool bLoadingHeroInfo
        {
            get { return _bLoadingHeroInfo; }
            set { Set("bLoadingHeroInfo", ref _bLoadingHeroInfo, value); }
        }

        // 英雄详情加载是否失败
        private bool _bFailedHeroInfo = false;
        public bool bFailedHeroInfo
        {
            get { return _bFailedHeroInfo; }
            set { Set("bFailedHeroInfo", ref _bFailedHeroInfo, value); }
        }


        // 是否正在加载英雄排行榜
        private bool _bLoadingHeroRanking = false;
        public bool bLoadingHeroRanking
        {
            get { return _bLoadingHeroRanking; }
            set { Set("bLoadingHeroRanking", ref _bLoadingHeroRanking, value); }
        }

        // 英雄排行榜加载是否失败
        private bool _bFailedHeroRanking = false;
        public bool bFailedHeroRanking
        {
            get { return _bFailedHeroRanking; }
            set { Set("bFailedHeroRanking", ref _bFailedHeroRanking, value); }
        }


        // 英雄属性Tab
        private int _iHeroAttrTabIndex = 0;
        public int iHeroAttrTabIndex
        {
            get { return _iHeroAttrTabIndex; }
            set { Set("iHeroAttrTabIndex", ref _iHeroAttrTabIndex, value); }
        }

        // 当前选中的英雄
        private Models.DotaHeroModel _CurrentHero = null;
        public Models.DotaHeroModel CurrentHero
        {
            get { return _CurrentHero; }
            private set { Set("CurrentHero", ref _CurrentHero, value); }
        }

        // 当前选中的英雄的详情
        private Models.Hero _CurrentHeroInfo = null;
        public Models.Hero CurrentHeroInfo
        {
            get { return _CurrentHeroInfo; }
            private set { Set("CurrentHeroInfo", ref _CurrentHeroInfo, value); }
        }

        // 当前选中的英雄的英雄榜
        private ObservableCollection<Models.RankingPlayer> _vRankingPlayers = null;
        public ObservableCollection<Models.RankingPlayer> vRankingPlayers
        {
            get { return _vRankingPlayers; }
            private set { Set("vRankingPlayers", ref _vRankingPlayers, value); }
        }

        // 获取到英雄详情后触发动画
        public Action ActPopInHeroInfoGrid { get; set; } = null;
        public Action ActShowHeroInfoButton { get; set; } = null;

        // 是否已经成功加载过英雄列表
        private bool _bLoadedDotaHeroes = false;

        private Windows.Web.Http.HttpClient heroInfoHttpClient = new Windows.Web.Http.HttpClient();
        private Windows.Web.Http.HttpClient heroRankingHttpClient = new Windows.Web.Http.HttpClient();

        public DotaHeroesViewModel()
        {
            //LoadDotaHeroes();
        }

        public async void LoadDotaHeroes()
        {
            try
            {
                if (_bLoadedDotaHeroes)
                    return;

                _bLoadedDotaHeroes = true;

                bLoadingHeroes = true;

                mapAllHeroes?.Clear();
                vStrHeroesList?.Clear();
                vAgiHeroesList?.Clear();
                vIntHeroesList?.Clear();
                mapAllHeroes = await ConstantsHelper.Instance.GetHeroesConstant();

                if (mapAllHeroes == null || mapAllHeroes.Count <= 0)
                {
                    bLoadingHeroes = false;
                    _bLoadedDotaHeroes = false;
                    return;
                }

                // 处理图片下载等流程，然后逐个添加到列表里面
                foreach (var item in mapAllHeroes)
                {
                    item.Value.img = "https://cdn.cloudflare.steamstatic.com" + item.Value.img;
                    string attr = item.Value.primary_attr.ToLower();
                    if (attr.Contains("str"))
                    {
                        vStrHeroesList.Add(item.Value);
                    }
                    else if (attr.Contains("agi"))
                    {
                        vAgiHeroesList.Add(item.Value);
                    }
                    else if (attr.Contains("int"))
                    {
                        vIntHeroesList.Add(item.Value);
                    }
                }
            }
            catch { _bLoadedDotaHeroes = false; }
            finally { bLoadingHeroes = false; }
        }

        public async void PickHero(Models.DotaHeroModel selectedHero)
        {
            try
            {
                this.CurrentHero = selectedHero;
                var info = await ReqHeroInfo(this.CurrentHero.id);

                CurrentHeroInfo = info?.result?.data?.heroes?.Length > 0 ? info?.result?.data?.heroes[0] : null;

                if (CurrentHeroInfo == null)
                    bFailedHeroInfo = true;
                else
                    bFailedHeroInfo = false;

                ActPopInHeroInfoGrid?.Invoke();
                ActShowHeroInfoButton?.Invoke();
            }
            catch { bFailedHeroInfo = true; }
        }

        /// <summary>
        /// 请求英雄详细数据信息
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private async Task<Models.DotaHeroInfoModel> ReqHeroInfo(int heroId, string language = "english")
        {
            try
            {
                bLoadingHeroInfo = true;
                bFailedHeroInfo = false;

                if (dictHeroInfos.ContainsKey(heroId))
                {
                    await Task.Delay(600);
                    return dictHeroInfos[heroId];
                }

                string url = string.Format("https://www.dota2.com/datafeed/herodata?language={0}&hero_id={1}", language, heroId);

                try
                {
                    var response = await heroInfoHttpClient.GetAsync(new Uri(url));
                    string jsonMessage = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    };

                    var infoModel = JsonConvert.DeserializeObject<Models.DotaHeroInfoModel>(jsonMessage, jsonSerializerSettings);

                    if (infoModel != null)
                    {
                        dictHeroInfos.Add(heroId, infoModel);
                        return infoModel;
                    }
                }
                catch { }
            }
            catch { }
            finally { bLoadingHeroInfo = false; }
            return null;
        }

        public async void FetchHeroRanking(int heroId)
        {
            try
            {
                vRankingPlayers?.Clear();
                vRankingPlayers = new ObservableCollection<Models.RankingPlayer>();

                var ranking = await ReqHeroRanking(heroId);

                if (ranking == null || ranking.rankings == null || ranking.hero_id != this.CurrentHero.id.ToString())
                {
                    bFailedHeroInfo = true;
                }
                else
                {
                    bFailedHeroInfo = false;

                    int rank = 1;
                    foreach (var item in ranking.rankings)
                    {
                        try
                        {
                            item.iRank = rank;
                            rank++;
                            int dotIndex = item.score.IndexOf('.');
                            string score = dotIndex <= 0 ? item.score : item.score.Substring(0, dotIndex);
                            item.score = score;
                            vRankingPlayers.Add(item);
                        }
                        catch { }
                    }
                }
            }
            catch { bFailedHeroInfo = true; }
        }

        /// <summary>
        /// 请求英雄榜列表
        /// </summary>
        /// <param name="heroId"></param>
        private async Task<Models.DotaHeroRankingModel> ReqHeroRanking(int heroId)
        {
            try
            {
                bLoadingHeroRanking = true;
                bFailedHeroRanking = false;

                if (dictHeroRankings.ContainsKey(heroId))
                {
                    await Task.Delay(600);
                    return dictHeroRankings[heroId];
                }

                string url = string.Format("https://api.opendota.com/api/rankings?hero_id={0}", heroId);

                try
                {
                    var response = await heroRankingHttpClient.GetAsync(new Uri(url));
                    string jsonMessage = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    };

                    var rankingModel = JsonConvert.DeserializeObject<Models.DotaHeroRankingModel>(jsonMessage, jsonSerializerSettings);

                    if (rankingModel != null)
                    {
                        dictHeroRankings.Add(heroId, rankingModel);
                        return rankingModel;
                    }
                }
                catch { }
            }
            catch { }
            finally { bLoadingHeroRanking = false; }
            return null;
        }
    }
}
