using Dotahold.Core.DataShop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Dotahold.ViewModels;
using Windows.ApplicationModel;
using Dotahold.Core.Utils;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private DotaViewModel ViewModel = null;
        private string _appVersion = string.Empty;
        private long _lastTimeCleanCache = 0;

        public SettingPage()
        {
            ViewModel = DotaViewModel.Instance;
            this.InitializeComponent();

            _appVersion = AppVersionUtils.GetAppVersion();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (DateTime.Now.Ticks - _lastTimeCleanCache > TimeSpan.FromSeconds(5).Ticks)
                {
                    _lastTimeCleanCache = DateTime.Now.Ticks;
                    DotaViewModel.Instance.GetImageCacheSize();
                }
            }
            catch { }
        }

        /// <summary>
        /// 双击打开调试选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDotaholdImageDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                DotaViewModel.Instance.bShowDevTools = true;
            }
            catch { }
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
            catch { }
        }

        /// <summary>
        /// 切换黑白模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppearanceSelectiongChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DotaViewModel.Instance.ActChangeTitleBarTheme?.Invoke();
                if (Window.Current.Content is FrameworkElement rootElement && sender is ComboBox cb)
                {
                    rootElement.RequestedTheme = cb.SelectedIndex == 1 ? ElementTheme.Light : ElementTheme.Dark;
                }
            }
            catch { }
        }

        /// <summary>
        /// 切换到白天主题
        /// </summary>
        public void Switch2LightMode()
        {
            try
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedForegroundColor = Colors.Black;
            }
            catch { }
        }

        /// <summary>
        /// 切换到夜间主题
        /// </summary>
        public void Switch2DarkMode()
        {
            try
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedForegroundColor = Colors.White;
            }
            catch { }
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickClearImageCache(object sender, RoutedEventArgs e)
        {
            try
            {
                DotaViewModel.Instance.ClearImageCache();
                _lastTimeCleanCache = 0;
            }
            catch { }
        }

        // 打开配置文件保存目录
        private async void OnClickOpenDataDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await StorageFilesCourier.GetDataFolder();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch { }
        }

        // 打开图片缓存目录
        private async void OnClickOpenImageCacheDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await ImageCourier.GetCacheFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch { }
        }

        // 强制下次更新常量数据
        private void OnClickForceUpdateLocalJson(object sender, RoutedEventArgs e)
        {
            try
            {
                ConstantsCourier.Instance.ResetConstantsGottenDate();
            }
            catch { }
        }

        // 访问 GitHub Issues
        private async void OnClickGitHubIssue(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold/issues"));
            }
            catch { }
        }

        // 访问 GitHub
        private async void OnClickGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold"));
            }
            catch { }
        }

        // 访问 Steam 个人页面
        private async void OnClickSteamProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
            }
            catch { }
        }
    }
}
