using System;
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

        private MainViewModel? _viewModel;

        private HeroModel? _heroModel;

        private int _lastLanguageIndex = -1;

        public HeroDataPage()
        {
            this.InitializeComponent();

            _visual = ElementCompositionPreview.GetElementVisual(this);

            this.Loaded += (_, _) =>
            {
                DataAttributesScrollViewer.Width = RootGrid.ActualWidth;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ConnectedAnimation? animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Hero");
                animation?.TryStart(HeroImageBorder);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            var param = e.Parameter as Tuple<MainViewModel, HeroModel>;

            _viewModel = param?.Item1;
            _heroModel = param?.Item2;

            int heroId = _heroModel?.DotaHeroAttributes.id ?? -1;
            int languageIndex = _viewModel?.AppSettings.LanguageIndex ?? 0;
            _lastLanguageIndex = languageIndex;
            _ = _viewModel?.HeroesViewModel.PickHero(heroId, languageIndex);
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
        }

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

        private async void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_heroModel is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroHistoryView(_heroModel, _viewModel?.HeroesViewModel.PickedHeroData?.DotaHeroData?.bio_loc ?? "Failed to fetch hero's history."),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero history failed: {ex.ToString()}", LogCourier.LogType.Error);
            }
        }

        private async void RankingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_heroModel is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroRankingsView(_heroModel),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero rankings failed: {ex.ToString()}", LogCourier.LogType.Error);
            }
        }

        private void LanguageRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int heroId = _heroModel?.DotaHeroAttributes.id ?? -1;
            int languageIndex = (sender as Microsoft.UI.Xaml.Controls.RadioButtons)?.SelectedIndex ?? 0;

            if (languageIndex != _lastLanguageIndex)
            {
                LanguageFlyout?.Hide();

                _lastLanguageIndex = languageIndex;
                _ = _viewModel?.HeroesViewModel.PickHero(heroId, languageIndex);
            }
        }
    }
}
