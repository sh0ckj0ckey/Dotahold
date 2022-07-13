using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using OpenDota_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroInfoPage : Page
    {
        DotaHeroesViewModel ViewModel = null;
        DotaViewModel MainViewModel = null;

        private Style _customDialogStyle = null;

        //HeroAttr heroAttr = null;
        //bool InitializeFinished = false;
        //ObservableCollection<HeroAbility> abilitiesObservableCollection = new ObservableCollection<HeroAbility>();
        //List<RankPlayer> rankPlayers;
        //ObservableCollection<RankPlayer> rankPlayersObservableCollection = new ObservableCollection<RankPlayer>();
        public HeroInfoPage()
        {
            this.InitializeComponent();

            try
            {
                ViewModel = DotaHeroesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                ViewModel.ActPopInHeroInfoGrid += () =>
                {
                    try
                    {
                        PopInHeroInfoStoryboard?.Begin();
                    }
                    catch { }
                };

                ViewModel.ActShowHeroHistoryButton += () =>
                {
                    try
                    {
                        ShowHeroHistoryButtonStoryboard?.Begin();
                    }
                    catch { }
                };
            }
            catch { }
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ShowBackgroundImage?.Begin();
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("animateHeroInfoPhoto");
                if (animation != null)
                {
                    animation.TryStart(HeroPhotoBorder, new UIElement[] { HeroNameGrid });
                }
            }
            catch { }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation =
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("animateBackHeroPhoto", HeroPhotoBorder);

                // Use the recommended configuration for back animation.
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }

        /// <summary>
        /// 加载选择的英雄对应的HTML代码
        /// </summary>
        public async void ShowHero()
        {
            //string html = await CrawlerHelper.InitializeHtml(HeroesPage.SelectedHero.ID);
            //if (html == "gg")
            //{
            //    //ShowDialog("Something went wrong, plz retry...");
            //    return;
            //}
            ////ShowTheHeroBio(html);
            ////ShowTheHeroTalent(html);
            ////ShowTheHeroAbility(html);
            ////ShowTheHeroAttr();
            ////ShowPlayersBoard();
            //InitializeFinished = true;
        }

        /// <summary>
        /// 显示英雄三围属性成长
        /// </summary>
        //private async void ShowTheHeroAttr()
        //{
        //    heroAttr = await DotaHeroHelper.GetHeroAttr(Array.IndexOf(ConstantsHelper.HeroID, SelectedHero.ID).ToString());
        //    if (heroAttr == null)
        //    {
        //        ShowDialog("Sorry, failed to load hero's attr.");
        //        return;
        //    }
        //    //初始属性
        //    StrTextBlock.Text = heroAttr.Base_str;
        //    AgiTextBlock.Text = heroAttr.Base_agi;
        //    IntTextBlock.Text = heroAttr.Base_int;

        //    HealthTextBlock.Text = (200 + ConvertString2Double(heroAttr.Base_str) * 20).ToString("f0");
        //    ManaTextBlock.Text = (75 + ConvertString2Double(heroAttr.Base_int) * 12).ToString("f0");
        //    MsTextBlock.Text = (ConvertString2Double(heroAttr.Move_speed) * (1 + ConvertString2Double(heroAttr.Base_agi) * 0.0005)).ToString("f0");
        //    ArmorTextBlock.Text = (ConvertString2Double(heroAttr.Base_armor) + ConvertString2Double(heroAttr.Base_agi) * 0.16).ToString("f1");
        //    HealthRegenTextBlock.Text = (ConvertString2Double(heroAttr.Base_health_regen) + 0.09 * ConvertString2Double(heroAttr.Base_str)).ToString("f1");
        //    ManaRegenTextBlock.Text = (ConvertString2Double(heroAttr.Base_mana_regen) + 0.05 * ConvertString2Double(heroAttr.Base_int)).ToString("f1");

        //    double DmgAddition = 0;
        //    //switch (HeroesPage.selectedHeroPA)
        //    //{
        //    //    case 1:
        //    //        DmgAddition = ConvertString2Double(heroAttr.Base_str);
        //    //        StrTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
        //    //        StrEllipse.Visibility = Visibility.Visible;
        //    //        break;
        //    //    case 2:
        //    //        DmgAddition = ConvertString2Double(heroAttr.Base_agi);
        //    //        AgiTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
        //    //        AgiEllipse.Visibility = Visibility.Visible;
        //    //        break;
        //    //    case 3:
        //    //        DmgAddition = ConvertString2Double(heroAttr.Base_int);
        //    //        IntTextBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
        //    //        IntEllipse.Visibility = Visibility.Visible;
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //    DmgTextBlock.Text = (ConvertString2Double(heroAttr.Base_attack_min) + DmgAddition) + " - " + (ConvertString2Double(heroAttr.Base_attack_max) + DmgAddition);
        //    AttackRangeTextBlock.Text = heroAttr.Attack_range;
        //    AttkRateTextBlock.Text = heroAttr.Attack_rate;
        //    TurnRateTextBlock.Text = heroAttr.Turn_rate;
        //    ProjectileSpeedTextBlock.Text = heroAttr.Projectile_speed;
        //    CMModeTextBlock.Text = heroAttr.Cm_enabled == "true" ? "是" : "否";

        //    double[] pick = new double[] { ConvertString2Double(heroAttr._1_pick), ConvertString2Double(heroAttr._2_pick), ConvertString2Double(heroAttr._3_pick), ConvertString2Double(heroAttr._4_pick), ConvertString2Double(heroAttr._5_pick), ConvertString2Double(heroAttr._6_pick), ConvertString2Double(heroAttr._7_pick), ConvertString2Double(heroAttr._8_pick) };
        //    double[] win = new double[] { ConvertString2Double(heroAttr._1_win), ConvertString2Double(heroAttr._2_win), ConvertString2Double(heroAttr._3_win), ConvertString2Double(heroAttr._4_win), ConvertString2Double(heroAttr._5_win), ConvertString2Double(heroAttr._6_win), ConvertString2Double(heroAttr._7_win), ConvertString2Double(heroAttr._8_win) };
        //    double max = pick.Max();
        //    double ratio = 328.0 / max;
        //    Pick_1Rectangle.Width = ConvertString2Double(heroAttr._1_pick) * ratio;
        //    Pick_2Rectangle.Width = ConvertString2Double(heroAttr._2_pick) * ratio;
        //    Pick_3Rectangle.Width = ConvertString2Double(heroAttr._3_pick) * ratio;
        //    Pick_4Rectangle.Width = ConvertString2Double(heroAttr._4_pick) * ratio;
        //    Pick_5Rectangle.Width = ConvertString2Double(heroAttr._5_pick) * ratio;
        //    Pick_6Rectangle.Width = ConvertString2Double(heroAttr._6_pick) * ratio;
        //    Pick_7Rectangle.Width = ConvertString2Double(heroAttr._7_pick) * ratio;
        //    Pick_8Rectangle.Width = ConvertString2Double(heroAttr._8_pick) * ratio;

        //    Win_1Rectangle.Width = ConvertString2Double(heroAttr._1_win) * ratio;
        //    Win_2Rectangle.Width = ConvertString2Double(heroAttr._2_win) * ratio;
        //    Win_3Rectangle.Width = ConvertString2Double(heroAttr._3_win) * ratio;
        //    Win_4Rectangle.Width = ConvertString2Double(heroAttr._4_win) * ratio;
        //    Win_5Rectangle.Width = ConvertString2Double(heroAttr._5_win) * ratio;
        //    Win_6Rectangle.Width = ConvertString2Double(heroAttr._6_win) * ratio;
        //    Win_7Rectangle.Width = ConvertString2Double(heroAttr._7_win) * ratio;
        //    Win_8Rectangle.Width = ConvertString2Double(heroAttr._8_win) * ratio;

        //    Win_1TextBlock.Text = heroAttr._1_win;
        //    Pick_1TextBlock.Text = heroAttr._1_pick;
        //    Rate_1TextBlock.Text = (100 * ConvertString2Double(heroAttr._1_win) / ConvertString2Double(heroAttr._1_pick)).ToString("f1") + "%";
        //    Win_2TextBlock.Text = heroAttr._2_win;
        //    Pick_2TextBlock.Text = heroAttr._2_pick;
        //    Rate_2TextBlock.Text = (100 * ConvertString2Double(heroAttr._2_win) / ConvertString2Double(heroAttr._2_pick)).ToString("f1") + "%";
        //    Win_3TextBlock.Text = heroAttr._3_win;
        //    Pick_3TextBlock.Text = heroAttr._3_pick;
        //    Rate_3TextBlock.Text = (100 * ConvertString2Double(heroAttr._3_win) / ConvertString2Double(heroAttr._3_pick)).ToString("f1") + "%";
        //    Win_4TextBlock.Text = heroAttr._4_win;
        //    Pick_4TextBlock.Text = heroAttr._4_pick;
        //    Rate_4TextBlock.Text = (100 * ConvertString2Double(heroAttr._4_win) / ConvertString2Double(heroAttr._4_pick)).ToString("f1") + "%";
        //    Win_5TextBlock.Text = heroAttr._5_win;
        //    Pick_5TextBlock.Text = heroAttr._5_pick;
        //    Rate_5TextBlock.Text = (100 * ConvertString2Double(heroAttr._5_win) / ConvertString2Double(heroAttr._5_pick)).ToString("f1") + "%";
        //    Win_6TextBlock.Text = heroAttr._6_win;
        //    Pick_6TextBlock.Text = heroAttr._6_pick;
        //    Rate_6TextBlock.Text = (100 * ConvertString2Double(heroAttr._6_win) / ConvertString2Double(heroAttr._6_pick)).ToString("f1") + "%";
        //    Win_7TextBlock.Text = heroAttr._7_win;
        //    Pick_7TextBlock.Text = heroAttr._7_pick;
        //    Rate_7TextBlock.Text = (100 * ConvertString2Double(heroAttr._7_win) / ConvertString2Double(heroAttr._7_pick)).ToString("f1") + "%";
        //    Win_8TextBlock.Text = heroAttr._8_win;
        //    Pick_8TextBlock.Text = heroAttr._8_pick;
        //    Rate_8TextBlock.Text = (100 * ConvertString2Double(heroAttr._8_win) / ConvertString2Double(heroAttr._8_pick)).ToString("f1") + "%";
        //}

        /// <summary>
        /// 显示英雄定位和背景
        /// </summary>
        //private async void ShowTheHeroBio(string html)
        //{
        //    HeroBio heroBio = await CrawlerHelper.GetHeroBio(SelectedHero.ID, html);
        //    if (heroBio == null)
        //    {
        //        return;
        //    }
        //    AtkTextBlock.Text = heroBio.Atk;
        //    RoleTextBlock.Text = heroBio.Role;
        //    if (heroBio.Bio.Replace(" ", "") == "")
        //    {
        //        heroBio.Bio = await CrawlerHelper.GetHeroBioBackup(SelectedHero.ID, html);
        //    }
        //    HeroInfoTextBlock.Text = heroBio.Bio;

        //    HeroBioProgressRing.IsActive = false;
        //    HeroBioProgressRing.Visibility = Visibility.Collapsed;
        //}

        /// <summary>
        /// 显示英雄天赋树
        /// </summary>
        //private async void ShowTheHeroTalent(string html)
        //{
        //    HeroTalent heroTalent = await CrawlerHelper.GetHeroTalent(SelectedHero.ID, html);
        //    if (heroTalent == null)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        Talent_25_leftTextBlock.Text = heroTalent.Talent_25_left;
        //        Talent_25_rightTextBlock.Text = heroTalent.Talent_25_right;
        //        Talent_20_leftTextBlock.Text = heroTalent.Talent_20_left;
        //        Talent_20_rightTextBlock.Text = heroTalent.Talent_20_right;
        //        Talent_15_leftTextBlock.Text = heroTalent.Talent_15_left;
        //        Talent_15_rightTextBlock.Text = heroTalent.Talent_15_right;
        //        Talent_10_leftTextBlock.Text = heroTalent.Talent_10_left;
        //        Talent_10_rightTextBlock.Text = heroTalent.Talent_10_right;
        //    }
        //    catch { }
        //}

        /// <summary>
        /// 显示英雄技能
        /// </summary>
        //private async void ShowTheHeroAbility(string html)
        //{
        //    List<HeroAbility> heroAbility = await CrawlerHelper.GetHeroAbility(SelectedHero.ID, html);
        //    if (heroAbility == null)
        //    {
        //        return;
        //    }
        //    foreach (HeroAbility item in heroAbility)
        //    {
        //        abilitiesObservableCollection.Add(item);
        //    }
        //    HeroAbilityListView.ItemsSource = abilitiesObservableCollection;
        //}

        /// <summary>
        /// 显示英雄榜
        /// </summary>
        //private async void ShowPlayersBoard()
        //{
        //    rankPlayers = await DotaHeroHelper.GetHeroPlayersAsync(Array.IndexOf(ConstantsHelper.HeroID, SelectedHero.ID).ToString());
        //    if (rankPlayers == null)
        //    {
        //        FailedTextBlock.Visibility = Visibility.Visible;
        //        PlayersIndexGridView.SelectedIndex = -1;
        //        PlayersIndexGridView.IsEnabled = false;
        //        LeftHyperlinkButton.IsEnabled = false;
        //        RightHyperlinkButton.IsEnabled = false;
        //        return;
        //    }
        //    HeroPlayerProgressRing.IsActive = false;
        //    HeroPlayerProgressRing.Visibility = Visibility.Collapsed;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        rankPlayers[i].Rank = i + 1;
        //        rankPlayersObservableCollection.Add(rankPlayers[i]);
        //    }
        //    PlayersListView.ItemsSource = rankPlayersObservableCollection;
        //    PlayersIndexGridView.SelectedIndex = 0;
        //}

        /// <summary>
        /// 选择英雄榜的页码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (PlayersIndexGridView.SelectedIndex < 0)
        //    {
        //        return;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex < 1)
        //    {
        //        LeftHyperlinkButton.IsEnabled = false;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex < 9)
        //    {
        //        RightHyperlinkButton.IsEnabled = true;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex > 8)
        //    {
        //        RightHyperlinkButton.IsEnabled = false;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex > 0)
        //    {
        //        LeftHyperlinkButton.IsEnabled = true;
        //    }

        //    if (rankPlayers != null && rankPlayers.Count >= 91)
        //    {
        //        int start = PlayersIndexGridView.SelectedIndex + 1;
        //        rankPlayersObservableCollection.Clear();
        //        for (int i = start * 10 - 10; i < start * 10; i++)
        //        {
        //            try
        //            {
        //                rankPlayers[i].Rank = i + 1;
        //                rankPlayersObservableCollection.Add(rankPlayers[i]);
        //            }
        //            catch
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 选择英雄榜前一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void LeftHyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (PlayersIndexGridView.SelectedIndex > 0)
        //    {
        //        PlayersIndexGridView.SelectedIndex--;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex < 1)
        //    {
        //        LeftHyperlinkButton.IsEnabled = false;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex < 9)
        //    {
        //        RightHyperlinkButton.IsEnabled = true;
        //    }
        //}

        /// <summary>
        /// 选择英雄榜后一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void RightHyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (PlayersIndexGridView.SelectedIndex < 9)
        //    {
        //        PlayersIndexGridView.SelectedIndex++;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex > 8)
        //    {
        //        RightHyperlinkButton.IsEnabled = false;
        //    }
        //    if (PlayersIndexGridView.SelectedIndex > 0)
        //    {
        //        LeftHyperlinkButton.IsEnabled = true;
        //    }
        //}

        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch { }
        }

        public double ConvertString2Double(string value)
        {
            if (Double.TryParse(value, out double result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel?.CurrentHeroInfo == null) return;

                string loc = TrimHeroHistory(ViewModel.CurrentHeroInfo.bio_loc);
                ViewModel.CurrentHeroInfo.bio_loc = loc;

                if (_customDialogStyle == null)
                {
                    _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
                }

                ContentDialog dialog = new ContentDialog();

                if (_customDialogStyle != null)
                {
                    dialog.Style = _customDialogStyle;
                }

                dialog.CloseButtonText = "Close";
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.Content = new UI.HeroHistoryDialogContent();
                dialog.RequestedTheme = MainViewModel.eAppTheme;

                _ = await dialog.ShowAsync();
            }
            catch { }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private string TrimHeroHistory(string history)
        {
            try
            {
                string strText = System.Text.RegularExpressions.Regex.Replace(history, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                strText = strText.Replace("\t", "");
                strText = strText.Replace("\r", "\n");
                return strText;
            }
            catch { }
            return history;
        }
    }
}
