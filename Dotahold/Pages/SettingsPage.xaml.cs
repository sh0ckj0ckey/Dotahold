using System;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel? _viewModel;

        public SettingsPage()
        {
            this.InitializeComponent();

            this.Loaded += async (_, _) =>
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        long size = await ImageCourier.GetCacheSizeAsync();
                        string cacheSize = ConvertSize(size);

                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            CacheSizeTextBlock.Text = cacheSize;
                        });
                    });
                }
                catch { }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = e.Parameter as MainViewModel;
        }

        /// <summary>
        /// 滚动时渐入标题分割线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var verticalOffset = scrollViewer.VerticalOffset;
                var maxOffset = Math.Min(120, scrollViewer.ScrollableHeight);

                if (maxOffset <= 0) return;

                // 透明度按滚动比例变化，从全透明到不透明
                double newOpacity = verticalOffset / maxOffset;
                if (newOpacity > 1)
                {
                    newOpacity = 1;
                }
                if (newOpacity < 0)
                {
                    newOpacity = 0;
                }

                SettingsPageHeaderSeperatorLineBorder.Opacity = newOpacity;
            }
        }

        /// <summary>
        /// 前往应用商店评分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }

        /// <summary>
        /// 双击 Logo 显示开发者选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            DevelopmentHeaderTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DevelopmentSettingsCard.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// 清理缓存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClearCacheButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
                ClearCacheProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ClearCacheProgressRing.IsActive = true;
                CacheSizeTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                await Task.Delay(1000);
                bool isSuccessful = await ImageCourier.ClearCacheAsync();

                button.IsEnabled = true;
                ClearCacheProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ClearCacheProgressRing.IsActive = false;
                CacheSizeTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;

                if (isSuccessful)
                {
                    CacheSizeTextBlock.Text = "Cache cleared";
                }
            }
        }

        /// <summary>
        /// 前往 GitHub 提交问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GitHubIssueButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold/issues"));
        }

        /// <summary>
        /// 前往 Steam 个人主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SteamPageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
        }

        /// <summary>
        /// 前往 GitHub 项目页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GitHubPageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold"));
        }

        /// <summary>
        /// 打开数据文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataFolerButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await StorageFilesCourier.GetDataFolder();
            await Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// 打开图片缓存文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImageCacheFolderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await ImageCourier.GetCacheFolderAsync();
            await Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// 下次启动强制更新常量数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForceUpdateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConstantsCourier.ResetConstantsGottenDate();
        }

        private static string ConvertSize(long size)
        {
            if (size < 1024)
                return $"{size} B";
            else if (size < 1024 * 1024)
                return $"{Math.Round(size / 1024.0, 2)} KB";
            else if (size < 1024 * 1024 * 1024)
                return $"{Math.Round(size / (1024.0 * 1024), 2)} MB";
            else
                return $"{Math.Round(size / (1024.0 * 1024 * 1024), 2)} GB";
        }
    }
}
