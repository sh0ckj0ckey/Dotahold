using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
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
    public sealed partial class ItemsPage : Page
    {
        private DotaItems SelectedItem = new DotaItems();
        public static string json = "";
        public static string jsonComponents = "";
        DotaItemsViewModel ViewModel = null;

        public ItemsPage()
        {
            this.InitializeComponent();
            ViewModel = DotaItemsViewModel.Instance;

            FrameShadow.Receivers.Add(ItemsListGrid);
            ItemGrid.Translation += new System.Numerics.Vector3(0, 0, 36);

            ItemFrame.Navigate(typeof(BlankPage));

            //if (AllItems.Count == 0)
            //{
            //    try
            //    {
            //        AddAllItems();
            //    }
            //    catch
            //    {
            //        AddAllItems();
            //    }
            //}
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }
                base.OnNavigatedTo(e);
            }
            catch { }
        }

        /// <summary>
        /// 将所有的物品添加到列表
        /// </summary>
        //private async void AddAllItems()
        //{
        //    ItemProgressRing.Visibility = Visibility.Visible;
        //    ItemProgressRing.IsActive = true;

        //    if (json == null || json == "")
        //    {
        //        try
        //        {
        //            json = await VM.GetItemDataAsync();
        //        }
        //        catch
        //        {
        //            NetworkSlowTextBlock.Visibility = Visibility.Visible;
        //            try
        //            {
        //                NetworkSlowTextBlock.Visibility = Visibility.Visible;
        //                json = await VM.GetItemDataAsync();
        //            }
        //            catch
        //            {
        //                return;
        //            }
        //        }
        //    }

        //    if (jsonComponents == null || jsonComponents == "")
        //    {
        //        try
        //        {
        //            jsonComponents = await VM.GetItemDataENAsync();
        //        }
        //        catch
        //        {
        //            NetworkSlowTextBlock.Visibility = Visibility.Visible;
        //            try
        //            {
        //                NetworkSlowTextBlock.Visibility = Visibility.Visible;
        //                json = await VM.GetItemDataENAsync();
        //            }
        //            catch
        //            {
        //                return;
        //            }
        //        }
        //    }


        //    ItemProgressRing.IsActive = false;
        //    ItemProgressRing.Visibility = Visibility.Collapsed;
        //    ProgressStackPanel.Visibility = Visibility.Collapsed;
        //}

        /// <summary>
        /// 点击列表显示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region
        private void ItemsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ItemsListView.SelectedIndex == -1)
            //{
            //    return;
            //}
            //SelectedItem = AllItems[ItemsListView.SelectedIndex];
            //GetItemInfo();
            //ItemsListView.SelectedIndex = -1;
        }

        #endregion

        /// <summary>
        /// 获取物品信息
        /// </summary>
        public void GetItemInfo()
        {
            //ItemPicture.Visibility = Visibility.Collapsed;
            //string regex = ("\"" + SelectedItem.ID + "\":{\"id\":\"[\\d]*?\",\"img\":\"[\\s\\S]*?\",\"dname\":\"([\\s\\S]*?)\",\"qual\":[\\s\\S]*?,\"cost\":([\\s\\S]*?),\"desc\":\"([\\s\\S]*?)\",\"notes\":\"([\\s\\S]*?)\",\"attrib\":\"([\\s\\S]*?)\",\"mc\":([\\s\\S]*?),\"cd\":([\\s\\S]*?),\"lore\":\"([\\s\\S]*?)\",\"components\":[\\s\\S]*?,\"created\":[\\s\\S]*?}");
            //Match match = Regex.Match(json, regex);

            //string regexComponents = ("\"" + SelectedItem.ID + "\":{\"id\":[\\s\\S]*?,\"components\":([\\s\\S]*?),\"created\":[\\s\\S]*?}");
            //Match matchComponents = Regex.Match(jsonComponents, regexComponents);

            //SelectedItem.Name = UnicodeToString(match.Groups[1].Value);
            //SelectedItem.Price = UnicodeToString(match.Groups[2].Value).Replace("\"", "");
            //SelectedItem.Info = UnicodeToString(match.Groups[3].Value.Replace(" \\/", "").Replace("<br>", "\r\n").Replace("<br />", "").Replace("<\\/h1>", "").Replace("<h1>", "").Replace("<span class=\\\"attribVal\\\">", "").Replace("<\\/span>", "").Replace("<span class=\\\"attribValText\\\">", "")
            //    .Replace("\\/", "/").Replace("\t", "\r\n").Replace("\\\\", "\\").Replace("\\r", "").Replace("\\n", "").Replace("<span class=\\\"GameplayValues GameplayVariable\\\">", "").Replace("<font color='#e03e2e'>", "\r\n").Replace("</font>", ""));
            //SelectedItem.Tips = UnicodeToString(match.Groups[4].Value.Replace(" \\/", "").Replace("\\\\", "\\").Replace("<br>", "\r\n").Replace("\\r", "").Replace("\\n", ""));
            //SelectedItem.Attributes = UnicodeToString(match.Groups[5].Value.Replace("\\/", "/").Replace("<span class=\\\"attribVal\\\">", "").Replace("</span>", "").Replace("<\\/span>", "").Replace("<span class=\\\"attribValText\\\">", "")
            //    .Replace("\\\\", "\\").Replace("<br>", "\r\n").Replace("< br>", "\r\n").Replace("\\r", "").Replace("\\n", "").Replace("<br />", "\r\n").Replace("<p class=\\\"pop_skill_p\\\">", "").Replace("<span class=\\\"color_yellow\\\">", "").Replace("</p>", "").Replace("+", "\r\n+"));
            //SelectedItem.Mana = UnicodeToString(match.Groups[6].Value.Replace("false", "0").Replace("\"\"", "0").Replace("\"", ""));
            //SelectedItem.CoolDown = UnicodeToString(match.Groups[7].Value.Replace("false", "0").Replace("\"\"", "0").Replace("\"", ""));
            //SelectedItem.Background = UnicodeToString(match.Groups[8].Value).Replace("\\n", "\n");
            //SelectedItem.Components = matchComponents.Groups[1].Value.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("\\", "").Split(',');
            //ItemFrame.Navigate(typeof(ItemInfoPage));
        }

        /// <summary>
        /// Unicode转换汉字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string UnicodeToString(string text)
        {
            System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(text, "\\\\u([\\w]{4})");
            if (mc != null && mc.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match m2 in mc)
                {
                    string v = m2.Value;
                    string word = v.Substring(2);
                    byte[] codes = new byte[2];
                    int code = Convert.ToInt32(word.Substring(0, 2), 16);
                    int code2 = Convert.ToInt32(word.Substring(2), 16);
                    codes[0] = (byte)code2;
                    codes[1] = (byte)code;
                    text = text.Replace(v, System.Text.Encoding.Unicode.GetString(codes));
                }
            }
            return text;
        }
    }
}
