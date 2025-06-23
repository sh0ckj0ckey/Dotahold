using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ConnectPage : Page
    {
        [GeneratedRegex(@"^\d+$")]
        private static partial Regex SteamIdRegex();

        private static bool _loadedPlayerConnectRecords = false;

        private readonly MainViewModel _viewModel;

        private CancellationTokenSource? _cancellationTokenSource;

        public ConnectPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SteamIdTextBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);

            if (!_loadedPlayerConnectRecords)
            {
                _loadedPlayerConnectRecords = true;
                await _viewModel.ProfileViewModel.PlayerConnectRecords.LoadPlayerConnectRecords();
            }
        }

        private void Page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlayerConnectRecordsItemsRepeater.ItemsSource = null;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task ConnectSteamId(string steamId)
        {
            try
            {
                ConnectHyperlinkButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ConnectingStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ConnectingProgressRing.IsActive = true;

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

                if (steamId.Length > 14)
                {
                    if (decimal.TryParse(steamId, out decimal id64))
                    {
                        steamId = (id64 - 76561197960265728).ToString();
                    }
                    else
                    {
                        ConnectionFailedInfoBar.Message = "Invalid Dota 2 ID. Please check and try again.";
                        ConnectionFailedInfoBar.IsOpen = true;
                        return;
                    }
                }

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource?.Token ?? CancellationToken.None;

                var profile = await ApiCourier.GetPlayerProfile(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (profile?.profile is null)
                {
                    ConnectionFailedInfoBar.Message = "Failed to retrieve player profile. Please check the Dota 2 ID and try again.";
                    ConnectionFailedInfoBar.IsOpen = true;
                    return;
                }

                _viewModel.AppSettings.SteamID = profile.profile.account_id;
                _viewModel.ProfileViewModel.PlayerConnectRecords.RecordPlayerConnect(profile.profile.avatarfull, profile.profile.personaname, profile.profile.account_id);

                if (!Type.Equals(this.Frame.CurrentSourcePageType, typeof(OverviewPage)))
                {
                    this.Frame.Navigate(typeof(OverviewPage));
                    this.Frame.BackStack.Clear();
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
            finally
            {
                if (ConnectHyperlinkButton is not null)
                {
                    ConnectHyperlinkButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                if (ConnectingStackPanel is not null)
                {
                    ConnectingStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                if (ConnectingProgressRing is not null)
                {
                    ConnectingProgressRing.IsActive = false;
                }
            }
        }

        private async void ConnectHyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ConnectSteamId(SteamIdTextBox.Text);
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ConnectSteamId((sender as Button)?.Tag?.ToString() ?? string.Empty);
        }

        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                this.Frame.ForwardStack.Clear();
                this.Frame.BackStack.Clear();
            }
            else
            {
                this.Frame.Navigate(typeof(OverviewPage));
                this.Frame.ForwardStack.Clear();
                this.Frame.BackStack.Clear();
            }
        }
    }
}
