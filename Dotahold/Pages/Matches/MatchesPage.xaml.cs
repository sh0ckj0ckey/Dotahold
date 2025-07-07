using System;
using System.Collections.Specialized;
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
    public sealed partial class MatchesPage : Page
    {
        private readonly MainViewModel _viewModel;

        public MatchesPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                _viewModel.MatchesViewModel.Matches.CollectionChanged += Matches_CollectionChanged;

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;

            };

            this.Unloaded += (_, _) =>
            {
                _viewModel.MatchesViewModel.Matches.CollectionChanged -= Matches_CollectionChanged;

                MatchesItemsRepeater.ItemsSource = null;

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        private void Matches_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                MatchesScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                if (sender is ScrollViewer scrollViewer)
                {
                    if (!e.IsIntermediate)
                    {
                        var distanceToEnd = scrollViewer.ExtentHeight - (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight);

                        if (distanceToEnd <= 60)
                        {
                            _viewModel.MatchesViewModel.LoadMoreMatches(20, _viewModel.MatchesViewModel.MatchesHeroFilter?.DotaHeroAttributes.id ?? -1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void MatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender as Button)?.Tag is not Data.Models.DotaMatchModel match)
                {
                    throw new Exception("DotaMatchModel is null");
                }

                if (!Type.Equals(this.Frame.CurrentSourcePageType, typeof(MatchDataPage)))
                {
                    this.Frame.Navigate(typeof(MatchDataPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                }

                _ = _viewModel.MatchesViewModel.LoadMatchData(match.match_id.ToString());
            }
            catch (Exception ex)
            {
                LogCourier.Log($"MatchButton click error: {ex.Message}", LogCourier.LogType.Error);
            }
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
                this.Frame.Navigate(typeof(BlankPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
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
