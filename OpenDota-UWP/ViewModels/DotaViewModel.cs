using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OpenDota_UWP.ViewModels
{
    public class DotaViewModel : ViewModelBase
    {
        private static Lazy<DotaViewModel> _lazyVM = new Lazy<DotaViewModel>(() => new DotaViewModel());
        public static DotaViewModel Instance => _lazyVM.Value;

        // 当前选中的Tab: 0-Heroes 1-Items 2-Matches
        private int _iSideMenuTabIndex = 0;
        public int iSideMenuTabIndex
        {
            get { return _iSideMenuTabIndex; }
            set { Set("iSideMenuTabIndex", ref _iSideMenuTabIndex, value); }
        }

        // 应用程序的主题
        private ElementTheme _eAppTheme = ElementTheme.Light;
        public ElementTheme eAppTheme
        {
            get { return _eAppTheme; }
            set { Set("eAppTheme", ref _eAppTheme, value); }
        }

        // 设置的启动页面: 0-Heroes 1-Items 2-Matches
        private int _iStartupTabIndex = 0;
        public int iStartupTabIndex
        {
            get { return _iStartupTabIndex; }
            set
            {
                if (value > 2 || value < 0)
                {
                    return;
                }
                Set("iStartupTabIndex", ref _iStartupTabIndex, value);
            }
        }


        // 用来弹窗的样式
        private Style _customDialogStyle = null;

        public DotaViewModel()
        {
            try
            {
                LoadSettingContainer();
            }
            catch { }
        }

        public void LoadSettingContainer()
        {
            try
            {
                // 读取设置的应用程序主题
                try
                {
                    if (App.AppSettingContainer?.Values["Theme"] == null)
                    {
                        this.eAppTheme = ElementTheme.Dark;
                    }
                    else if (App.AppSettingContainer?.Values["Theme"]?.ToString() == "Light")
                    {
                        this.eAppTheme = ElementTheme.Light;
                    }
                    else if (App.AppSettingContainer?.Values["Theme"]?.ToString() == "Dark")
                    {
                        this.eAppTheme = ElementTheme.Dark;
                    }
                    else
                    {
                        this.eAppTheme = ElementTheme.Dark;
                    }
                }
                catch { }

                // 读取设置的启动页面
                try
                {
                    if (App.AppSettingContainer?.Values["StartupPage"] == null ||
                        App.AppSettingContainer?.Values["StartupPage"].ToString() == "0")
                    {
                        iStartupTabIndex = 0;
                    }
                    else if (App.AppSettingContainer?.Values["StartupPage"].ToString() == "1")
                    {
                        iStartupTabIndex = 1;
                    }
                    else if (App.AppSettingContainer?.Values["StartupPage"].ToString() == "2")
                    {
                        iStartupTabIndex = 2;
                    }
                    else
                    {
                        iStartupTabIndex = 0;
                    }
                }
                catch { }
            }
            catch { }
        }
    }
}
