using System;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel? _viewModel;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = e.Parameter as MainViewModel;
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var verticalOffset = scrollViewer.VerticalOffset;
                var maxOffset = Math.Min(120, scrollViewer.ScrollableHeight);

                if (maxOffset <= 0) return;

                // 透明度按滚动比例变化，从全透明到不透明
                double newOpacity = verticalOffset / maxOffset;
                if (newOpacity > 1)
                {
                    newOpacity = 1;
                }
                if (newOpacity < 0)
                {
                    newOpacity = 0;
                }

                SettingsPageHeaderSeperatorLineBorder.Opacity = newOpacity;
            }
        }

        private void Image_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {

        }

        private void RateButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
