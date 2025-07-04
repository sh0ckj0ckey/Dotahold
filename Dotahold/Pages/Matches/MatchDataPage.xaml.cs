using System;
using System.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchDataPage : Page
    {
        private readonly MainViewModel _viewModel;

        private INotifyPropertyChanged? _registeredNpc;

        public MatchDataPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                UpdateLayoutsWidth();

                if (_viewModel?.MatchesViewModel is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += MatchesViewModel_PropertyChanged;
                    _registeredNpc = npc;
                }

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
            };

            this.Unloaded += (_, _) =>
            {
                if (_registeredNpc is not null)
                {
                    _registeredNpc.PropertyChanged -= MatchesViewModel_PropertyChanged;
                    _registeredNpc = null;
                }

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        private void MatchesViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(_viewModel.MatchesViewModel.SelectedMatchData))
                {
                    MatchDataScrollViewer?.ScrollToVerticalOffset(0);
                    MatchDataOverviewScrollViewer?.ScrollToHorizontalOffset(0);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error in MatchesViewModel_PropertyChanged: {ex.Message}");
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutsWidth();
        }

        private void UpdateLayoutsWidth()
        {
            if (LeagueNameGrid is not null)
            {
                LeagueNameGrid.Width = RootGrid.ActualWidth - 32;
            }

            DataNotParsedInfoBar.Width = RootGrid.ActualWidth - 32;
            MatchDataOverviewScrollViewer.Width = RootGrid.ActualWidth;
            RadiantPlayersHeaderGrid.Width = RootGrid.ActualWidth;
            RadiantPlayersGrid.Width = RootGrid.ActualWidth - 32;
            DirePlayersHeaderGrid.Width = RootGrid.ActualWidth;
            DirePlayersGrid.Width = RootGrid.ActualWidth - 32;
            MatchIdAndDateGrid.Width = RootGrid.ActualWidth;
            VisitOpenDotaButton.Width = RootGrid.ActualWidth - 32;
        }

        private async void VisitOpenDotaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.MatchesViewModel.SelectedMatchData?.DotaMatchData is not null)
                {
                    string url = $"https://www.opendota.com/matches/{_viewModel.MatchesViewModel.SelectedMatchData.DotaMatchData.match_id}";
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
                }
            }
            catch (Exception ex) { LogCourier.Log($"VisitSteamProfileMenuFlyoutItem Click error: {ex.Message}", LogCourier.LogType.Error); }

        }

        #region GoBack

        private bool TryGoBack()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(BlankPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom });
                this.Frame.ForwardStack.Clear();
                this.Frame.BackStack.Clear();
            }

            return true;
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object? sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TryGoBack();
        }

        #endregion
    }
}
