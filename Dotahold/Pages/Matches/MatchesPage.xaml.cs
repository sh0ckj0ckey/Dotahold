using System;
using Dotahold.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                var steamId = e.Parameter?.ToString();

                if (!string.IsNullOrWhiteSpace(steamId))
                {
                    _ = _viewModel.MatchesViewModel.LoadPlayerAllMatches(steamId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
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

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage));
            this.Frame.ForwardStack.Clear();
            this.Frame.BackStack.Clear();
        }
    }
}
