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

        public ObservableCollection<Models.DotaItems> AllItems { get; set; } = new ObservableCollection<Models.DotaItems>();


        public DotaItemsViewModel()
        {
            InitialDotaItems();
        }

        public void InitialDotaItems()
        {
            ConstantsHelper.Instance.
        }

    }
}
