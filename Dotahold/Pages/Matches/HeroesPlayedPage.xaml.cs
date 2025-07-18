using System;
using Dotahold.Data.DataShop;
using Dotahold.Models;
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
    public sealed partial class HeroesPlayedPage : Page
    {
        private readonly MainViewModel _viewModel;

        public HeroesPlayedPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                PlayerHeroPerformancesItemsRepeater.ItemsSource = _viewModel.ProfileViewModel.PlayerHeroPerformances;

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
            };

            this.Unloaded += (_, _) =>
            {
                PlayerHeroPerformancesItemsRepeater.ItemsSource = null;

                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Frame.Navigate(typeof(MatchesPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });

                HeroModel? heroFilter = (sender as Button)?.Tag as HeroModel;
                bool isNewFilter = _viewModel.MatchesViewModel.MatchesHeroFilter != heroFilter;

                _viewModel.MatchesViewModel.SetMatchesHeroFilter(heroFilter);

                await _viewModel.MatchesViewModel.LoadPlayerAllMatches(_viewModel.AppSettings.SteamID);

                if (_viewModel.MatchesViewModel.MatchesHeroFilter == heroFilter)
                {
                    if (isNewFilter)
                    {
                        await _viewModel.MatchesViewModel.ClearMatches(heroFilter?.DotaHeroAttributes.id ?? -1);
                    }

                    if (_viewModel.MatchesViewModel.Matches.Count <= 0)
                    {
                        _viewModel.MatchesViewModel.LoadMoreMatches(40, heroFilter?.DotaHeroAttributes.id ?? -1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"HeroPlayedButton click error: {ex.Message}", LogCourier.LogType.Error);
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
