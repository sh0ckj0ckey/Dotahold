using System;
using System.Text.RegularExpressions;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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

        #region 菜单

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
