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

        // 英雄属性Tab
        private int _iHeroAttrTabIndex = 0;
        public int iHeroAttrTabIndex
        {
            get { return _iHeroAttrTabIndex; }
            set { Set("iHeroAttrTabIndex", ref _iHeroAttrTabIndex, value); }
        }

        public DotaHeroesViewModel()
        {
            InitialDotaHeroes();
        }

        private async void InitialDotaHeroes()
        {
            try
            {
                bLoadingHeroes = true;

                mapAllHeroes?.Clear();
                vStrHeroesList?.Clear();
                vAgiHeroesList?.Clear();
                vIntHeroesList?.Clear();
                mapAllHeroes = await ConstantsHelper.Instance.GetHeroesConstant();
                // 处理图片下载等流程，然后逐个添加到 vAllItems 里面
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
            catch { }
            finally { bLoadingHeroes = false; }
        }
    }
}
