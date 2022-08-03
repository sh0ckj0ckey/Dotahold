using OpenDota_UWP.Helpers;
using OpenDota_UWP.ViewModels;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchesPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public MatchesPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                ViewModel.ActUpdatePieChart += (win, lose) =>
                {
                    ShowPieChart(win, lose);
                };

                FrameShadow.Receivers.Add(PlayerProfileGrid);
                MatchGrid.Translation += new System.Numerics.Vector3(0, 0, 36);

                MatchFrame.Navigate(typeof(BlankPage));
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

                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }

                if (string.IsNullOrEmpty(ViewModel.sSteamId))
                {
                    BindAccount();
                }
            }
            catch { }
        }

        //MatchData_KillTextBlock.Text = total[0];
        //MatchData_DeadTextBlock.Text = total[1];
        //MatchData_AssistTextBlock.Text = total[2];
        //MatchData_KDATextBlock.Text = total[3];
        //MatchData_GPMTextBlock.Text = total[4];
        //MatchData_XPMTextBlock.Text = total[5];
        //MatchData_Last_hitTextBlock.Text = total[6];
        //MatchData_DeniesTextBlock.Text = total[7];
        //MatchData_LevelTextBlock.Text = total[8];
        //MatchData_HeroDamageTextBlock.Text = total[9];
        //MatchData_TowerDamageTextBlock.Text = total[10];
        //MatchData_HealingTextBlock.Text = total[11];

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
            //catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
        }

        private void BindingGridPopOut_Completed(object sender, object e)
        {
            try
            {
                BindGrid.Visibility = Visibility.Collapsed;
            }
            catch { }
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
                    catch { }
                }
            }
            catch { }
        }

        private void HideBindingAccountGrid()
        {
            try
            {
                SteamIDTextBox.Text = "";
                BindingGridPopOut.Begin();
            }
            catch { }
        }

        #endregion

        #region 胜率饼状图

        /// <summary>
        /// 显示饼状图
        /// </summary>
        public void ShowPieChart(double win, double lose)
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

                double x = 24, y = 48;
                bool isWinRateBigger = false;
                if (rate > 0.5)
                {
                    rate = 1 - rate;
                    //胜率大于50%,应该左边表示胜利
                    isWinRateBigger = true;
                }
                if (rate < 0)
                {
                    x = 24; y = 48;
                }
                else if (rate <= 0.25)
                {
                    x = 24 + 24 * Math.Sin(2 * Math.PI * rate);
                    y = 24 - 24 * Math.Cos(2 * Math.PI * rate);
                }
                else if (rate > 0.25 && rate <= 0.5)
                {
                    x = 24 + 24 * Math.Cos((2 * rate - 0.5) * Math.PI);
                    y = 24 + 24 * Math.Sin((2 * rate - 0.5) * Math.PI);
                }
                else
                {
                    x = 24; y = 48;
                }
                RateArcSegment.Point = new Point(x, y);
                RatePolyLineSegment.Points = new PointCollection { new Point(24, 0), new Point(24, 24), new Point(x, y) };
                RatePolyline.Points = new PointCollection { new Point(24, 0), new Point(24, 24), new Point(x + 2, y + 2) };

                if (isWinRateBigger)
                {
                    LeftPieChart.Fill = new SolidColorBrush(Colors.ForestGreen);
                    RightPieChart.Fill = new SolidColorBrush(Colors.Firebrick);
                }
                else
                {
                    RightPieChart.Fill = new SolidColorBrush(Colors.ForestGreen);
                    LeftPieChart.Fill = new SolidColorBrush(Colors.Firebrick);
                }
            }
            catch { }
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
            catch { }
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
            catch { }
        }

        #endregion

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
            catch { }
        }

        /// <summary>
        ///  点击查看所有比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickRecentMatches(object sender, RoutedEventArgs e)
        {
            try
            {
                MatchFrame.Navigate(typeof(MatchesListPage));
            }
            catch { }
        }
    }
}
