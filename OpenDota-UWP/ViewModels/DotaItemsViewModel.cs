using OpenDota_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.ViewModels
{
    public class DotaItemsViewModel : ViewModelBase
    {
        private static Lazy<DotaItemsViewModel> _lazyVM = new Lazy<DotaItemsViewModel>(() => new DotaItemsViewModel());
        public static DotaItemsViewModel Instance => _lazyVM.Value;

        // 所有物品
        public Dictionary<string, Models.DotaItemModel> dictAllItems { get; set; } = new Dictionary<string, Models.DotaItemModel>();
        public List<Models.DotaItemModel> vAllItems { get; set; } = new List<Models.DotaItemModel>();

        // 列表展示的物品
        public ObservableCollection<Models.DotaItemModel> vItemsList { get; set; } = new ObservableCollection<Models.DotaItemModel>();

        // 是否正在加载物品列表
        private bool _bLoadingItems = false;
        public bool bLoadingItems
        {
            get { return _bLoadingItems; }
            set { Set("bLoadingItems", ref _bLoadingItems, value); }
        }

        // 当前选中的物品
        private Models.DotaItemModel _CurrentItem = null;
        public Models.DotaItemModel CurrentItem
        {
            get { return _CurrentItem; }
            set { Set("CurrentItem", ref _CurrentItem, value); }
        }

        // 是否已经成功加载过装备列表
        private bool _bLoadedDotaItems = false;

        public DotaItemsViewModel()
        {
            //LoadDotaItems();
        }

        // 固定返回true，用来等待返回值的
        public async Task<bool> LoadDotaItems()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading Items ---> " + DateTime.Now.Ticks);

                if (_bLoadedDotaItems)
                    return true;

                _bLoadedDotaItems = true;

                bLoadingItems = true;

                dictAllItems?.Clear();
                vAllItems?.Clear();
                vItemsList?.Clear();
                dictAllItems = await ConstantsHelper.Instance.GetItemsConstant();

                if (dictAllItems == null || dictAllItems.Count <= 0)
                {
                    bLoadingItems = false;
                    _bLoadedDotaItems = false;
                    return true;
                }

                // 处理图片下载等流程，然后逐个添加到 vAllItems 里面
                foreach (var item in dictAllItems)
                {
                    try
                    {
                        item.Value.img = "https://cdn.cloudflare.steamstatic.com" + item.Value.img;

                        if (item.Value.cost != null)
                        {
                            var cost = item.Value.cost.ToString().ToLower();
                            item.Value.cost = (cost == "false" || cost == "true") ? "0" : cost;
                        }
                        else
                        {
                            item.Value.cost = "0";
                        }

                        if (item.Value.cd != null)
                        {
                            var cd = item.Value.cd.ToString().ToLower();
                            item.Value.cd = (cd == "false" || cd == "true") ? "0" : cd;
                        }
                        else
                        {
                            item.Value.cd = "0";
                        }

                        if (item.Value.tier != null)
                        {
                            var tier = item.Value.tier.ToString().ToLower();
                            item.Value.tier = (tier == "false" || tier == "true") ? "0" : tier;
                        }
                        else
                        {
                            item.Value.tier = "0";
                        }

                        if (item.Value.mc != null)
                        {
                            var mc = item.Value.mc.ToString().ToLower();
                            item.Value.mc = (mc == "false" || mc == "true") ? "0" : mc;
                        }
                        else
                        {
                            item.Value.mc = "0";
                        }

                        vAllItems.Add(item.Value);
                        vItemsList.Add(item.Value);
                    }
                    catch { }
                }

            }
            catch { _bLoadedDotaItems = false; }
            finally { bLoadingItems = false; }
            return true;
        }
    }
}
