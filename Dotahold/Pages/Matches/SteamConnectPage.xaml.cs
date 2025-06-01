using System;
using System.Text.RegularExpressions;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SteamConnectPage : Page
    {
        private readonly MainViewModel _viewModel;

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex SteamIdRegex();

        public SteamConnectPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _viewModel.ProfileViewModel.LoadPlayerConnectRecords();
        }

        private async void ConnectSteamId(string steamId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(steamId))
                {
                    ConnectionFailedInfoBar.Message = "Dota 2 ID cannot be empty.";
                    ConnectionFailedInfoBar.IsOpen = true;
                    return;
                }

                if (!SteamIdRegex().IsMatch(steamId))
                {
                    ConnectionFailedInfoBar.Message = "Invalid Dota 2 ID format. Please enter a valid numeric ID.";
                    ConnectionFailedInfoBar.IsOpen = true;
                    return;
                }

                bool success = await _viewModel.ProfileViewModel.GetPlayerProfile(steamId);

                if (!success || _viewModel.ProfileViewModel.PlayerProfile?.DotaPlayerProfile.profile is null)
                {
                    ConnectionFailedInfoBar.Message = "Failed to retrieve player profile. Please check the Dota 2 ID and try again.";
                    ConnectionFailedInfoBar.IsOpen = true;
                    return;
                }

                _viewModel.AppSettings.SteamID = _viewModel.ProfileViewModel.PlayerProfile.DotaPlayerProfile.profile?.account_id ?? string.Empty;

                if (!Type.Equals(this.Frame.CurrentSourcePageType, typeof(OverviewPage)))
                {
                    this.Frame.Navigate(typeof(OverviewPage));
                    this.Frame.BackStack.Clear();
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        private void ConnectHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectSteamId(SteamIdTextBox.Text);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectSteamId((sender as Button)?.Tag?.ToString() ?? string.Empty);
        }

        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(OverviewPage));
                this.Frame.BackStack.Clear();
            }
        }
    }
}
