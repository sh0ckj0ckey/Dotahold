using Dotahold.Models;
using Dotahold.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DotaMatchesPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public DotaMatchesPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                ViewModel.ActUpdateWinRateCapsule += (win, lose) =>
                {
                    ShowWinRateCapsule(win, lose);
                };

                MatchFrame.Navigate(typeof(BlankPage));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
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

                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }

                if (string.IsNullOrWhiteSpace(DotaMatchesViewModel.Instance.sSteamId))
                {
                    BindAccount();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击查看常用英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickMostPlayedHeroes(object sender, RoutedEventArgs e)
        {
            try
            {
                MatchFrame.Navigate(typeof(MatchHeroesPlayedPage));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击查看某个英雄的所有比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPlayedHeroMatch(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is DotaMatchHeroPlayedModel hero)
                {
                    DotaMatchesViewModel.Instance.GetHeroMatchesFormAllAsync(hero);
                    MatchFrame.Navigate(typeof(MatchHeroMatchesPage));
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击查看所有比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickRecentMatches(object sender, RoutedEventArgs e)
        {
            try
            {
                MatchFrame.Navigate(typeof(MatchesListPage));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 查看最近的一场比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFlipRecentMatch(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Models.DotaRecentMatchModel match && match.match_id != null)
                {
                    ViewModel.GetMatchInfoAsync(match.match_id ?? 0);
                    MatchFrame.Navigate(typeof(MatchInfoPage));
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 查看最近的一场比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickListRecentMatch(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is Models.DotaRecentMatchModel match && match.match_id != null)
                {
                    ViewModel.GetMatchInfoAsync(match.match_id ?? 0);
                    MatchFrame.Navigate(typeof(MatchInfoPage));
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击绑定历史账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDotaIdHistory(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Models.DotaIdBindHistoryModel steamId)
                {
                    if (steamId.SteamId != ViewModel.sSteamId)
                    {
                        ViewModel.SetSteamID(steamId.SteamId);
                        ViewModel.InitialDotaMatches();
                        MatchFrame.Navigate(typeof(BlankPage));
                    }
                    HideBindingAccountGrid();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #region 菜单

        /// <summary>
        /// 查看社区主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SteamCommunityLinkMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = ViewModel.PlayerProfile.profile.profileurl;
                if (!string.IsNullOrEmpty(url.Trim()))
                {
                    if (!string.IsNullOrEmpty(url) && !url.StartsWith("http"))
                    {
                        url = "https://" + url;
                    }
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
                    return;
                }
                else
                {
                    SteamCommunityLinkMenuFlyoutItem.IsEnabled = false;
                }
            }
            catch { SteamCommunityLinkMenuFlyoutItem.IsEnabled = false; }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProfileMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    UpdateProfileMenuFlyoutItem.Text = "Updating Data";
            //    UpdateProfileMenuFlyoutItem.IsEnabled = false;

            //    ViewModel.PostRefreshAsync(ViewModel.sSteamId);
            //}
            //catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 点击复制ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameIDMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(ViewModel.sSteamId);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 更改绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                BindAccount();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.InitialDotaMatches();
                MatchFrame.Navigate(typeof(BlankPage));
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

        #region 游戏ID绑定

        /// <summary>
        /// 输入内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string input = SteamIDTextBox.Text;
                if (input.Length > 0 && Regex.IsMatch(input, @"^\d+$"))
                {
                    OKButton.IsEnabled = true;
                }
                else
                {
                    OKButton.IsEnabled = false;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 按下回车确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamIDTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    SetInputSteamId();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 确认绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetInputSteamId();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 显示绑定新账号窗口
        /// </summary>
        public void BindAccount()
        {
            try
            {
                BindGrid.Visibility = Visibility.Visible;
                BindingGridPopIn.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideBindingAccountGrid();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private void BindingGridPopOut_Completed(object sender, object e)
        {
            try
            {
                BindGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private void SetInputSteamId()
        {
            try
            {
                string input = SteamIDTextBox.Text;
                if (input.Length > 0 && Regex.IsMatch(input, @"^\d+$"))
                {
                    try
                    {
                        ViewModel.SetSteamID(input);
                        ViewModel.InitialDotaMatches();
                        HideBindingAccountGrid();
                        MatchFrame.Navigate(typeof(BlankPage));
                    }
                    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private void HideBindingAccountGrid()
        {
            try
            {
                SteamIDTextBox.Text = "";
                BindingGridPopOut.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

        #region 胜率图

        /// <summary>
        /// 显示胜率图
        /// </summary>
        public void ShowWinRateCapsule(double win, double lose)
        {
            try
            {
                double rate = 0.5;

                if (win == 0) rate = 0;
                else if (lose == 0) rate = 1;
                else
                {
                    rate = win / (win + lose);
                    rate = (Math.Floor(100 * rate) / 100);
                }

                AllLoseBorder.Visibility = Visibility.Collapsed;
                AllWinBorder.Visibility = Visibility.Collapsed;
                WinRateGrid.Visibility = Visibility.Collapsed;
                if (rate <= 0)
                {
                    AllLoseBorder.Visibility = Visibility.Visible;
                }
                else if (rate >= 1)
                {
                    AllWinBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    WinRateGrid.Visibility = Visibility.Visible;
                    rate *= 100;
                    WinColumn.Width = new GridLength(rate, GridUnitType.Star);
                    LoseColumn.Width = new GridLength(100 - rate, GridUnitType.Star);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 鼠标移入，显示胜负场数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                HoverToShowWinLose?.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 鼠标移出，显示胜率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ExitToShowWinRate?.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

        #region 搜索

        /// <summary>
        /// 显示搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bSearchingByMatchId = true;
                ShowTitleSearchTextBox?.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 确定搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGoSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = TitleSearchTextBox.Text;
                if (input.Length > 0 && Regex.IsMatch(input, @"^\d+$"))
                {
                    long id;
                    bool parse = long.TryParse(input, out id);
                    if (parse)
                    {
                        ViewModel.GetMatchInfoAsync(id);
                        MatchFrame.Navigate(typeof(MatchInfoPage));
                        ViewModel.bSearchingByMatchId = false;
                        TitleSearchTextBox.Text = "";
                        ShowTitlePersonaname?.Begin();
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        /// <summary>
        /// 取消搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.bSearchingByMatchId = false;
                TitleSearchTextBox.Text = "";
                ShowTitlePersonaname?.Begin();
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

    }
}
