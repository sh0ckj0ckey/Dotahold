using System;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchHeroMatchesPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public MatchHeroMatchesPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击查看比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickMatch(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is Models.DotaRecentMatchModel match && match.match_id != null)
                {
                    ViewModel.GetMatchInfoAsync(match.match_id ?? 0);
                    this.Frame.Navigate(typeof(MatchInfoPage));
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
