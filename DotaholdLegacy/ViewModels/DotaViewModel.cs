using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;

namespace Dotahold.ViewModels
{
    public class DotaViewModel : ObservableObject
    {
        private static Lazy<DotaViewModel> _lazyVM = new Lazy<DotaViewModel>(() => new DotaViewModel());
        public static DotaViewModel Instance => _lazyVM.Value;

        public SettingsCourier AppSettings { get; } = new SettingsCourier();

        private int _sideMenuTabIndex = 0;

        /// <summary>
        /// 当前选中的Tab: 0-Heroes 1-Items 2-Matches 3-Settings
        /// </summary>
        public int SideMenuTabIndex
        {
            get => _sideMenuTabIndex;
            set => SetProperty(ref _sideMenuTabIndex, value);
        }
    }
}
