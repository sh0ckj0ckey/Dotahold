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

        public ObservableCollection<Models.DotaItemModel> vAllItems { get; set; } = new ObservableCollection<Models.DotaItemModel>();


        public DotaItemsViewModel()
        {
            InitialDotaItems();
        }

        public async void InitialDotaItems()
        {
            var items = await ConstantsHelper.Instance.GetItemsConstant();
            ObservableCollection<Models.DotaItemModel> dotaItems = new ObservableCollection<Models.DotaItemModel>();
            // 处理图片下载等流程，然后逐个添加到 vAllItems 里面
            foreach (var item in items)
            {
                item.Value.img = "https://steamcdn-a.akamaihd.net/" + item.Value.img;
                dotaItems.Add(item.Value);
            }
            var temp = dotaItems.OrderBy(item => item.id);
            vAllItems.Clear();
            foreach (var item in temp)
            {
                vAllItems.Add(item);
            }
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
