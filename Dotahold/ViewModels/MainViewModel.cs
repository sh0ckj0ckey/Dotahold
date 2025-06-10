using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;

namespace Dotahold.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public SettingsCourier AppSettings { get; private set; }

        public HeroesViewModel HeroesViewModel { get; private set; }

        public ItemsViewModel ItemsViewModel { get; private set; }

        public MatchesViewModel MatchesViewModel { get; private set; }

        public ProfileViewModel ProfileViewModel { get; private set; }

        private int _tabIndex = 0;

        private string _appVersion = string.Empty;

        /// <summary>
        /// 当前选中的Tab索引, 0-Heroes 1-Items 2-Matches 3-Settings
        /// </summary>
        public int TabIndex
        {
            get => _tabIndex;
            set => SetProperty(ref _tabIndex, value);
        }

        /// <summary>
        /// 应用程序版本号
        /// </summary>
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appVersion))
                {
                    _appVersion = Utils.AppVersionUtil.GetAppVersion();
                }

                return _appVersion;
            }
        }

        public MainViewModel()
        {
            this.AppSettings = new SettingsCourier();
            this.HeroesViewModel = new HeroesViewModel();
            this.ItemsViewModel = new ItemsViewModel();
            this.MatchesViewModel = new MatchesViewModel(this.HeroesViewModel, this.ItemsViewModel);
            this.ProfileViewModel = new ProfileViewModel(this.HeroesViewModel, this.ItemsViewModel, this.MatchesViewModel);
        }
    }
}
