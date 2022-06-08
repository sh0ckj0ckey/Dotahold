using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.ViewModels
{
    public class DotaViewModel : ViewModelBase
    {
        private static Lazy<DotaViewModel> _lazyVM = new Lazy<DotaViewModel>(() => new DotaViewModel());
        public static DotaViewModel Instance => _lazyVM.Value;

        // 当前选中的Tab: 0-Heroes 1-Items 2-Matches
        private int _iSideMenuTabIndex= 0;
        public int iSideMenuTabIndex
        {
            get { return _iSideMenuTabIndex; }
            set { Set("iSideMenuTabIndex", ref _iSideMenuTabIndex, value); }
        }
    }
}
