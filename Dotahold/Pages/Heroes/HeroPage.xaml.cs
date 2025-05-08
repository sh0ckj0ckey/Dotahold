using System;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.UI.Composition;
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
    public sealed partial class HeroPage : Page
    {
        private readonly Visual _visual;

        private MainViewModel? _viewModel;

        private HeroModel? _heroModel;

        public HeroPage()
        {
            this.InitializeComponent();

            _visual = ElementCompositionPreview.GetElementVisual(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var param = e.Parameter as Tuple<MainViewModel, HeroModel>;

            _viewModel = param?.Item1;
            _heroModel = param?.Item2;

            try
            {
                ConnectedAnimation? animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Hero");
                animation?.TryStart(HeroImageBorder);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RankingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LanguageRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
