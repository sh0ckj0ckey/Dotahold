using System;
using Dotahold.Controls;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Heroes
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroDataPage : Page
    {
        private readonly Visual _visual;

        private readonly MainViewModel _viewModel;

        private HeroModel? _heroModel;

        public HeroDataPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            _visual = ElementCompositionPreview.GetElementVisual(this);

            this.Loaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;

                DataAttributesScrollViewer.Width = RootGrid.ActualWidth;
                FacetsGrid.Width = RootGrid.ActualWidth - 128;
            };

            this.Unloaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ConnectedAnimation? animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Hero");
                animation?.TryStart(HeroImageBorder);

                _heroModel = e.Parameter as HeroModel;

                if (_heroModel is not null)
                {
                    _ = _viewModel.HeroesViewModel.PickHero(_heroModel, _viewModel.AppSettings.LanguageIndex);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromSeconds(0.3);
                ConnectedAnimationService.GetForCurrentView().DefaultEasingFunction = CompositionEasingFunction.CreateCubicBezierEasingFunction(_visual.Compositor, new(0.41f, 0.52f), new(0.0f, 0.94f));
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("HeroBack", HeroImageBorder);
                animation.Configuration = new BasicConnectedAnimationConfiguration();
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataAttributesScrollViewer.Width = RootGrid.ActualWidth;
            FacetsGrid.Width = RootGrid.ActualWidth - 128;
        }

        /// <summary>
        /// 查看英雄背景故事
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.HeroesViewModel.PickedHero is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroHistoryView(_viewModel.HeroesViewModel.PickedHero, _viewModel.HeroesViewModel.PickedHeroData?.DotaHeroData?.bio_loc ?? "Failed to fetch hero's history."),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero history failed: {ex.Message}", LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// 查看英雄排名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RankingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.HeroesViewModel.PickedHero is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroRankingsView(_viewModel.HeroesViewModel.PickedHero),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero rankings failed: {ex.Message}", LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn)
            {
                string languageName = btn.Content.ToString() ?? "English";

                int index = languageName switch
                {
                    "English" => 0,
                    "Chinese" => 1,
                    "Russian" => 2,
                    _ => 0
                };

                if (_viewModel.AppSettings.LanguageIndex != index)
                {
                    LanguageFlyout?.Hide();

                    _viewModel.AppSettings.LanguageIndex = index;

                    if (_heroModel is not null)
                    {
                        _ = _viewModel.HeroesViewModel.PickHero(_heroModel, _viewModel.AppSettings.LanguageIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 查看天赋树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalentsButton_Click(object sender, RoutedEventArgs e)
        {
            HeroTalentsTeachingTip.IsOpen = !HeroTalentsTeachingTip.IsOpen;
        }

        /// <summary>
        /// 查看先天技能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InnateButton_Click(object sender, RoutedEventArgs e)
        {
            HeroInnateTeachingTip.IsOpen = !HeroInnateTeachingTip.IsOpen;
        }

        #region GoBack

        private bool TryGoBack()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            else
            {
                return false;
            }
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
