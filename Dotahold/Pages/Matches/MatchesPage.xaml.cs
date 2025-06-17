using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchesPage : Page
    {
        private readonly MainViewModel _viewModel;

        public MatchesPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
            };

            this.Unloaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (e.NavigationMode == NavigationMode.New && !string.IsNullOrWhiteSpace(_viewModel.AppSettings.SteamID))
                {
                    await _viewModel.MatchesViewModel.LoadPlayerAllMatches(_viewModel.AppSettings.SteamID);
                    _viewModel.MatchesViewModel.LoadMoreMatches();
                }
            }
            catch (Exception ex) { LogCourier.Log($"MatchesPage Loaded error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                if (sender is ScrollViewer scrollViewer)
                {
                    var verticalOffset = scrollViewer.VerticalOffset;
                    var maxOffset = Math.Min(120, scrollViewer.ScrollableHeight);

                    if (maxOffset <= 0) return;

                    if (!e.IsIntermediate)
                    {
                        var scroller = (ScrollViewer)sender;
                        var distanceToEnd = scroller.ExtentHeight - (scroller.VerticalOffset + scroller.ViewportHeight);

                        if (distanceToEnd <= 60)
                        {
                            _viewModel.MatchesViewModel.LoadMoreMatches();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        #region GoBack

        private bool TryGoBack()
        {
            this.Frame.Navigate(typeof(BlankPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            this.Frame.ForwardStack.Clear();
            this.Frame.BackStack.Clear();
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
