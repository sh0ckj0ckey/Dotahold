using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
        private static string _currentSteamId = string.Empty;

        private readonly MainViewModel _viewModel;

        public OverviewPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                UpdateLayoutsWidth();

                if (string.IsNullOrWhiteSpace(_viewModel.AppSettings.SteamID))
                {
                    this.Frame.Navigate(typeof(ConnectPage));
                    this.Frame.ForwardStack.Clear();
                    this.Frame.BackStack.Clear();
                }
                else
                {
                    if (MatchesFrame.Content is null || _currentSteamId != _viewModel.AppSettings.SteamID)
                    {
                        MatchesFrame.Navigate(typeof(BlankPage), null, new DrillInNavigationTransitionInfo());
                        MatchesFrame.ForwardStack.Clear();
                        MatchesFrame.BackStack.Clear();

                        if (_currentSteamId != _viewModel.AppSettings.SteamID)
                        {
                            _currentSteamId = _viewModel.AppSettings.SteamID;
                            await _viewModel.ProfileViewModel.LoadPlayerOverview(_currentSteamId);
                        }
                    }
                }
            }
            catch (Exception ex) { LogCourier.Log($"OverviewPage Loaded error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private void PlayerOverviewGrid_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            UpdateLayoutsWidth();
        }

        private void UpdateLayoutsWidth()
        {
            PlayerRecentMatchesFlipView.Width = PlayerOverviewGrid.ActualWidth - 34;
            PlayerProfileGrid.Width = PlayerOverviewGrid.ActualWidth - 34;
            PlayerOverallScrollViewer.Width = PlayerOverviewGrid.ActualWidth - 2;
            PlayerHeroesScrollViewer.Width = PlayerOverviewGrid.ActualWidth - 2;
            PlayerRecentMatchesGrid.Width = PlayerOverviewGrid.ActualWidth - 34;
            PlayerCurrentNumberGrid.MaxWidth = PlayerOverviewGrid.ActualWidth - 34;
        }

        /// <summary>
        /// Visit player's Steam Profile website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VisitSteamProfileMenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var url = _viewModel.ProfileViewModel.PlayerProfile?.DotaPlayerProfile.profile?.profileurl;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
                }
            }
            catch (Exception ex) { LogCourier.Log($"VisitSteamProfileMenuFlyoutItem Click error: {ex.Message}", LogCourier.LogType.Error); }
        }

        /// <summary>
        /// Navigate to SteamConnectPage to change account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeAccountMenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                this.Frame.Navigate(typeof(ConnectPage));
            }
            catch (Exception ex) { LogCourier.Log($"ChangeAccountMenuFlyoutItem Click error: {ex.Message}", LogCourier.LogType.Error); }
        }

        /// <summary>
        /// Re-fetch player profile data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshProfileMenuFlyoutItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.AppSettings.SteamID))
                {
                    throw new InvalidOperationException("SteamID is not set.");
                }

                MatchesFrame.Navigate(typeof(BlankPage), null, new DrillInNavigationTransitionInfo());
                MatchesFrame.ForwardStack.Clear();
                MatchesFrame.BackStack.Clear();

                _currentSteamId = _viewModel.AppSettings.SteamID;
                await _viewModel.ProfileViewModel.LoadPlayerOverview(_currentSteamId);
            }
            catch (Exception ex) { LogCourier.Log($"RefreshProfileMenuFlyoutItem Click error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private void HeroesPlayedHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!Type.Equals(this.MatchesFrame.CurrentSourcePageType, typeof(HeroesPlayedPage)))
            {
                MatchesFrame.Navigate(typeof(HeroesPlayedPage));
            }
        }

        private void AllMatchesHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!Type.Equals(this.MatchesFrame.CurrentSourcePageType, typeof(MatchesPage)))
            {
                MatchesFrame.Navigate(typeof(MatchesPage));
            }
        }
    }
}
