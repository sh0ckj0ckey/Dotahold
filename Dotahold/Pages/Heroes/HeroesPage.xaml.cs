using System;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.UI.Composition;
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
    public sealed partial class HeroesPage : Page
    {
        private readonly Visual _visual;

        private MainViewModel? _viewModel;

        private Button? _connectedAnimationButton;

        public HeroesPage()
        {
            this.InitializeComponent();

            _visual = ElementCompositionPreview.GetElementVisual(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel = e.Parameter as MainViewModel;

            try
            {
                if (_connectedAnimationButton is not null)
                {
                    ConnectedAnimation? animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("HeroBack");
                    animation?.TryStart(_connectedAnimationButton);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button && button?.DataContext is HeroModel heroModel && heroModel is not null)
            {
                _connectedAnimationButton = button;

                ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromSeconds(0.4);
                ConnectedAnimationService.GetForCurrentView().DefaultEasingFunction = CompositionEasingFunction.CreateCubicBezierEasingFunction(_visual.Compositor, new(0.41f, 0.52f), new(0.0f, 0.94f));
                ConnectedAnimation heroAnimation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Hero", _connectedAnimationButton);
                heroAnimation.Configuration = new BasicConnectedAnimationConfiguration();

                this.Frame.Navigate(typeof(HeroPage), heroModel, new SuppressNavigationTransitionInfo());
            }
        }
    }
}
