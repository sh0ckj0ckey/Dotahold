using System;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchHeroesPlayedPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public MatchHeroesPlayedPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private void OnClickPlayedHeroMatch(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is DotaMatchHeroPlayedModel hero)
                {
                    DotaMatchesViewModel.Instance.GetHeroMatchesFormAllAsync(hero);
                    this.Frame.Navigate(typeof(MatchHeroMatchesPage));
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
