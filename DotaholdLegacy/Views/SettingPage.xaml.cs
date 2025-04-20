using System;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private DotaViewModel _viewModel = null;

        private DateTime _lastTimeCheckCacheSize = new DateTime(1970, 1, 1);

        public SettingPage()
        {
            _viewModel = DotaViewModel.Instance;

            this.InitializeComponent();

            this.Loaded += async (s, e) =>
            {
                try
                {
                    // 获取版本号
                    string version = GetAppVersion();
                    VersionTextBlock.Text = version;

                    // 获取图片缓存大小
                    if (DateTime.Now - _lastTimeCheckCacheSize > TimeSpan.FromSeconds(3))
                    {
                        _lastTimeCheckCacheSize = DateTime.Now;
                        string size = await GetImageCacheSize();

                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            };
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        private static string GetAppVersion()
        {
            try
            {
                PackageVersion version = Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

            return "";
        }

        #region Image Cache

        /// <summary>
        /// 获取图片缓存的大小
        /// </summary>
        private async Task<string> GetImageCacheSize()
        {
            long size = await ImageCourier.GetCacheSizeAsync();
            string cacheSize = ConvertSize(size);
            return cacheSize;
        }

        /// <summary>
        /// 清理图片缓存
        /// </summary>
        public async void ClearImageCache()
        {
            try
            {
                bCleaningImageCache = true;
                bool result = await ImageCourier.ClearCacheAsync();

                if (result) GetImageCacheSize();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            finally { bCleaningImageCache = false; }
        }

        private string ConvertSize(long size)
        {
            try
            {
                if (size / (1024 * 1024 * 1024) >= 1)
                {
                    return $"{Math.Round(size / (float)(1024 * 1024 * 1024), 2)}GB";
                }
                else if (size / (1024 * 1024) >= 1)
                {
                    return $"{Math.Round(size / (float)(1024 * 1024), 2)}MB";
                }
                else if (size / 1024 >= 1)
                {
                    return $"{Math.Round(size / (float)1024, 2)}KB";
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }

            return $"{size}B";
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
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

        #region Dev Tools

        /// <summary>
        /// 双击打开调试选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDotaholdImageDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                DebugToolsHeader.Visibility = Visibility.Visible;
                DebugToolsGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 打开配置文件保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickOpenDataDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await StorageFilesCourier.GetDataFolder();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 打开图片缓存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickOpenImageCacheDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await ImageCourier.GetCacheFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 强制下次更新常量数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickForceUpdateLocalJson(object sender, RoutedEventArgs e)
        {
            try
            {
                ConstantsCourier.Instance.ResetConstantsGottenDate();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

        #region Contact Me

        /// <summary>
        /// 访问 GitHub Issues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGitHubIssue(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold/issues"));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold"));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 访问 Steam 个人页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickSteamProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

    }
}
