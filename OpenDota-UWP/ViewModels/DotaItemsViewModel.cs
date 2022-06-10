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
        public Dictionary<string, Models.DotaItemModel> mapAllItems { get; set; } = new Dictionary<string, Models.DotaItemModel>();
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

        public DotaItemsViewModel()
        {
            InitialDotaItems();
        }

        private async void InitialDotaItems()
        {
            try
            {
                bLoadingItems = true;

                mapAllItems?.Clear();
                vAllItems?.Clear();
                vItemsList?.Clear();
                mapAllItems = await ConstantsHelper.Instance.GetItemsConstant();
                // 处理图片下载等流程，然后逐个添加到 vAllItems 里面
                foreach (var item in mapAllItems)
                {
                    item.Value.img = "https://cdn.cloudflare.steamstatic.com" + item.Value.img;
                    vAllItems.Add(item.Value);
                    vItemsList.Add(item.Value);
                }
            }
            catch { }
            finally { bLoadingItems = false; }
        }

        ///// <summary>
        ///// 通过API获取物品信息
        ///// </summary>
        ///// <returns></returns>
        //public async Task<string> GetItemDataAsync()
        //{
        //    string url = "http://www.dota2.com.cn/items/json";
        //    Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
        //    var response = await http.GetAsync(new Uri(url));
        //    var jsonMessage = await response.Content.ReadAsStringAsync();
        //    return jsonMessage;
        //}

        //public async Task<string> GetItemDataENAsync()
        //{
        //    string url = String.Format("http://www.dota2.com/jsfeed/itemdata");
        //    Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
        //    var response = await http.GetAsync(new Uri(url));
        //    var jsonMessage = await response.Content.ReadAsStringAsync();
        //    return jsonMessage;
        //}

    }
}
