using OpenDota_UWP.Helpers;
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
        //public static PlayerProfile MatchPlayerProfile = new PlayerProfile();
        //public WinNLose wL = new WinNLose();
        private string ID = "";
        private string SteamCommunityLink = "";
        //private ObservableCollection<RecentMatchViewModel> recentMatchesObservableCollection = new ObservableCollection<RecentMatchViewModel>();
        List<HeroUsingInfo> heroUsingInfos = null;
        private ObservableCollection<HeroUsingInfoViewModel> heroUsingInfoObservableCollection = new ObservableCollection<HeroUsingInfoViewModel>();

        public MatchesPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            //ID = DotaMatchHelper.GetSteamID();
            ////读取用户ID，如果为空就显示绑定页面
            //if (ID == "")
            //{
            //    BindAccount();
            //}
            //else
            //{
            //    try
            //    {
            //        ShowPlayerProfileAsync(ID);
            //        ShowPieChart(ID);
            //        ShowPlayerTotalDataAsync(ID);
            //        ShowRecentMatches(ID);
            //        ShowHeroUsingInfo(ID);
            //    }
            //    catch
            //    {
            //        ShowDialog("Sorry, something went wrong while fetching data.");
            //        this.Frame.Navigate(typeof(BlankPage));
            //    }
            //}
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is NavigationTransitionInfo transition)
            {
                navigationTransition.DefaultNavigationTransitionInfo = transition;
            }
            //SharedData.Share.PlayerID = DotaMatchHelper.GetSteamID();

            //this.RegisterBackgroundTask();
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 显示玩家资料
        /// </summary>
        public async void ShowPlayerProfileAsync(string id)
        {
            //try
            //{
            //    MatchPlayerProfile = await DotaMatchHelper.GetPlayerProfileAsync(id);
            //    if (MatchPlayerProfile == null)
            //    {
            //        //ShowDialog("抱歉，可能是查询数据太频繁，请稍后尝试");
            //        return;
            //    }
            //    PlayerPhotoBitmapImage.UriSource = new Uri(MatchPlayerProfile.profile.avatarfull);
            //    PersonNameTextBlock.Text = MatchPlayerProfile.profile.personaname;
            //    GameIDTextBlock.Text = "ID: " + MatchPlayerProfile.profile.account_id.ToString();
            //    SteamCommunityLink = MatchPlayerProfile.profile.profileurl;

            //    CurrentNumberTextBlock.Text = await DotaMatchHelper.GetNumberOfCurrentPlayers();

            //    string rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRank0-0.png";
            //    MatchData_LeaderboardTextBlock.Text = "—";
            //    //没有分段
            //    if (MatchPlayerProfile.rank_tier == null)
            //    { }
            //    //如果是冠绝一世即“80”
            //    else if (MatchPlayerProfile.rank_tier == "80")
            //    {
            //        //如果有排名
            //        if ((MatchPlayerProfile.leaderboard_rank != null && MatchPlayerProfile.leaderboard_rank != "null" && Regex.IsMatch(MatchPlayerProfile.leaderboard_rank, "^[\\d]+$")))
            //        {
            //            int leaderboard_rank = Convert.ToInt32(MatchPlayerProfile.leaderboard_rank);
            //            MatchData_LeaderboardTextBlock.Text = MatchPlayerProfile.leaderboard_rank;
            //            if (leaderboard_rank > 1000)
            //            {
            //                rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop0.png";
            //            }
            //            else if (leaderboard_rank <= 1000 && leaderboard_rank > 100)
            //            {
            //                rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop1.png";
            //            }
            //            else if (leaderboard_rank <= 100 && leaderboard_rank > 10)
            //            {
            //                rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop2.png";
            //            }
            //            else if (leaderboard_rank <= 10 && leaderboard_rank > 1)
            //            {
            //                rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop3.png";
            //            }
            //            else if (leaderboard_rank == 1)
            //            {
            //                rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop4.png";
            //            }
            //            else
            //            {
            //                rankMedalSource = String.Format("ms-appx:///Assets/RankMedal/SeasonalRankTop0.png");
            //            }
            //        }
            //        else
            //        {
            //            rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop0.png";
            //        }
            //    }
            //    else if (MatchPlayerProfile.rank_tier.Length == 2)
            //    {
            //        rankMedalSource = String.Format("ms-appx:///Assets/RankMedal/SeasonalRank{0}-{1}.png", MatchPlayerProfile.rank_tier[0], MatchPlayerProfile.rank_tier[1]);
            //        MatchData_LeaderboardTextBlock.Text = "—";
            //    }
            //    else
            //    {
            //        rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRank0-0.png";
            //    }
            //    MatchData_RankMedalImage.Source = new BitmapImage(new Uri(rankMedalSource));
            //    MatchData_MMRTextBlock.Text = MatchPlayerProfile.mmr_estimate.estimate.ToString();
            //}
            //catch
            //{
            //    //ShowDialog("无法获取当前绑定账号的信息，请重新绑定");
            //    //this.Frame.Navigate(typeof(BlankPage));
            //    return;
            //}

            //更新磁贴
            //SetTile(MatchPlayerProfile);
        }

        /// <summary>
        /// 显示玩家的全期数据
        /// </summary>
        /// <param name="id"></param>
        public async void ShowPlayerTotalDataAsync(string id)
        {
            try
            {
                //string[] total = await DotaMatchHelper.GetTotalAsync(id);
                //if (total == null)
                //{
                //    ShowDialog("Sorry, something went wrong while connecting to server.");
                //    this.Frame.Navigate(typeof(BlankPage));
                //    return;
                //}

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
                //MatchData_APMTextBlock.Text = total[12];
            }
            catch
            {
                ShowDialog("Sorry, something went wrong while connecting to server.");
                this.Frame.Navigate(typeof(BlankPage));
                return;
            }
        }

        /// <summary>
        /// 显示最近20场比赛
        /// </summary>
        public async void ShowRecentMatches(string id)
        {
            try
            {
                //List<RecentMatch> result = await DotaMatchHelper.GetRecentMatchAsync(id);
                //if (result == null)
                //{
                //    ShowDialog("The data query may be too frequent, please try again later.");
                //    return;
                //}
                //foreach (RecentMatch item in result)
                //{
                //    //处理评级(skill)的字符串
                //    string skill;
                //    switch (item.skill)
                //    {
                //        case "1":
                //            skill = "Normal";
                //            break;
                //        case "2":
                //            skill = "High";
                //            break;
                //        case "3":
                //            skill = "Very High";
                //            break;
                //        default:
                //            skill = "";
                //            break;
                //    }
                //    //处理时间
                //    string time = ComputeTime(item.start_time);
                //    //处理胜负
                //    Visibility win = Visibility.Collapsed;
                //    Visibility lose = Visibility.Collapsed;
                //    if ((item.radiant_win == "true" && (item.player_slot == "0" || item.player_slot == "1" || item.player_slot == "2" || item.player_slot == "3" || item.player_slot == "4")) ||
                //        (item.radiant_win == "false" && (item.player_slot == "128" || item.player_slot == "129" || item.player_slot == "130" || item.player_slot == "131" || item.player_slot == "132")))
                //    {
                //        win = Visibility.Visible;
                //        lose = Visibility.Collapsed;
                //    }
                //    else
                //    {
                //        win = Visibility.Collapsed;
                //        lose = Visibility.Visible;
                //    }
                //    //将这些数据整理起来添加到列表中
                //    recentMatchesObservableCollection.Add(
                //        new RecentMatchViewModel()
                //        {
                //            SelectedHero = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(item.hero_id)]].Name,
                //            HeroPhoto = ConstantsHelper.dotaHerosDictionary[ConstantsHelper.HeroID[Convert.ToInt32(item.hero_id)]].LargePic,
                //            Skill = skill,
                //            Time = time,
                //            KDA = item.kills + "/" + item.deaths + "/" + item.assists,
                //            PlayerWin = win,
                //            PlayerLose = lose,
                //            GPM = item.gold_per_min,
                //            XPM = item.xp_per_min,
                //            Match_ID = item.match_id
                //        });
                //}
            }
            catch
            {
                ShowDialog("Sorry, something went wrong while connecting to server.");
                return;
            }
        }

        /// <summary>
        /// 显示常用英雄信息
        /// </summary>
        public async void ShowHeroUsingInfo(string id)
        {
            //heroUsingInfos = await DotaMatchHelper.GetHeroUsingAsync(id);
            //if (heroUsingInfos == null)
            //{
            //    FailedTextBlock.Visibility = Visibility.Visible;
            //    UsingIndexGridView.SelectedIndex = -1;
            //    UsingIndexGridView.IsEnabled = false;
            //    LeftHyperlinkButton.IsEnabled = false;
            //    RightHyperlinkButton.IsEnabled = false;
            //    return;
            //}
            //UsingIndexGridView.SelectedIndex = 0;
        }

        /// <summary>
        /// 显示饼状图
        /// </summary>
        /// <param name="isWinRateBiggerThanHalf"></param>
        public async void ShowPieChart(string id)
        {
            //wL = await DotaMatchHelper.GetPlayerWLAsync(id);

            DataContext = this;

            //double rate = wL.win / (wL.win + wL.lose);
            //CountTextBlock.Text = (wL.win + wL.lose).ToString();
            //RateTextBlock.Text = (100 * rate).ToString("f1");

            //double[] point = GetPieChart(rate);
            //RateArcSegment.Point = new Point(point[0], point[1]);
            //RatePolyLineSegment.Points = new PointCollection { new Point(0, 0), new Point(0, 96), new Point(point[0], point[1]) };
            //RatePolyline.Points = new PointCollection { new Point(0, 0), new Point(0, 96), new Point(point[0], point[1]) };
            //ShowPieChartScale.Begin();

            //if (rate > 0.5/*如果胜率大于0.5*/)
            //{
            //    //那么就是左边显示胜利,填充颜色红色,文字显示胜利
            //    LeftTextBlock.Text = "胜：" + wL.win;
            //    RightTextBlock.Text = "负：" + wL.lose;
            //    LeftPieChart.Fill = new SolidColorBrush(Colors.ForestGreen);
            //    RightPieChart.Fill = new SolidColorBrush(Colors.Firebrick);
            //}
            //else /*胜率小于0.5*/
            //{
            //    //右边显示胜利,填充颜色红色,文字显示胜利
            //    RightTextBlock.Text = "胜：" + wL.win;
            //    LeftTextBlock.Text = "负：" + wL.lose;
            //    RightPieChart.Fill = new SolidColorBrush(Colors.ForestGreen);
            //    LeftPieChart.Fill = new SolidColorBrush(Colors.Firebrick);
            //}
        }

        ///// <summary>
        ///// 计算扇形图的“终点”坐标
        ///// </summary>
        ///// <param name="rate">胜率</param>
        ///// <returns></returns>
        //public double[] GetPieChart(double rate)
        //{
        //    //bool IsWinRateBiggerThanHalf = false;
        //    if (rate > 0.5)
        //    {
        //        rate = 1 - rate;
        //        //胜率大于50%,应该左边表示胜利
        //        //IsWinRateBiggerThanHalf = true;
        //    }
        //    double x, y;
        //    if (rate < 0)
        //    {
        //        return new double[] { 0, 0 };
        //    }
        //    else if (rate <= 0.25)
        //    {
        //        x = 96 * Math.Sin(2 * Math.PI * rate);
        //        y = 96 - 96 * Math.Cos(2 * Math.PI * rate);
        //    }
        //    else if (rate > 0.25 && rate <= 0.5)
        //    {
        //        x = 96 * Math.Cos((2 * rate - 0.5) * Math.PI);
        //        y = 96 + 96 * Math.Sin((2 * rate - 0.5) * Math.PI);
        //    }
        //    else
        //    {
        //        //开头已经保证rate小于0.5,这里应该不可能发生了,以防万一放在这里防止意外
        //        return new double[] { 0, 0 };
        //    }
        //    return new double[] { x, y };
        //}

        /// <summary>
        /// 查看社区主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (SteamCommunityLink != "")
            {
                try
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(SteamCommunityLink));
                }
                catch
                {
                    SteamCommunityLinkMenuFlyoutItem.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            //DotaMatchHelper.PostRefreshAsync(ID);
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
            //    this.Frame.Navigate(typeof(MatchesPage));
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// 更改绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            BindAccount();
        }

        /// <summary>
        /// 显示绑定新账号窗口
        /// </summary>
        public void BindAccount()
        {
            //BindGrid.Visibility = Visibility.Visible;
            //BindingGridPopIn.Begin();
            //string id = DotaMatchHelper.GetSteamID();
            //if (id == "")
            //{
            //    BackAppBarButton.IsEnabled = false;
            //}
        }

        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            BindingGridPopOut.Begin();
        }

        private void BindingGridPopOut_Completed(object sender, object e)
        {
            BindGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 输入内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool a = Regex.IsMatch(SteamIDTextBox.Text, @"^\d+$");
            if (a == false)
            {
                OKButton.IsEnabled = false;
            }
            else
            {
                OKButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 按下回车确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SteamIDTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key == Windows.System.VirtualKey.Enter)
            //{
            //    if (Regex.IsMatch(SteamIDTextBox.Text, @"^\d+$"))
            //    {
            //        try
            //        {
            //            DotaMatchHelper.SetSteamID(SteamIDTextBox.Text);
            //            this.NavigationCacheMode = NavigationCacheMode.Disabled;
                        
            //                this.Frame.Navigate(typeof(MatchesPage));
            //            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            //        }
            //        catch
            //        {
            //            FailedTextBlock.Visibility = Visibility.Visible;
            //            SteamIDTextBox.Text = "";
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 确认绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Regex.IsMatch(SteamIDTextBox.Text, @"^\d+$"))
            //{
            //    try
            //    {
            //        DotaMatchHelper.SetSteamID(SteamIDTextBox.Text);
            //        this.NavigationCacheMode = NavigationCacheMode.Disabled;
            //            this.Frame.Navigate(typeof(MatchesPage));
            //        this.NavigationCacheMode = NavigationCacheMode.Enabled;
            //    }
            //    catch
            //    {
            //        FailedTextBlock.Visibility = Visibility.Visible;
            //        SteamIDTextBox.Text = "";
            //    }
            //}
        }

        /// <summary>
        /// 指向MMR单元格(GridViewItem)的时候显示注释
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            MMRTitleTextBlock.Text = "NOT RANK";
            MMRTitleTextBlock.FontSize = 12;
        }
        private void GridViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            MMRTitleTextBlock.Text = "MMR";
            MMRTitleTextBlock.FontSize = 14;
        }

        /// <summary>
        /// 显示 "无法显示数据?" 提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        /// <summary>
        /// 点击一条比赛记录查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecentMatchListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MatchPicture.Visibility = Visibility.Collapsed;
            //    MatchInfoFrame.Navigate(typeof(MatchInfoPage), recentMatchesObservableCollection[RecentMatchListView.SelectedIndex]);
        }

        /// <summary>
        /// 十几页常用英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsingIndexGridView.SelectedIndex < 0)
            {
                return;
            }
            if (UsingIndexGridView.SelectedIndex < 1)
            {
                LeftHyperlinkButton.IsEnabled = false;
            }
            if (UsingIndexGridView.SelectedIndex < 11)
            {
                RightHyperlinkButton.IsEnabled = true;
            }
            if (UsingIndexGridView.SelectedIndex > 10)
            {
                RightHyperlinkButton.IsEnabled = false;
            }
            if (UsingIndexGridView.SelectedIndex > 0)
            {
                LeftHyperlinkButton.IsEnabled = true;
            }

            if (heroUsingInfos != null)
            {
                int start = UsingIndexGridView.SelectedIndex + 1;
                heroUsingInfoObservableCollection.Clear();
                for (int i = start * 10 - 10; i < start * 10; i++)
                {
                    try
                    {
                        HeroUsingInfoViewModel heroUsingInfoOverview = new HeroUsingInfoViewModel(heroUsingInfos[i]);
                        heroUsingInfoOverview.Time = ComputeTime(heroUsingInfos[i].last_played);
                        heroUsingInfoObservableCollection.Add(heroUsingInfoOverview);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }
        private void LeftHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsingIndexGridView.SelectedIndex > 0)
            {
                UsingIndexGridView.SelectedIndex--;
            }
            if (UsingIndexGridView.SelectedIndex < 1)
            {
                LeftHyperlinkButton.IsEnabled = false;
            }
            if (UsingIndexGridView.SelectedIndex < 11)
            {
                RightHyperlinkButton.IsEnabled = true;
            }
        }
        private void RightHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsingIndexGridView.SelectedIndex < 11)
            {
                UsingIndexGridView.SelectedIndex++;
            }
            if (UsingIndexGridView.SelectedIndex > 10)
            {
                RightHyperlinkButton.IsEnabled = false;
            }
            if (UsingIndexGridView.SelectedIndex > 0)
            {
                LeftHyperlinkButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 计算一串字符串表示时间是多久以前
        /// </summary>
        /// <param name="tm"></param>
        /// <returns></returns>
        public string ComputeTime(string tm)
        {
            if (tm == "0")
            {
                return "—";
            }
            string time = "";
            TimeSpan timed = DateTime.UtcNow - new DateTime(1970, 1, 1);
            double duration = (Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - Convert.ToInt64(tm));
            if (duration / 31536000 >= 1)
            {
                time = Convert.ToInt32(duration / 31536000) + " years ago";
            }
            else if (duration / 2592000 >= 1)
            {
                time = Convert.ToInt32(duration / 2592000) + " months ago";
            }
            else if (duration / 604800 >= 1)
            {
                time = Convert.ToInt32(duration / 604800) + " weeks ago";
            }
            else if (duration / 86400 >= 1)
            {
                time = Convert.ToInt32(duration / 86400) + " days ago";
            }
            else if (duration / 3600 >= 1)
            {
                time = Convert.ToInt32(duration / 3600) + " hours ago";
            }
            else if (duration / 60 >= 1)
            {
                time = Convert.ToInt32(duration / 60) + " minutes ago";
            }
            else
            {
                time = "";
            }
            return time;
        }

        /// <summary>
        /// 显示程序异常对话框，自定义内容
        /// </summary>
        public static async void ShowDialog(string content)
        {
            var dialog = new ContentDialog()
            {
                Title = ":(",
                Content = content,
                PrimaryButtonText = "Okay",
                FullSizeDesired = false
            };

            dialog.PrimaryButtonClick += (_s, _e) => { };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }

    }
}
