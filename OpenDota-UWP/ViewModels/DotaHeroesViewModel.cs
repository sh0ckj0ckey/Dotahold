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

        // 是否正在加载英雄列表
        private bool _bLoadingHeroes = false;
        public bool bLoadingHeroes
        {
            get { return _bLoadingHeroes; }
            set { Set("bLoadingHeroes", ref _bLoadingHeroes, value); }
        }

        // 是否正在加载英雄详情
        private bool _bLoadingHeroeInfo = false;
        public bool bLoadingHeroeInfo
        {
            get { return _bLoadingHeroeInfo; }
            set { Set("bLoadingHeroeInfo", ref _bLoadingHeroeInfo, value); }
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

        // 是否已经成功加载过英雄列表
        private bool _bLoadedDotaHeroes = false;

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

        public void PickHero(Models.DotaHeroModel selectedHero)
        {
            try
            {
                this.CurrentHero = selectedHero;
                ReqHeroInfo(this.CurrentHero.id);
            }
            catch { }
        }

        private void ReqHeroInfo(int heroId)
        {
            try
            {

            }
            catch { }
        }
    }
}
