using OpenDota_UWP.Helpers;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchPlayerPage : Page
    {
        HeroPlayerInfoViewModel HeroPlayerInfo = null;
        ObservableCollection<AbilityViewModel> abilityNames = new ObservableCollection<AbilityViewModel>();

        public MatchPlayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //HeroPlayerInfo = (e.Parameter as HeroPlayerInfoViewModel);
            //if (HeroPlayerInfo == null)
            //{
            //    ShowDialog();
            //    return;
            //}
            //foreach (var item in HeroPlayerInfo.Permanent_buffs)
            //{
            //    switch (item.Permanent_buff)
            //    {
            //        case "1":
            //            Buff1Grid.Visibility = Visibility.Visible;
            //            Buff1TextBlock.Text = item.Stack_count;
            //            break;
            //        case "2":
            //            Buff2Grid.Visibility = Visibility.Visible;
            //            Buff2TextBlock.Text = item.Stack_count;
            //            break;
            //        case "3":
            //            Buff3Grid.Visibility = Visibility.Visible;
            //            Buff3TextBlock.Text = item.Stack_count;
            //            break;
            //        case "4":
            //            Buff4Grid.Visibility = Visibility.Visible;
            //            Buff4TextBlock.Text = item.Stack_count;
            //            break;
            //        case "5":
            //            Buff5Grid.Visibility = Visibility.Visible;
            //            Buff5TextBlock.Text = item.Stack_count;
            //            break;
            //        case "6":
            //            Buff6Grid.Visibility = Visibility.Visible;
            //            Buff6TextBlock.Text = item.Stack_count;
            //            break;
            //        case "7":
            //            Buff7Grid.Visibility = Visibility.Visible;
            //            Buff7TextBlock.Text = item.Stack_count;
            //            break;
            //        case "8":
            //            Buff8Grid.Visibility = Visibility.Visible;
            //            Buff8TextBlock.Text = item.Stack_count;
            //            break;
            //        case "9":
            //            Buff9Grid.Visibility = Visibility.Visible;
            //            Buff9TextBlock.Text = item.Stack_count;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //string rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRank0-0.png";
            //if (HeroPlayerInfo.Rank_tier == null) { }
            //else if (HeroPlayerInfo.Rank_tier[0] == '8')
            //{
            //    rankMedalSource = "ms-appx:///Assets/RankMedal/SeasonalRankTop0.png";
            //}
            //else if (HeroPlayerInfo.Rank_tier.Length == 2)
            //{
            //    rankMedalSource = string.Format("ms-appx:///Assets/RankMedal/SeasonalRank{0}-{1}.png", HeroPlayerInfo.Rank_tier[0], HeroPlayerInfo.Rank_tier[1]);
            //}
            //else { }
            //MatchData_RankMedalImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(rankMedalSource));

            //for (int i = 0; i < HeroPlayerInfo.Ability_upgrades_arr.Count; i++)
            //{
            //    try
            //    {
            //        AbilityViewModel temp = new AbilityViewModel()
            //        {
            //            Ability = ConstantsHelper.abilitiesIDDictionary[HeroPlayerInfo.Ability_upgrades_arr[i]].StartsWith("special_bonus") ? "ms-appx:///Assets/Icons/talent.jpg" : string.Format("https://www.dota2.com.cn/images/heroes/abilities/{0}_hp1.png", ConstantsHelper.abilitiesIDDictionary[HeroPlayerInfo.Ability_upgrades_arr[i]])
            //        };
            //        string heroname = HeroPlayerInfo.Hero_name;
            //        if (heroname.ToLower() != "invoker")
            //        {
            //            if (i < 16)
            //            {
            //                temp.ID = (i + 1).ToString();
            //            }
            //            else if (i == 16)
            //            {
            //                temp.ID = "18";
            //            }
            //            else if (i == 17)
            //            {
            //                temp.ID = "20";
            //            }
            //            else if (i == 18)
            //            {
            //                temp.ID = "25";
            //            }
            //        }
            //        else
            //        {
            //            temp.ID = (i + 1).ToString();
            //        }
            //        abilityNames.Add(temp);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}

            //if (HeroPlayerInfo.Account_id == "ID: null")
            //{
            //    IDTextBlock.Visibility = Visibility.Collapsed;
            //}

            //base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (abilityNames != null)
            {
                abilityNames.Clear();
                abilityNames = null;
            }
            if (HeroPlayerInfo != null)
            {
                HeroPlayerInfo = null;
            }
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
