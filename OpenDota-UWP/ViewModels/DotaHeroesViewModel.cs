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

        // 获取到英雄详情后触发动画
        public Action ActPopInHeroInfoGrid { get; set; } = null;

        // 是否已经成功加载过英雄列表
        private bool _bLoadedDotaHeroes = false;

        private Windows.Web.Http.HttpClient heroInfoHttpClient = new Windows.Web.Http.HttpClient();

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
            }
            catch { bFailedHeroInfo = true; }
        }

        private async Task<Models.DotaHeroInfoModel> ReqHeroInfo(int heroId, string language = "english")
        {
            try
            {
                if (dictHeroInfos.ContainsKey(heroId))
                    return dictHeroInfos[heroId];

                bLoadingHeroInfo = true;
                bFailedHeroInfo = false;

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
    }
}
