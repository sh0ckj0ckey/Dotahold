using System;
using System.Text.RegularExpressions;
using System.Threading;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    internal sealed partial class MatchSearchView : UserControl
    {
        [GeneratedRegex(@"^\d+$")]
        private static partial Regex MatchIdRegex();

        private readonly Action _hideDialogContent;

        private readonly MatchesViewModel _viewModel;

        private string _matchId = string.Empty;

        private CancellationTokenSource? _cancellationTokenSource;

        internal MatchSearchView(MatchesViewModel matchesViewModel, Action hideContentDialog)
        {
            _viewModel = matchesViewModel;
            _hideDialogContent = hideContentDialog;

            this.Loaded += (_, _) =>
            {
                GoToNormalState();
            };

            this.Unloaded += (_, _) =>
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            };

            this.InitializeComponent();
        }

        public string GetMatchId() => _matchId;

        private void MatchIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (ErrorTextBlock.Visibility == Visibility.Visible)
                {
                    GoToNormalState();
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        private async void SearchHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GoToSearchingState();

                string matchId = MatchIdTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(matchId))
                {
                    GoToErrorState("Match ID cannot be empty");
                    return;
                }

                if (!MatchIdRegex().IsMatch(matchId))
                {
                    GoToErrorState("Invalid Match ID format");
                    return;
                }

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource?.Token ?? CancellationToken.None;

                var verified = await _viewModel.VerifyMatchId(matchId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (verified)
                {
                    _matchId = matchId;
                    _hideDialogContent.Invoke();
                }
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _matchId = string.Empty;
            _hideDialogContent.Invoke();
        }

        private void GoToNormalState()
        {
            try
            {
                MatchIdTextBox.IsEnabled = true;
                MatchIdTextBox.Focus(FocusState.Keyboard);

                SearchHyperlinkButton.Visibility = Visibility.Visible;

                SearchingStackPanel.Visibility = Visibility.Collapsed;
                SearchingProgressRing.IsActive = false;

                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Text = "";
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        private void GoToSearchingState()
        {
            try
            {
                MatchIdTextBox.IsEnabled = false;

                SearchHyperlinkButton.Visibility = Visibility.Collapsed;

                SearchingStackPanel.Visibility = Visibility.Visible;
                SearchingProgressRing.IsActive = true;

                ErrorTextBlock.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Text = "";
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }

        private void GoToErrorState(string errorMessage)
        {
            try
            {
                MatchIdTextBox.IsEnabled = true;
                MatchIdTextBox.SelectAll();

                SearchHyperlinkButton.Visibility = Visibility.Collapsed;

                SearchingStackPanel.Visibility = Visibility.Collapsed;
                SearchingProgressRing.IsActive = false;

                ErrorTextBlock.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = errorMessage;
            }
            catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
