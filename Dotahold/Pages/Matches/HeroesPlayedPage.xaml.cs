using Dotahold.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage));
            this.Frame.ForwardStack.Clear();
            this.Frame.BackStack.Clear();
        }
    }
}
