using Dotahold.Core.DataShop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dotahold.ViewModels
{
    public class DotaItemsViewModel : ViewModelBase
    {
        private static Lazy<DotaItemsViewModel> _lazyVM = new Lazy<DotaItemsViewModel>(() => new DotaItemsViewModel());
        public static DotaItemsViewModel Instance => _lazyVM.Value;

        // 所有物品
        public Dictionary<string, Core.Models.DotaItemModel> dictNameToAllItems { get; set; } = new Dictionary<string, Core.Models.DotaItemModel>();
        public Dictionary<string, Core.Models.DotaItemModel> dictIdToAllItems { get; set; } = new Dictionary<string, Core.Models.DotaItemModel>();

        private List<Core.Models.DotaItemModel> _vAllItems { get; set; } = new List<Core.Models.DotaItemModel>();

        // 列表展示的物品
        public ObservableCollection<Core.Models.DotaItemModel> vAllShowItemsList { get; set; } = new ObservableCollection<Core.Models.DotaItemModel>();

        // 搜索时展示的物品
        public ObservableCollection<Core.Models.DotaItemModel> vSearchItemsList { get; set; } = new ObservableCollection<Core.Models.DotaItemModel>();

        // 是否正在加载物品列表
        private bool _bLoadingItems = false;
        public bool bLoadingItems
        {
            get { return _bLoadingItems; }
            set { Set("bLoadingItems", ref _bLoadingItems, value); }
        }

        // 当前选中的物品
        private Core.Models.DotaItemModel _CurrentItem = null;
        public Core.Models.DotaItemModel CurrentItem
        {
            get { return _CurrentItem; }
            set { Set("CurrentItem", ref _CurrentItem, value); }
        }

        // 是否已经成功加载过装备列表
        private bool _bLoadedDotaItems = false;

        // 是否正在搜索，搜索时显示vSearchItemsList，否则显示vAllShowItemsList
        private bool _bSearchingItems = false;
        public bool bSearchingItems
        {
            get { return _bSearchingItems; }
            set { Set("bSearchingItems", ref _bSearchingItems, value); }
        }

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

                bSearchingItems = false;

                dictNameToAllItems?.Clear();
                _vAllItems?.Clear();
                vAllShowItemsList?.Clear();
                vSearchItemsList?.Clear();

                dictNameToAllItems = await ConstantsCourier.Instance.GetItemsConstant();

                if (dictNameToAllItems == null || dictNameToAllItems.Count <= 0)
                {
                    _bLoadedDotaItems = false;
                    return true;
                }

                foreach (var dictItem in dictNameToAllItems)
                {
                    try
                    {
                        var item = dictItem.Value;

                        item.img = "https://cdn.cloudflare.steamstatic.com" + item.img;
                        if (!string.IsNullOrEmpty(item.cost))
                        {
                            string cost = item.cost.ToLower();
                            item.cost = (cost == "false" || cost == "true" || cost == "null") ? "0" : cost;
                        }
                        else
                        {
                            item.cost = "0";
                        }

                        if (!string.IsNullOrEmpty(item.cd))
                        {
                            string cd = item.cd.ToLower();
                            item.cd = (cd == "false" || cd == "true" || cd == "null") ? "0" : cd;
                        }
                        else
                        {
                            item.cd = "0";
                        }

                        if (!string.IsNullOrEmpty(item.tier))
                        {
                            string tier = item.tier.ToLower();
                            item.tier = (tier == "false" || tier == "true" || tier == "null") ? "0" : tier;
                        }
                        else
                        {
                            item.tier = "0";
                        }

                        if (!string.IsNullOrEmpty(item.mc))
                        {
                            string mc = item.mc.ToLower();
                            item.mc = (mc == "false" || mc == "true" || mc == "null") ? "0" : mc;
                        }
                        else
                        {
                            item.mc = "0";
                        }
                        _vAllItems.Add(item);

                        if (!string.IsNullOrEmpty(item.id) && item.id != "0")
                        {
                            dictIdToAllItems.Add(item.id, item);
                        }
                    }
                    catch { }
                }

                bLoadingItems = false;

                foreach (var item in _vAllItems)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.qual) || (!string.IsNullOrEmpty(item.tier) && item.tier != "0"))
                        {
                            vAllShowItemsList.Add(item);
                        }
                    }
                    catch { }
                }

                foreach (var item in _vAllItems)
                {
                    await item.LoadImageAsync(85);
                }
            }
            catch { _bLoadedDotaItems = false; }
            finally { bLoadingItems = false; }
            return true;
        }

        // 搜索
        public void SearchItems(string search)
        {
            try
            {
                vSearchItemsList?.Clear();
                search = search.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    bSearchingItems = true;

                    bool searchFuzzy = DotaViewModel.Instance.bSearchFuzzy;
                    if (searchFuzzy)
                    {
                        // 模糊匹配搜索
                        StringBuilder sb = new StringBuilder();
                        sb.Append(".*");
                        foreach (var item in search)
                        {
                            sb.Append(item);
                            sb.Append(".*");
                        }
                        foreach (var item in _vAllItems)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.dname) && Regex.IsMatch(item.dname.ToLower(), sb.ToString().ToLower()))
                            {
                                vSearchItemsList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        // 全字匹配搜索
                        foreach (var item in _vAllItems)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.dname) && item.dname.ToLower().Contains(search.ToLower()))
                            {
                                vSearchItemsList.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    bSearchingItems = false;
                }
            }
            catch { }
        }
    }
}
