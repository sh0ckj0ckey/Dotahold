using OpenDota_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchesListPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public MatchesListPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;
            }
            catch { }
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                DotaMatchesViewModel.Instance.GetAllMatchesAsync();
            }
            catch { }
        }

        /// <summary>
        /// 加载更多比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickLoadMoreMatches(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.IncreaseFromAllMatches();
            }
            catch { }
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
            catch { }
        }
    }
}
