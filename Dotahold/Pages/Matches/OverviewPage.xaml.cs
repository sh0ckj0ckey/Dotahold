using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
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
                if (string.IsNullOrWhiteSpace(_viewModel.AppSettings.SteamID))
                {
                    if (!Type.Equals(this.Frame.CurrentSourcePageType, typeof(SteamConnectPage)))
                    {
                        this.Frame.Navigate(typeof(SteamConnectPage));
                        this.Frame.BackStack.Clear();
                    }
                }
                else if (_viewModel.ProfileViewModel.PlayerProfile is null)
                {
                    await _viewModel.ProfileViewModel.LoadPlayerProfile(_viewModel.AppSettings.SteamID);
                    await _viewModel.ProfileViewModel.LoadPlayerOverview(_viewModel.AppSettings.SteamID);
                }
            }
            catch (Exception ex) { LogCourier.Log($"OverviewPage Loaded error: {ex.Message}", LogCourier.LogType.Error); }
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
                this.Frame.Navigate(typeof(SteamConnectPage));
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
                await _viewModel.ProfileViewModel.LoadPlayerProfile(_viewModel.AppSettings.SteamID);
                await _viewModel.ProfileViewModel.LoadPlayerOverview(_viewModel.AppSettings.SteamID);
            }
            catch (Exception ex) { LogCourier.Log($"RefreshProfileMenuFlyoutItem Click error: {ex.Message}", LogCourier.LogType.Error); }
        }
    }
}
