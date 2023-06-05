using Dotahold.Helpers;
using Dotahold.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchInfoPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        private SlideNavigationTransitionInfo SlideNaviTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        public MatchInfoPage()
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

                BanPickScrollViewer?.ChangeView(0, 0, 1, true);
            }
            catch { }
        }

        /// <summary>
        /// 切换到玩家经济走势图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowGoldSeries(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SwitchPlayersGoldOrXPSeries(true, ViewModel.CurrentMatchInfo);
            }
            catch { }
        }

        /// <summary>
        /// 切换到玩家经验走势图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowXPSeries(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SwitchPlayersGoldOrXPSeries(false, ViewModel.CurrentMatchInfo);
            }
            catch { }
        }

        /// <summary>
        /// 打开opendota网页查看更多比赛信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickVisitWebsite(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.CurrentMatchId > 0)
                {
                    string url = "https://www.opendota.com/matches/" + ViewModel.CurrentMatchId;
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
                }
            }
            catch { }
        }

        /// <summary>
        /// 点击玩家查看其详细数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPlayer(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is Models.Player player)
                {
                    ViewModel.CurrentMatchPlayer = player;
                    ViewModel.AnalyzePlayerInfo();
                    this.Frame.Navigate(typeof(MatchPlayerPage), null, SlideNaviTransition);
                }
            }
            catch { }
        }

        /// <summary>
        /// 点击显示或折叠天辉优势图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowHideRadiantAdv(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bShowRadiantAdv = !ViewModel.bShowRadiantAdv;
            }
            catch { }
        }

        /// <summary>
        /// 点击显示或折叠天辉优势图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowHidePlayerSeries(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bShowPlayerSeries = !ViewModel.bShowPlayerSeries;
            }
            catch { }
        }
    }
}
