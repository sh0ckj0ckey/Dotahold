using System;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Matches
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
        private readonly MainViewModel _viewModel;

        public OverviewPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.AppSettings.SteamID))
            {
                if (!Type.Equals(this.Frame.CurrentSourcePageType, typeof(SteamConnectPage)))
                {
                    this.Frame.Navigate(typeof(SteamConnectPage));
                    this.Frame.BackStack.Clear();
                }
            }
        }
    }
}
