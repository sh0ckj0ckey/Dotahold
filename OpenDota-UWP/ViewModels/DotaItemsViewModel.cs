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

            // 处理图片下载等流程，然后逐个添加到 vAllItems 里面
            foreach (var item in items)
            {
                item.Value.img = "https://steamcdn-a.akamaihd.net/" + item.Value.img;
                vAllItems.Add(item.Value);
            }
        }

    }
}
