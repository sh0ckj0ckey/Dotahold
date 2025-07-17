using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.WinUI.Controls;
using Dotahold.Models;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchDataPlayerPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private MatchPlayerModel? _matchPlayerModel;
        public MatchPlayerModel? MatchPlayerModel
        {
            get => _matchPlayerModel;
            set
            {
                if (_matchPlayerModel != value)
                {
                    _matchPlayerModel = value;
                    OnPropertyChanged(nameof(MatchPlayerModel));
                }
            }
        }

        private List<LineSeries> _playerSelectedGraph = [];
        public List<LineSeries> PlayerSelectedGraph
        {
            get => _playerSelectedGraph;
            set
            {
                if (_playerSelectedGraph != value)
                {
                    _playerSelectedGraph = value;
                    OnPropertyChanged(nameof(PlayerSelectedGraph));
                }
            }
        }

        public MatchDataPlayerPage()
        {
            this.DataContext = this;
            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                UpdateLayoutsWidth();
                GraphChartsSegmented.SelectedIndex = -1;
                GraphChartsSegmented.SelectedIndex = 0;
                PlayerDataScrollViewer.ScrollToVerticalOffset(0);
                OverviewScrollViewer.ScrollToHorizontalOffset(0);
                StatisticsScrollViewer.ScrollToHorizontalOffset(0);
                AbilityUpgradesScrollViewer.ScrollToHorizontalOffset(0);
                PermanentBuffsScrollViewer.ScrollToHorizontalOffset(0);
                RunesScrollViewer.ScrollToHorizontalOffset(0);
                KillsScrollViewer.ScrollToHorizontalOffset(0);
                PurchaseScrollViewer.ScrollToHorizontalOffset(0);
                BenchmarksScrollViewer.ScrollToHorizontalOffset(0);

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
            this.MatchPlayerModel = e.Parameter as MatchPlayerModel;
            if (this.MatchPlayerModel is not null)
            {
                foreach (var item in this.MatchPlayerModel.AbilityUpgrades)
                {
                    await item.IconImage.LoadImageAsync(true);
                }
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutsWidth();
        }

        private void UpdateLayoutsWidth()
        {
            PlayerDataStackPanel.Width = RootGrid.ActualWidth;
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

        private void GraphChartsSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Segmented segmented && this.MatchPlayerModel is not null)
            {
                int selectedIndex = segmented.SelectedIndex;
                switch (selectedIndex)
                {
                    case 0:
                        this.PlayerSelectedGraph = [new LineSeries
                        {
                            Icon = this.MatchPlayerModel.Hero?.HeroIcon,
                            Title = "Gold",
                            LineColorBrush = new SolidColorBrush(Colors.Goldenrod),
                            Data = this.MatchPlayerModel.DotaMatchPlayer.gold_t ?? [],
                        }];
                        break;
                    case 1:
                        this.PlayerSelectedGraph = [new LineSeries
                        {
                            Icon = this.MatchPlayerModel.Hero?.HeroIcon,
                            Title = "XP",
                            LineColorBrush = new SolidColorBrush(Colors.MediumOrchid),
                            Data = this.MatchPlayerModel.DotaMatchPlayer.xp_t ?? [],
                        }];
                        break;
                    case 2:
                        this.PlayerSelectedGraph = [new LineSeries
                        {
                            Icon = this.MatchPlayerModel.Hero?.HeroIcon,
                            Title = "Last Hits",
                            LineColorBrush = this.MatchPlayerModel.SlotColorBrush,
                            Data = this.MatchPlayerModel.DotaMatchPlayer.lh_t ?? [],
                        }];
                        break;
                    case 3:
                        this.PlayerSelectedGraph = [new LineSeries
                        {
                            Icon = this.MatchPlayerModel.Hero?.HeroIcon,
                            Title = "Denies",
                            LineColorBrush = this.MatchPlayerModel.SlotColorBrush,
                            Data = this.MatchPlayerModel.DotaMatchPlayer.dn_t ?? [],
                        }];
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
