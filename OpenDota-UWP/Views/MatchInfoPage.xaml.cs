using OpenDota_UWP.Helpers;
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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchInfoPage : Page
    {
        private ObservableCollection<PlayersInfoViewModel> radiantPlayers = new ObservableCollection<PlayersInfoViewModel>();
        private ObservableCollection<PlayersInfoViewModel> direPlayers = new ObservableCollection<PlayersInfoViewModel>();
        private List<PlayersInfoViewModel> radiantPlayersList = new List<PlayersInfoViewModel>();
        private List<PlayersInfoViewModel> direPlayersList = new List<PlayersInfoViewModel>();

        public ISeries[] Series { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private string match_id = "";

        public MatchInfoPage()
        {
            this.InitializeComponent();
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
            match_id = (e.Parameter as RecentMatchViewModel).Match_ID;
            MatchIDTextBlock.Text = (e.Parameter as RecentMatchViewModel).Match_ID;
            MatchData_BeginTimeTextBlock.Text = (e.Parameter as RecentMatchViewModel).Time;
            MatchData_LevelTextBlock.Text = (e.Parameter as RecentMatchViewModel).Skill == "" ? "—" : (e.Parameter as RecentMatchViewModel).Skill;
            if (MatchData_LevelTextBlock.Text == "Normal")
            {
                MatchData_LevelTextBlock.Opacity = 0.3;
            }
            else if (MatchData_LevelTextBlock.Text == "High")
            {
                MatchData_LevelTextBlock.Opacity = 0.6;
            }
            else if (MatchData_LevelTextBlock.Text == "Very High")
            {
                MatchData_LevelTextBlock.Foreground = new SolidColorBrush(Colors.DarkOrange);
            }
            ShowMatchInfo(match_id);
            ShowPlayers(match_id);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (radiantPlayers != null)
            {
                radiantPlayers.Clear();
                radiantPlayers = null;
            }
            if (direPlayers != null)
            {
                direPlayers.Clear();
                direPlayers = null;
            }
            if (radiantPlayersList != null)
            {
                radiantPlayersList.Clear();
                radiantPlayersList = null;
            }
            if (direPlayersList != null)
            {
                direPlayersList.Clear();
                direPlayersList = null;
            }

            base.OnNavigatedFrom(e);
        }

        private async void ShowMatchInfo(string match_id)
        {
            List<string> list = await DotaMatchHelper.GetMatchInfoAsync(match_id);
            if (list[0] == "time_limit")
            {
                ShowDialog("好像获取不到数据了(超时)，请稍等一会儿再查看");
                this.Frame.Navigate(typeof(BlankPage));
                return;
            }
            else if (list[0] == "server_error")
            {
                ShowDialog("抱歉，OpenDota 服务器连接错误，请过一段时间再试");
                this.Frame.Navigate(typeof(BlankPage));
                return;
            }
            else if (list[0] == "data_error")
            {
                ShowDialog("非常抱歉，解析比赛数据时出现故障，烦请发送比赛编号 " + match_id + " 给 yaoyiming123@live.com，感谢！");
                this.Frame.Navigate(typeof(BlankPage));
                return;
            }

            try
            {
                MatchData_FBTextBlock.Text = (Convert.ToInt32(list[0]) / 60) + ":" + (Convert.ToInt32(list[0]) % 60).ToString().PadLeft(2, '0');
            }
            catch { }
            try
            {
                MatchData_GameTimeTextBlock.Text = (Convert.ToInt32(list[1]) / 60) + ":" + (Convert.ToInt32(list[1]) % 60).ToString().PadLeft(2, '0');
            }
            catch { }
            try
            {
                MatchData_GameModeTextBlock.Text = ConstantsHelper.GameModeID[Convert.ToInt32(list[2])];
                if (MatchData_GameModeTextBlock.Text == "暗月来袭" || MatchData_GameModeTextBlock.Text == "小贪魔节" || MatchData_GameModeTextBlock.Text == "-")
                {
                    MaskGrid.Visibility = Visibility.Visible;
                    MatchInfoScrollViewer.IsEnabled = false;
                    return;
                }
            }
            catch { }
            try
            {
                DownloadHyperlinkButton.NavigateUri = new Uri(list[3]);
            }
            catch
            {
                DownloadHyperlinkButton.IsEnabled = false;
            }
            RadiantScoreTextBlock.Text = "Score: " + list[4];
            DireScoreTextBlock.Text = "Score: " + list[5];
            switch (list[6])
            {
                case "9":
                    MatchData_GameModeTextBlock.Foreground = new SolidColorBrush(Colors.Brown);//"勇士联赛"
                    break;
                case "5":
                case "6":
                case "7":
                    MatchData_GameModeTextBlock.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);//"天梯"
                    break;
                default:
                    break;
            }
            try
            {
                DireWinTextBlock.Text = list[7] == "true" ? "LOSE" : "WIN";
                RadiantWinTextBlock.Text = list[7] == "true" ? "WIN" : "LOSE";
            }
            catch { }
            string[] radiant_gold_adv = list[8].Split(',');
            string[] radiant_xp_adv = list[9].Split(',');
            try
            {
                DrawChart(radiant_gold_adv, radiant_xp_adv);
            }
            catch
            {
                ChartGrid.Visibility = Visibility.Collapsed;
                NoChartNoteStackPanel.Visibility = Visibility.Visible;
            }
        }

        private async void ShowPlayers(string match_id)
        {
            List<PlayersInfo> players = await DotaMatchHelper.GetPlayersInfoAsync(match_id);
            if (players == null)
            {
                ShowDialog("玩家列表数据格式错误无法解析，烦请将比赛编号 " + match_id + " 发送给开发者 yaoyiming123@live.com，谢谢！");
                return;
            }

            double radiant_herodamage = 0;
            double radiant_score = 0;
            double dire_herodamage = 0;
            double dire_score = 0;
            for (int i = 0; i < 5; i++)
            {
                radiantPlayersList.Add(new PlayersInfoViewModel(players[i]) { PlayerName = "Anonymous", PlayerPhoto = "ms-appx:///Assets/Pictures/null.png" });
                radiant_herodamage += Convert.ToDouble(players[i].Hero_damage);
                radiant_score += Convert.ToDouble(players[i].Kills);
            }
            foreach (PlayersInfoViewModel item in radiantPlayersList)
            {
                item.Fight_rate = "参战率: " + (100 * (item.Kills + item.Assists) / radiant_score).ToString("0.0") + "%";
                item.Damage_rate = "Damage: " + (100 * Convert.ToDouble(item.Hero_damage) / radiant_herodamage).ToString("0.0") + "%";
            }
            for (int i = 5; i < 10; i++)
            {
                direPlayersList.Add(new PlayersInfoViewModel(players[i]) { PlayerName = "Anonymous", PlayerPhoto = "ms-appx:///Assets/Pictures/null.png" });
                dire_herodamage += Convert.ToDouble(players[i].Hero_damage);
                dire_score += Convert.ToDouble(players[i].Kills);
            }
            foreach (PlayersInfoViewModel item in direPlayersList)
            {
                item.Fight_rate = "参战率: " + (100 * (item.Kills + item.Assists) / dire_score).ToString("0.0") + "%";
                item.Damage_rate = "Damage: " + (100 * Convert.ToDouble(item.Hero_damage) / dire_herodamage).ToString("0.0") + "%";
            }
            ShowPlayersPhoto(radiantPlayersList, direPlayersList);
        }

        private async void ShowPlayersPhoto(List<PlayersInfoViewModel> radiant, List<PlayersInfoViewModel> dire)
        {
            foreach (PlayersInfoViewModel item in radiant)
            {
                if (RadiantProgressRing.IsActive)
                {
                    RadiantProgressRing.IsActive = false;
                    RadiantProgressRing.Visibility = Visibility.Collapsed;
                }

                try
                {
                    //先判断一下这一个玩家是不是自己，是的话就不要请求数据了，避免造成超出请求速率限制
                    if (item.Account_id == MatchesPage.MatchPlayerProfile.profile.account_id.ToString())
                    {
                        item.PlayerName = MatchesPage.MatchPlayerProfile.profile.personaname;
                        item.PlayerPhoto = MatchesPage.MatchPlayerProfile.profile.avatarmedium;
                        radiantPlayers.Add(item);
                        continue;
                    }
                }
                catch { }
                //查看是否已经缓存这个账号的信息
                if (DotaMatchHelper.PlayersNameCache.ContainsKey(item.Account_id))
                {
                    item.PlayerName = DotaMatchHelper.PlayersNameCache[item.Account_id];
                    item.PlayerPhoto = DotaMatchHelper.PlayersPhotoCache[item.Account_id];
                    radiantPlayers.Add(item);
                }
                else
                {
                    string url = String.Format("https://api.opendota.com/api/players/{0}", item.Account_id);
                    HttpClient http = new HttpClient();
                    List<PlayersInfo> playersInfoList = new List<PlayersInfo>();
                    try
                    {
                        var response = await http.GetAsync(new Uri(url));
                        var jsonMessage = await response.Content.ReadAsStringAsync();

                        if (jsonMessage.StartsWith("{\"error\":}"))
                        {
                            item.PlayerName = "Anonymous";
                            item.PlayerPhoto = "ms-appx:///Assets/Pictures/null.png";
                        }
                        else
                        {
                            try
                            {
                                Match personanameMatch = Regex.Match(jsonMessage, "\\\"personaname\\\":\\\"([\\s\\S]*?)\\\",");
                                Match photoMatch = Regex.Match(jsonMessage, "\\\"avatarmedium\\\":\\\"([\\s\\S]*?)\\\",");

                                item.PlayerName = personanameMatch.Groups[1].Value == "" ? "Anonymous" : personanameMatch.Groups[1].Value;
                                item.PlayerPhoto = photoMatch.Groups[1].Value == "" ? "ms-appx:///Assets/Pictures/null.png" : photoMatch.Groups[1].Value;
                            }
                            catch
                            {
                                item.PlayerName = "Anonymous";
                                item.PlayerPhoto = "ms-appx:///Assets/Pictures/null.png";
                            }
                        }
                        radiantPlayers.Add(item);
                        if (item.PlayerPhoto != "ms-appx:///Assets/Pictures/null.png")
                        {
                            DotaMatchHelper.PlayersNameCache.Add(item.Account_id, item.PlayerName);
                            DotaMatchHelper.PlayersPhotoCache.Add(item.Account_id, item.PlayerPhoto);
                        }
                    }
                    catch
                    {
                        ShowDialog("The data query may be too frequent, please try again later.");
                        return;
                    }
                }
            }
            foreach (PlayersInfoViewModel item in dire)
            {
                if (DireProgressRing.IsActive)
                {
                    DireProgressRing.IsActive = false;
                    DireProgressRing.Visibility = Visibility.Collapsed;
                }

                try
                {
                    if (item.Account_id == MatchesPage.MatchPlayerProfile.profile.account_id.ToString())
                    {
                        item.PlayerName = MatchesPage.MatchPlayerProfile.profile.personaname;
                        item.PlayerPhoto = MatchesPage.MatchPlayerProfile.profile.avatarmedium;
                        direPlayers.Add(item);
                        continue;
                    }
                }
                catch { }
                if (DotaMatchHelper.PlayersNameCache.ContainsKey(item.Account_id))
                {
                    item.PlayerName = DotaMatchHelper.PlayersNameCache[item.Account_id];
                    item.PlayerPhoto = DotaMatchHelper.PlayersPhotoCache[item.Account_id];
                    direPlayers.Add(item);
                }
                else
                {
                    string url = String.Format("https://api.opendota.com/api/players/{0}", item.Account_id);
                    HttpClient http = new HttpClient();
                    List<PlayersInfo> playersInfoList = new List<PlayersInfo>();
                    try
                    {
                        var response = await http.GetAsync(new Uri(url));
                        var jsonMessage = await response.Content.ReadAsStringAsync();

                        if (jsonMessage == "{\"error\":\"rate limit exceeded\"}")
                        {
                            item.PlayerName = "Anonymous";
                            item.PlayerPhoto = "ms-appx:///Assets/Pictures/null.png";
                        }
                        else
                        {
                            try
                            {
                                Match personanameMatch = Regex.Match(jsonMessage, "\\\"personaname\\\":\\\"([\\s\\S]*?)\\\",");
                                Match photoMatch = Regex.Match(jsonMessage, "\\\"avatarmedium\\\":\\\"([\\s\\S]*?)\\\",");
                                item.PlayerName = personanameMatch.Groups[1].Value == "" ? "Anonymous" : personanameMatch.Groups[1].Value;
                                item.PlayerPhoto = photoMatch.Groups[1].Value == "" ? "ms-appx:///Assets/Pictures/null.png" : photoMatch.Groups[1].Value;
                            }
                            catch
                            {
                                item.PlayerName = "Anonymous";
                                item.PlayerPhoto = "ms-appx:///Assets/Pictures/null.png";
                            }
                        }
                        direPlayers.Add(item);
                        if (item.PlayerPhoto != "ms-appx:///Assets/Pictures/null.png")
                        {
                            DotaMatchHelper.PlayersNameCache.Add(item.Account_id, item.PlayerName);
                            DotaMatchHelper.PlayersPhotoCache.Add(item.Account_id, item.PlayerPhoto);
                        }
                    }
                    catch
                    {
                        ShowDialog("The data query may be too frequent, please try again later.");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制曲线图
        /// </summary>
        private void DrawChart(string[] adv_gold, string[] adv_xp)
        {
            List<int> gold_adv = new List<int>();
            List<int> xp_adv = new List<int>();

            foreach (string item in adv_gold)
            {
                try
                {
                    gold_adv.Add(Convert.ToInt32(item));
                }
                catch
                {
                    gold_adv.Add(0);
                }
            }
            foreach (string item in adv_xp)
            {
                try
                {
                    xp_adv.Add(Convert.ToInt32(item));
                }
                catch
                {
                    xp_adv.Add(0);
                }
            }
            if (gold_adv.Count < 2 || xp_adv.Count < 2)
            {
                ChartGrid.Visibility = Visibility.Collapsed;
                NoChartNoteStackPanel.Visibility = Visibility.Visible;
                // DataChart.DataContext = this;
                return;
            }
            double time_gap = 560.0 / (gold_adv.Count - 1);
            int max = gold_adv.Max() > xp_adv.Max() ? gold_adv.Max() : xp_adv.Max();
            int min = gold_adv.Min() < xp_adv.Min() ? gold_adv.Min() : xp_adv.Min();
            GraphTextBlock1.Text = max.ToString();
            GraphTextBlock2.Text = ((max - (max - min) / 4) < 0 ? -(max - (max - min) / 4) : (max - (max - min) / 4)).ToString();
            GraphTextBlock3.Text = ((max - 2 * (max - min) / 4) < 0 ? -(max - 2 * (max - min) / 4) : (max - 2 * (max - min) / 4)).ToString();
            GraphTextBlock4.Text = ((max - 3 * (max - min) / 4) < 0 ? -(max - 3 * (max - min) / 4) : (max - 3 * (max - min) / 4)).ToString();
            GraphTextBlock5.Text = (-min).ToString();
            ZeroPolyline.Points = new PointCollection { new Point(0, 480 * (max - gold_adv[0]) / (max - min)), new Point(560, 480 * (max - gold_adv[0]) / (max - min)) };
            GraphTextBlock0.Margin = new Thickness(0, 12 + 480 * (max - gold_adv[0]) / (max - min), 8, 0);
            try
            {
                for (int i = 0; i < gold_adv.Count; i++)
                {
                    GoldPolyline.Points.Add(new Point(time_gap * i, 480 * (max - gold_adv[i]) / (max - min)));
                    XpPolyline.Points.Add(new Point(time_gap * i, 480 * (max - xp_adv[i]) / (max - min)));
                }
            }
            catch
            {
                ChartGrid.Visibility = Visibility.Collapsed;
                NoChartNoteStackPanel.Visibility = Visibility.Visible;
                return;
            }
            ChartGrid.Visibility = Visibility.Visible;
            NoChartNoteStackPanel.Visibility = Visibility.Collapsed;

            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "经济",
            //        Values = gold_adv,
            //        PointForeround = new SolidColorBrush(Colors.Gold),
            //        PointGeometrySize = 4,
            //        LineSmoothness = 0,
            //        Fill=new SolidColorBrush(new Color(){ A = 128, R = 255, G = 215, B = 0})
            //    },
            //    new LineSeries
            //    {
            //        Title = "经验",
            //        Values = xp_adv,
            //        PointForeround = new SolidColorBrush(Colors.DeepSkyBlue),
            //        PointGeometrySize = 4,
            //        LineSmoothness = 0,
            //        Fill=new SolidColorBrush(new Color(){ A = 128, R = 0, G = 191, B = 255})
            //    }
            //};

            //List<string> timeList = new List<string>();
            //for (int i = 0; i < gold_adv.Count; i++)
            //{
            //    timeList.Add(i.ToString());
            //}
            //Labels = timeList.ToArray();
            //YFormatter = value => value.ToString();
            //DataChart.DataContext = this;
        }

        /// <summary>
        /// 显示团战Json数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TeamFightTextBox.Text != "")
            {
                return;
            }
            TeamFightWaitStackPanel.Visibility = Visibility.Visible;
            TeamFightProgressRing.IsActive = true;
            TeamFightTextBox.Visibility = Visibility.Visible;
            TeamFightTextBox.Text = await DotaMatchHelper.GetTeamfightInfoAsync(match_id);
            TeamFightProgressRing.IsActive = false;
            TeamFightWaitStackPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 点击复制比赛编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dp = new DataPackage();
            dp.SetText(MatchIDTextBlock.Text);
            Clipboard.SetContent(dp);
            CopySuccessGrid.Visibility = Visibility.Visible;
            ShowCopySuccess.Begin();
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        public async void ShowDialog(string content)
        {
            var dialog = new ContentDialog()
            {
                Title = ":(",
                Content = content,
                PrimaryButtonText = "Okay",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (_s, _e) =>
            {
                this.Frame.Navigate(typeof(BlankPage));
            };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }

        /// <summary>
        /// 点击查看指定玩家数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiantListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadiantListView.SelectedIndex >= 0)
            {
                //this.NavigationCacheMode = NavigationCacheMode.Enabled;

                HeroPlayerInfoViewModel heroPlayerInfoViewModel = DotaMatchHelper.GetHeroPlayerInfo(RadiantListView.SelectedIndex);
                PlayersInfoViewModel temp = radiantPlayers[RadiantListView.SelectedIndex];
                heroPlayerInfoViewModel.Account_id = "ID: " + temp.Account_id;
                heroPlayerInfoViewModel.Personaname = temp.PlayerName;
                heroPlayerInfoViewModel.KDA = Regex.Match(temp.KDA, "KDA: ([\\d\\D]*)").Groups[1].Value;
                heroPlayerInfoViewModel.Level = "Lvl" + temp.Level;
                heroPlayerInfoViewModel.Last_hits = Regex.Match(temp.LD, "LH/DN: ([\\d\\D]*?)/").Groups[1].Value;
                heroPlayerInfoViewModel.Denies = Regex.Match(temp.LD, "LH/DN: [\\d\\D]*?/([\\d\\D]*)").Groups[1].Value;
                heroPlayerInfoViewModel.KDAString = "KDA: " + temp.K_D_A.ToString();

                this.Frame.Navigate(typeof(MatchPlayerPage), heroPlayerInfoViewModel);
            }
        }

        /// <summary>
        /// 点击查看指定玩家数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DireListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DireListView.SelectedIndex >= 0)
            {
                //this.NavigationCacheMode = NavigationCacheMode.Enabled;

                HeroPlayerInfoViewModel heroPlayerInfoViewModel = DotaMatchHelper.GetHeroPlayerInfo(DireListView.SelectedIndex + 5);
                PlayersInfoViewModel temp = direPlayers[DireListView.SelectedIndex];
                heroPlayerInfoViewModel.Account_id = "ID: " + temp.Account_id;
                heroPlayerInfoViewModel.Personaname = temp.PlayerName;
                heroPlayerInfoViewModel.KDA = Regex.Match(temp.KDA, "KDA: ([\\d\\D]*)").Groups[1].Value;
                heroPlayerInfoViewModel.Level = "Lvl" + temp.Level;
                heroPlayerInfoViewModel.Last_hits = Regex.Match(temp.LD, "LH/DN: ([\\d\\D]*?)/").Groups[1].Value;
                heroPlayerInfoViewModel.Denies = Regex.Match(temp.LD, "LH/DN: [\\d\\D]*?/([\\d\\D]*)").Groups[1].Value;
                heroPlayerInfoViewModel.KDAString = "KDA: " + temp.K_D_A.ToString();

                this.Frame.Navigate(typeof(MatchPlayerPage), heroPlayerInfoViewModel);
            }
        }
    }
}
