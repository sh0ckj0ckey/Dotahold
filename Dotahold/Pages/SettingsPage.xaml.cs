using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.ApplicationModel;
using Windows.System;
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
                    static string ConvertSize(long size)
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

                    long size = await ImageCourier.GetCacheSizeAsync();
                    string cacheSize = ConvertSize(size);
                    CacheSizeTextBlock.Text = cacheSize;
                }
                catch { }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = e.Parameter as MainViewModel;
        }

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

        private async void ClearCacheButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
                ClearCacheProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ClearCacheProgressRing.IsActive = true;
                CacheSizeTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

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

        private async void RateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }

        private void Image_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            DevelopmentHeaderTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DevelopmentSettingsCard.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void GitHubIssueButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold/issues"));
        }

        private async void SteamPageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
        }

        private async void GitHubPageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold"));
        }

        private async void DataFolerButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await StorageFilesCourier.GetDataFolder();
            await Launcher.LaunchFolderAsync(folder);
        }

        private async void ImageCacheFolderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await ImageCourier.GetCacheFolderAsync();
            await Launcher.LaunchFolderAsync(folder);
        }

        private void ForceUpdateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConstantsCourier.ResetConstantsGottenDate();
        }
    }
}
